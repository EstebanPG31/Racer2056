using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Rigidbody carRb;
    [SerializeField] private Transform[] rayPointsFront;
    [SerializeField] private Transform[] rayPointsBack;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationPoint;

    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness; //Ayuda a que el coche no se quede rebotando
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadiusFront;
    [SerializeField] private float wheelRadiusBack;

    private int[] wheelIsGrounded = new int[4];
    private bool isGrounded = false;

    [Header("Input")]
    private float moveInput = 0;
    private float steerInput = 0;

    [Header("CarSettings")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float steerStength = 15f;
    [SerializeField] private AnimationCurve turningCurve; //Calcula la fuerza del giro basándose en la velocidad del auto
    [SerializeField] private float dragCoefficient = 1f;

    private Vector3 currentCarLocalVelocity = Vector3.zero;
    private float carVelocityRatio = 0;

     private void Start()
    {
        carRb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        SuspensionFront();
        SuspensionBack();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
    }

    private void Update()
    {
        GetPlayerInput();
    }

    #region Movement

    private void Movement()
    {
        if (isGrounded)
        {
            Acceleration();
            Decelration();
            Turn();
            SidewaysDrag();
        }
    }

    private void Acceleration()
    {
        carRb.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
        //"acceleration" determina la magnitud de la fuerza a aplicar, "moveInput" hacia dónde quiere moverse el jugador (atrás o adelante),
        //y "transform.forward" porque queremos que la aceleración sea hacia adelante
    }

    private void Decelration()  //Usamos esta función para hacer que el auto no tarde tanto en detenerse, no incrementamos el atributo "drag" porque ese se aplica en todas las direcciones y 
    {                           //solo nos interesa que se reduzca el movimiento hacia adelante
        carRb.AddForceAtPosition(deceleration * moveInput * -transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Turn()
    {
        carRb.AddTorque(steerStength * steerInput * turningCurve.Evaluate(carVelocityRatio) * Mathf.Sign(carVelocityRatio) * transform.up, ForceMode.Acceleration);
        //steerStength calcula qué tan cerrado es el giro. turningCurve.Evaluate nos permite calcular de manera dinámica la fuerza del giro basándonos en la velocidad
        //Mathf.Sign para detectar si el auto va hacia adelante o atrás. Transform.up para que la rotación sea sobre ese eje
    }

    private void SidewaysDrag()
    {
        float currentSidewaysSpeed = currentCarLocalVelocity.x;

        float dragMagnitude = -currentSidewaysSpeed * dragCoefficient; //En negativo porque queremos contrarrestar el deslizamiento lateral

        Vector3 dragForce = transform.right * dragMagnitude;

        carRb.AddForceAtPosition(dragForce, carRb.worldCenterOfMass, ForceMode.Acceleration);
    }

    #endregion

    #region Car Status Check

    private void GroundCheck()
    {
        int tempGroundedWheels = 0;

        for (int i = 0; i < wheelIsGrounded.Length; i++)
        {
            tempGroundedWheels += wheelIsGrounded[i];
        }

        if (tempGroundedWheels > 1)  //Si hay más de una rueda tocando el suelo, el auto está en el suelo
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void CalculateCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(carRb.velocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed; //Devuelve un valor entre 0 y 1, donde 0 es que el auto está detenido y 1 que va a su máxima velocidad
    }
    #endregion

    #region Input Handling

    private void GetPlayerInput()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    #endregion

    #region Suspension Functions
    private void SuspensionFront()
    {
        for (int i =0; i<2; i++)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;

            if (Physics.Raycast(rayPointsFront[i].position, -rayPointsFront[i].up, out hit, maxLength + wheelRadiusFront, drivable)) //Si está tocando el suelo
            {
                wheelIsGrounded[i] = 1;

                float currentSpringLength = hit.distance - wheelRadiusFront;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(carRb.GetPointVelocity(rayPointsFront[i].position), rayPointsFront[i].up);
                float dampForce = damperStiffness * springVelocity; //fuerza que contrarestará el rebote innecesario de la suspension
               
                float springForce = springStiffness * springCompression;

                float netForce = springForce - dampForce;

                carRb.AddForceAtPosition(netForce * rayPointsFront[i].up, rayPointsFront[i].position); //Añade empuje hacia arriba al auto para simular el efecto de la suspension

                Debug.DrawLine(rayPointsFront[i].position, hit.point, Color.magenta);
            }
            else
            {
                wheelIsGrounded[i] = 0;
                
                Debug.DrawLine(rayPointsFront[i].position, rayPointsFront[i].position + (wheelRadiusFront + maxLength) * -rayPointsFront[i].up, Color.green);
            }
        }
    }

    private void SuspensionBack()
    {
        for (int i = 0; i < 2; i++)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;

            if (Physics.Raycast(rayPointsBack[i].position, -rayPointsBack[i].up, out hit, maxLength + wheelRadiusBack, drivable)) //Si está tocando el suelo
            {
                wheelIsGrounded[i+2] = 1; //El +2 es para diferenciar las ruedas traseras y así no se sobreescriban los valores en el arreglo

                float currentSpringLength = hit.distance - wheelRadiusBack;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(carRb.GetPointVelocity(rayPointsBack[i].position), rayPointsBack[i].up);
                float dampForce = damperStiffness * springVelocity; //fuerza que contrarestará el rebote innecesario de la suspension

                float springForce = springStiffness * springCompression;

                float netForce = springForce - dampForce;

                carRb.AddForceAtPosition(netForce * rayPointsBack[i].up, rayPointsBack[i].position); //Añade empuje hacia arriba al auto para simular el efecto de la suspension

                Debug.DrawLine(rayPointsBack[i].position, hit.point, Color.magenta);
            }
            else
            {
                wheelIsGrounded[i + 2] = 0;

                Debug.DrawLine(rayPointsBack[i].position, rayPointsBack[i].position + (wheelRadiusBack + maxLength) * -rayPointsBack[i].up, Color.green);
            }
        }
    }

    #endregion
}
