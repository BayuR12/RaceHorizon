using UnityEngine;
using TMPro;

public class CarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    [Header("Wheel Transforms")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    [Header("Car Settings")]
    public float motorForce = 2000f;
    public float reverseForce = 1500f;
    public float brakeForce = 4000f; 
    public float maxSteerAngle = 20f;

    [Header("Nitro Boost")]
    public float nitroForce = 7000f;

    [Header("UI")]
    public TMP_Text speedText;

    [Header("Checkpoint System")]
    public CheckpointManager checkpointManager;

    [Header("Drift Effect")]
    public ParticleSystem driftSmoke;

    // =========================================================
    // UPDATE BARU: SEKARANG BISA MASUKIN 2 AUDIO SEKALIGUS
    // =========================================================
    [Header("Dual Engine Sound Settings")]
    [SerializeField] private AudioSource idleAudioSource;  // Taruh Audio Source Idle di sini
    [SerializeField] private AudioSource maxGasAudioSource; // Taruh Audio Source Ngegas di sini
    
    [SerializeField] private float minPitch = 0.8f;   
    [SerializeField] private float maxPitch = 2.0f;   
    [SerializeField] private float pitchSpeed = 4f;   

    private float horizontalInput;
    private float verticalInput;
    private bool isNitroActive;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1f, 0);
        rb.linearDamping = 0.2f;
        rb.angularDamping = 1f;

        // Memastikan kedua audio langsung berputar di awal game
        if (idleAudioSource != null && !idleAudioSource.isPlaying) idleAudioSource.Play();
        if (maxGasAudioSource != null && !maxGasAudioSource.isPlaying) maxGasAudioSource.Play();
    }

    void FixedUpdate()
    {
        HandleMotorAndBraking(); 
        HandleSteering();
        UpdateWheels();
        UpdateSpeedUI();
        UpdateDualEngineSound(); // Fungsi pencampur suara baru
    }

    public void MoveInput(float inputMove)
    {
        verticalInput = inputMove; 
    }

    public void SteerInput(float inputSteer)
    {
        horizontalInput = inputSteer; 
    }

    public void NitroInput(bool statusNitro)
    {
        isNitroActive = statusNitro;
    }

    public void TriggerRespawn()
    {
        Respawn();
    }

    void HandleMotorAndBraking()
    {
        float motorTorque = 0f;
        float brakeTorque = 0f;

        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        if (verticalInput > 0) 
        {
            if (forwardSpeed < -0.5f) brakeTorque = brakeForce;
            else motorTorque = verticalInput * motorForce;
        }
        else if (verticalInput < 0) 
        {
            if (forwardSpeed > 0.5f) 
            {
                brakeTorque = brakeForce;
                TriggerDriftSmoke(true); 
            }
            else 
            {
                motorTorque = verticalInput * reverseForce;
                rb.linearDamping = 0f; 
                TriggerDriftSmoke(false);
            }
        }
        else 
        {
            TriggerDriftSmoke(false);
        }

        frontLeftCollider.motorTorque = motorTorque;
        frontRightCollider.motorTorque = motorTorque;
        frontLeftCollider.brakeTorque = brakeTorque;
        frontRightCollider.brakeTorque = brakeTorque;
        rearLeftCollider.brakeTorque = brakeTorque;
        rearRightCollider.brakeTorque = brakeTorque;

        if (isNitroActive)
        {
            rb.AddForce(transform.forward * nitroForce);
            rb.linearDamping = 0.02f;
        }
    }

    // SIMULASI CROSSFADE & PITCH DUA AUDIO
    void UpdateDualEngineSound()
    {
        if (idleAudioSource == null || maxGasAudioSource == null || rb == null) return;

        // 1. Hitung rasio kecepatan (0 saat diam, 1 saat kencang)
        float currentSpeed = rb.linearVelocity.magnitude;
        float speedRatio = currentSpeed / 35f;
        speedRatio = Mathf.Clamp01(speedRatio);

        // 2. Tentukan target pitch dasar berdasarkan input gas dan kecepatan
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
        if (Mathf.Abs(verticalInput) > 0)
        {
            targetPitch += 0.2f; 
        }
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        // 3. Terapkan pitch yang halus ke kedua Audio Source
        idleAudioSource.pitch = Mathf.MoveTowards(idleAudioSource.pitch, targetPitch, Time.fixedDeltaTime * pitchSpeed);
        maxGasAudioSource.pitch = Mathf.MoveTowards(maxGasAudioSource.pitch, targetPitch, Time.fixedDeltaTime * pitchSpeed);

        // 4. MAININ VOLUME (Pencampuran Suara):
        // Jika mobil diam, volume idle penuh (1) dan volume gas hilang (0).
        // Jika mobil kencang, volume idle mengecil dan volume gas membesar penuh.
        idleAudioSource.volume = Mathf.Lerp(0.8f, 0.1f, speedRatio);
        maxGasAudioSource.volume = Mathf.Lerp(0.1f, 0.9f, speedRatio);
    }

    void HandleSteering()
    {
        float speedFactor = rb.linearVelocity.magnitude / 20f;
        float currentSteerAngle = Mathf.Lerp(maxSteerAngle, 10f, speedFactor);
        float steerAngle = currentSteerAngle * horizontalInput;

        frontLeftCollider.steerAngle = steerAngle;
        frontRightCollider.steerAngle = steerAngle;
    }

    void TriggerDriftSmoke(bool active)
    {
        if (active && rb.linearVelocity.magnitude > 10f)
        {
            if (!driftSmoke.isPlaying) driftSmoke.Play();
            rearLeftCollider.sidewaysFriction = DriftFriction(0.5f);
            rearRightCollider.sidewaysFriction = DriftFriction(0.5f);
        }
        else
        {
            if (driftSmoke.isPlaying) driftSmoke.Stop();
            rearLeftCollider.sidewaysFriction = DriftFriction(1f);
            rearRightCollider.sidewaysFriction = DriftFriction(1f);
        }
    }

    void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftCollider, frontLeftWheel);
        UpdateSingleWheel(frontRightCollider, frontRightWheel);
        UpdateSingleWheel(rearLeftCollider, rearLeftWheel);
        UpdateSingleWheel(rearRightCollider, rearRightWheel);
    }

    void UpdateSingleWheel(WheelCollider collider, Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }

    void UpdateSpeedUI()
    {
        if (speedText != null)
        {
            float speed = rb.linearVelocity.magnitude * 3.6f;
            speedText.text = "Speed : " + Mathf.Round(speed) + " KM/H";
        }
    }

    void Respawn()
    {
        if (checkpointManager != null && checkpointManager.currentRespawnPoint != null)
        {
            transform.position = checkpointManager.currentRespawnPoint.position + Vector3.up * 2f;
            transform.rotation = checkpointManager.currentRespawnPoint.rotation;
        }
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private WheelFrictionCurve DriftFriction(float stiffness)
    {
        WheelFrictionCurve friction = rearLeftCollider.sidewaysFriction;
        friction.stiffness = stiffness;
        return friction;
    }
}