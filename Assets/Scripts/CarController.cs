using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
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
    [SerializeField] ParticleSystem lSmoke;
    [SerializeField] ParticleSystem rSmoke;
    [SerializeField] TrailRenderer lskid;
    [SerializeField] TrailRenderer rskid;
    [SerializeField] AudioSource Engine;
    [SerializeField] AudioSource Brake;
    [SerializeField] AudioClip BrakeSound;
    [SerializeField] Image needle;
    [SerializeField] Rigidbody rb;

    [Header("Values")]
    [SerializeField] private float MaxTorque = 100000;
    [SerializeField] private float MaxSpeed = 2400;
    [SerializeField] private float maxBackwardsSpeed = 100;
    [SerializeField] private float normalBrakeTorque = 100000;
    [SerializeField] private float slideBrakeTorque = 15000;
    [SerializeField] private float minPitch =0.4f;
    [SerializeField] private float maxPitch = 1.1f;
    [SerializeField] private float mainForwardFriction = 10.0f;
    [SerializeField] private float mainSidewaysFriction = 10.0f;
    [SerializeField] private float driftForwardFriction = 0.60f;
    [SerializeField] private float driftSidewaysFriction = 18f;
    [SerializeField] private float downForce = 500f;

    private float currentSpeed = 0f;
    private Vector3 FLpos;
    private Quaternion FLrot;
    private Vector3 FRpos;
    private Quaternion FRrot;
    private bool isBreaking = false;
    private bool isDrifting = false;
    private bool isPlayingSound = false;
    public UIManager UIManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainForwardFriction = RRWheel.forwardFriction.stiffness;
        mainSidewaysFriction = RRWheel.sidewaysFriction.stiffness;
        //Brake = gameObject.AddComponent(typeof(AudioSource));
        Brake.clip = BrakeSound;
        Brake.loop = true;
        Brake.volume = 0f;
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * downForce);
        Speedometer(currentSpeed);
        Movement();
        Drifting();
        GearSound();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
        wheelRotation();
        wheelSteer();
    }

   

    void TogglePause()
    {
        if (Time.timeScale == 0f)
        {
            // El juego está pausado, resumirlo
            Time.timeScale = 1f;
            UIManager.ShowHUD();
            // Reanudar el audio si estaba pausado
            Engine.UnPause();
            Brake.UnPause();
        }
        else
        {
            // Pausar el juego
            Time.timeScale = 0f;
            UIManager.ShowPause();
            // Pausar el audio si está sonando
            Engine.Pause();
            Brake.Pause();
        }
    }


#region Movement

void Movement()
    {

        FLWheel.steerAngle = 15 * Input.GetAxis("Horizontal");
        FRWheel.steerAngle = 15 * Input.GetAxis("Horizontal");

        if (((currentSpeed > 0 && Input.GetAxis("Vertical") < 0)) || (currentSpeed <0 && Input.GetAxis("Vertical") > 0)){
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
            if (currentSpeed < MaxSpeed && currentSpeed>maxBackwardsSpeed*-1 )
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
        currentSpeed = Mathf.Round((Mathf.PI * 2 * FLWheel.radius) * FLWheel.rpm * 15 / 100);
    }

    void Drifting()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            BrakeEffect(isDrifting = true);
            RLWheel.brakeTorque = slideBrakeTorque;
            RRWheel.brakeTorque = slideBrakeTorque;
            if ((Mathf.Abs(rb.velocity.z) > 1) || (Mathf.Abs(rb.velocity.x) > 1))
            {
                SetFriction(driftForwardFriction, driftSidewaysFriction);
            }
        }
        else
        {
            BrakeEffect(isDrifting = false);
            RLWheel.brakeTorque = 0;
            RRWheel.brakeTorque = 0;
            SetFriction(mainForwardFriction, mainSidewaysFriction);
        }
    }

    void SetFriction(float FwdFriction, float SdwFriction)
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
    }

    #endregion


    #region Visuals and Sound

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

    private void BrakeEffect(bool play)
    {
        if (play)
        {
             WheelHit hit;
            if (RLWheel.GetGroundHit(out hit))
            {
                rSmoke.Play();
                lSmoke.Play();
                lskid.emitting = true;
                rskid.emitting = true;
                if (isPlayingSound == false)
                {
                    isPlayingSound = true;
                    Brake.volume = 1f;
                    Brake.Play();
                }
            }
        }
        else
        {
            isPlayingSound = false;
            Brake.Stop();
            lSmoke.Stop();
            rSmoke.Stop();
            lskid.emitting = false;
            rskid.emitting= false;
        }
    }

    public void Speedometer(float speed)
    {
        if (speed >=0)
        {
            float speedRatio = speed / MaxSpeed;
            float angle = (speedRatio * 280) - 140;

            needle.rectTransform.rotation = Quaternion.Euler(0f, 0f, -angle); 
        }
    }
    
    private void GearSound()
    {
        Engine.pitch= Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(currentSpeed/MaxSpeed));
    }
    #endregion
}
