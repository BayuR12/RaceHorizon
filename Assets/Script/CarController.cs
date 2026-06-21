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
    public float brakeForce = 3000f;
    public float maxSteerAngle = 20f;

    [Header("Nitro Boost")]
    public float nitroForce = 7000f;

    [Header("UI")]
    public TMP_Text speedText;

    [Header("Checkpoint System")]
    public CheckpointManager checkpointManager;

    [Header("Drift Effect")]
    public ParticleSystem driftSmoke;

    private float horizontalInput;
    private float verticalInput;
    private bool isBraking;
    private bool isNitroActive;

    private float currentBrakeForce;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Membuat mobil lebih stabil (tidak mudah terbalik)
        rb.centerOfMass = new Vector3(0, -1f, 0);

        rb.linearDamping = 0.2f;
        rb.angularDamping = 1f;
    }

    void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        HandleBraking();
        UpdateWheels();
        UpdateSpeedUI();
    }

    // =========================================================
    // FUNGSI INPUT UNTUK UI BUTTON ANDROID (DIPANGGIL DI UNITY)
    // =========================================================

    public void MoveInput(float inputMove)
    {
        verticalInput = inputMove; // 1 = Maju, -1 = Mundur, 0 = Lepas Gas
    }

    public void SteerInput(float inputSteer)
    {
        horizontalInput = inputSteer; // 1 = Kanan, -1 = Kiri, 0 = Lurus
    }

    public void BrakeInput(bool statusBrake)
    {
        isBraking = statusBrake; // true = Rem ditekan, false = Dilepas
    }

    public void NitroInput(bool statusNitro)
    {
        isNitroActive = statusNitro; // true = Nitro ditekan, false = Dilepas
    }

    public void TriggerRespawn()
    {
        Respawn();
    }

    // =========================================================

    void HandleMotor()
    {
        float currentMotorTorque = 0f;

        // Memisahkan tenaga: jika verticalInput minus (-1), pakai tenaga mundur
        if (verticalInput > 0)
        {
            currentMotorTorque = verticalInput * motorForce; // Tenaga Maju
            
            if (!isNitroActive) rb.linearDamping = 0.2f; // Damping normal saat maju
        }
        else if (verticalInput < 0)
        {
            currentMotorTorque = verticalInput * reverseForce; // Tenaga Mundur
            
            // JALAN KELUARNYA DI SINI:
            rb.linearDamping = 0f; // Hambatan jadi 0 agar mobil enteng saat mundur
        }
        else
        {
            if (!isNitroActive) rb.linearDamping = 0.2f; // Damping normal saat diam
        }

        // Terapkan tenaga yang sudah disesuaikan ke roda depan
        frontLeftCollider.motorTorque = currentMotorTorque;
        frontRightCollider.motorTorque = currentMotorTorque;

        // Sistem Nitro
        if (isNitroActive)
        {
            rb.AddForce(transform.forward * nitroForce);
            rb.linearDamping = 0.02f;
        }
    }

    void HandleSteering()
    {
        float speedFactor = rb.linearVelocity.magnitude / 20f;
        float currentSteerAngle = Mathf.Lerp(maxSteerAngle, 10f, speedFactor);
        float steerAngle = currentSteerAngle * horizontalInput;

        frontLeftCollider.steerAngle = steerAngle;
        frontRightCollider.steerAngle = steerAngle;
    }

    void HandleBraking()
    {
        // PERBAIKAN DI SINI:
        // Jika menekan rem tapi kecepatan mobil sudah sangat pelan (< 0.5 KM/H), 
        // matikan kekuatan rem agar roda bisa berputar ke belakang (mundur)
        if (isBraking)
        {
            if (rb.linearVelocity.magnitude > 0.1f)
            {
                currentBrakeForce = brakeForce;
            }
            else
            {
                currentBrakeForce = 0f; // Rem dilepas otomatis saat mobil diam total
            }
        }
        else
        {
            currentBrakeForce = 0f;
        }

        ApplyBraking();

        // Drift Effect
        if (isBraking && rb.linearVelocity.magnitude > 10f)
        {
            if (!driftSmoke.isPlaying)
            {
                driftSmoke.Play();
            }

            rearLeftCollider.sidewaysFriction = DriftFriction(0.5f);
            rearRightCollider.sidewaysFriction = DriftFriction(0.5f);
        }
        else
        {
            if (driftSmoke.isPlaying)
            {
                driftSmoke.Stop();
            }

            rearLeftCollider.sidewaysFriction = DriftFriction(1f);
            rearRightCollider.sidewaysFriction = DriftFriction(1f);
        }
    }

    void ApplyBraking()
    {
        frontLeftCollider.brakeTorque = currentBrakeForce;
        frontRightCollider.brakeTorque = currentBrakeForce;
        rearLeftCollider.brakeTorque = currentBrakeForce;
        rearRightCollider.brakeTorque = currentBrakeForce;
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
        else
        {
            transform.position = Vector3.zero;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    WheelFrictionCurve DriftFriction(float stiffness)
    {
        WheelFrictionCurve friction = rearLeftCollider.sidewaysFriction;
        friction.stiffness = stiffness;
        return friction;
    }
}