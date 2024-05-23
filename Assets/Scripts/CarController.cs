using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Rigidbody carRb;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;

    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness; //Ayuda a que el coche no se quede rebotando
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;

    private void Start()
    {
        carRb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Suspension();
    }
    private void Suspension()
    {
        foreach (Transform rayPoint in rayPoints)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;

            if (Physics.Raycast(rayPoint.position, -rayPoint.up, out hit, maxLength + wheelRadius, drivable)) //Si está tocando el suelo
            {
                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(carRb.GetPointVelocity(rayPoint.position), rayPoint.up);
                float dampForce = damperStiffness * springVelocity; //fuerza que contrarestará el rebote innecesario de la suspension
               
                float springForce = springStiffness * springCompression;

                float netForce = springForce - dampForce;

                carRb.AddForceAtPosition(netForce * rayPoint.up, rayPoint.position); //Añade empuje hacia arriba al auto para simular el efecto de la suspension

                Debug.DrawLine(rayPoint.position, hit.point, Color.magenta);
            }
            else
            {
                Debug.DrawLine(rayPoint.position, rayPoint.position + (wheelRadius + maxLength) * -rayPoint.up, Color.green);
            }
        }
    }
}
