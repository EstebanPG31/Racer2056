using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CCGPT : MonoBehaviour
{
    [Header("References")]
    [SerializeField] WheelCollider FLWheel;
    [SerializeField] WheelCollider FRWheel;
    [SerializeField] WheelCollider RLWheel;
    [SerializeField] WheelCollider RRWheel;
    [SerializeField] Transform FL;
    [SerializeField] Transform FR;
    [SerializeField] Transform RL;
    [SerializeField] Transform RR;
    [SerializeField] AudioSource Engine;

    [Header("Values")]
    [SerializeField] private float MaxTorque = 260000;
    [SerializeField] private float MaxSpeed = 3300;
    [SerializeField] private float maxBackwardsSpeed = 300;
    [SerializeField] private float normalBrakeTorque = 150000;
    //[SerializeField] private float slideBrakeTorque = 1300000;
    [SerializeField] private float minPitch = 0.32f;
    [SerializeField] private float maxPitch = 1.4f;
    /*[SerializeField] private float mainForwardFriction = 15.0f;
    [SerializeField] private float mainSidewaysFriction = 15.0f;
    [SerializeField] private float driftForwardFriction = 15.60f;
    [SerializeField] private float driftSidewaysFriction = 15.15f;*/
    [SerializeField] public float slideForce = 10000f;
    [SerializeField] public float slideDuration = 3f;


    private float currentSpeed = 0f;
    public Texture2D SpeedDisplay;
    public Texture2D SpeedPointer;
    Vector2 centro;
    private Vector3 FLpos;
    private Quaternion FLrot;
    private Vector3 FRpos;
    private Quaternion FRrot;
    private bool isBreaking = false;

    private Rigidbody rb;
    private bool isSliding = false;
    private float slideTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Movement();
        Drifting();
        GearSound();
    }

    private void Update()
    {
        wheelRotation();
        wheelSteer();
        if (currentSpeed > 100 && Input.GetAxis("Horizontal") != 0 && Input.GetKeyDown(KeyCode.Space) && !isSliding && slideTimer <= 0f)
        {
            StartSlide();
        }
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 150, 50), new GUIContent("Speed " + currentSpeed));
        GUI.Box(new Rect(1640, 700, 280, 280), SpeedDisplay);
        centro = new Vector2(1780, 840);
        if (currentSpeed >= 0)
        {
            GUIUtility.RotateAroundPivot((currentSpeed / 10) - 140, centro);
        }
        else
        {
            GUIUtility.RotateAroundPivot(-140, centro);
        }
        GUI.DrawTexture(new Rect(1640, 700, 280, 280), SpeedPointer, ScaleMode.ScaleToFit, true, 0);
    }



    #region Movement

    void Movement()
    {

        FLWheel.steerAngle = 15 * Input.GetAxis("Horizontal");
        FRWheel.steerAngle = 15 * Input.GetAxis("Horizontal");

        if (((currentSpeed > 0 && Input.GetAxis("Vertical") < 0)) || (currentSpeed < 0 && Input.GetAxis("Vertical") > 0))
        {
            isBreaking = true;
        }
        else
        {
            isBreaking = false;
            FLWheel.brakeTorque = 0;
            FRWheel.brakeTorque = 0;
        }

        if (isBreaking == false)
        {
            if (currentSpeed < MaxSpeed && currentSpeed > maxBackwardsSpeed * -1)
            {
                FLWheel.motorTorque = MaxTorque * Input.GetAxis("Vertical");
                FRWheel.motorTorque = MaxTorque * Input.GetAxis("Vertical");
            }
            else
            {
                FLWheel.motorTorque = 0;
                FRWheel.motorTorque = 0;
            }
        }
        else
        {
            FLWheel.brakeTorque = normalBrakeTorque;
            FRWheel.brakeTorque = normalBrakeTorque;
        }
        currentSpeed = Mathf.Round((Mathf.PI * 2 * FLWheel.radius) * FLWheel.rpm * 60 / 1000);
    }

    void Drifting()
    {
        float steerDirection = Input.GetAxis("Horizontal");
        if (isSliding)
        {
            rb.AddRelativeForce( Vector3.right * slideForce * Time.deltaTime, ForceMode.Acceleration);
            slideTimer -= Time.fixedDeltaTime;
        }
        if (slideTimer <= 0f)
        {
            EndSlide();
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        // Aquí podrías reproducir una animación de power slide o efecto de sonido si lo deseas
    }

    private void EndSlide()
    {
        isSliding = false;
        slideTimer = 0;
        // Aquí podrías realizar acciones adicionales cuando el power slide termine
    }

    /*void SetFriction(float FwdFriction, float SdwFriction)
    {
        WheelFrictionCurve ffriction = FLWheel.forwardFriction;
        ffriction.stiffness = FwdFriction;
        FLWheel.forwardFriction = ffriction;
        FRWheel.forwardFriction = ffriction;
        RLWheel.forwardFriction = ffriction;
        RRWheel.forwardFriction = ffriction;

        WheelFrictionCurve sfriction = FLWheel.sidewaysFriction;
        sfriction.stiffness = SdwFriction;
        FLWheel.sidewaysFriction = sfriction;
        FRWheel.sidewaysFriction = sfriction;
        RLWheel.sidewaysFriction = sfriction;
        RRWheel.sidewaysFriction = sfriction;
    }*/

    #endregion


    #region Visuals

    void wheelRotation()
    {
        FL.Rotate(FLWheel.rpm / 600 * 360 * Time.deltaTime, 0, 0);
        FR.Rotate(FRWheel.rpm / 600 * 360 * Time.deltaTime, 0, 0);
        RL.Rotate(RLWheel.rpm / 600 * 360 * Time.deltaTime, 0, 0);
        RR.Rotate(RRWheel.rpm / 600 * 360 * Time.deltaTime, 0, 0);
    }

    void wheelSteer()
    {
        FLWheel.GetWorldPose(out FLpos, out FLrot);
        FRWheel.GetWorldPose(out FRpos, out FRrot);
        FL.transform.position = FLpos;
        FL.transform.rotation = FLrot * Quaternion.Euler(0, 180, 0);
        FR.transform.position = FRpos;
        FR.transform.rotation = FRrot * Quaternion.Euler(0, 180, 0);
    }

    #endregion

    #region Sound

    private void GearSound()
    {
        Engine.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(currentSpeed / MaxSpeed));
    }


    #endregion
}
