using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    [SerializeField] AudioSource Engine;
    [SerializeField] AudioSource Brake;
    [SerializeField] AudioClip BrakeSound;
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
    public Texture2D SpeedDisplay;
    public Texture2D SpeedPointer;
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
        Movement();
        Drifting();
        GearSound();
    }

    private void Update()
    {
        rb.AddForce(Vector3.down * downForce);
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
        wheelRotation();
        wheelSteer();
    }

    /*private void OnGUI()
    {
        GUI.Box(new Rect(0,0,150,50), new GUIContent("Speed " + currentSpeed));
        GUI.Box(new Rect(1640, 700, 280, 280), SpeedDisplay);
        centro = new Vector2(1780, 840);
        if (currentSpeed >= 0)
        {
            GUIUtility.RotateAroundPivot((currentSpeed / 10) - 140, centro); 
        }
        else {
            GUIUtility.RotateAroundPivot( - 140, centro);
        }
        GUI.DrawTexture(new Rect(1640,700,280,280), SpeedPointer, ScaleMode.ScaleToFit,true,0);
    }*/

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
        currentSpeed = Mathf.Round((Mathf.PI * 2 * FLWheel.radius) * FLWheel.rpm * 60 / 1000);
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

    private void BrakeEffect(bool play)
    {
        if (play)
        {
            if (isPlayingSound == false)
            {
                isPlayingSound = true;
                Brake.volume = 1f;
                Brake.Play();
                lSmoke.Play();
                rSmoke.Play();
            }

        }
        else
        {
            isPlayingSound = false;
            Brake.Stop();
            lSmoke.Stop();
            rSmoke.Stop();
        }
    }

    #endregion

    #region Sound

    private void GearSound()
    {
        Engine.pitch= Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(currentSpeed/MaxSpeed));
    }
    
  
    #endregion
}
