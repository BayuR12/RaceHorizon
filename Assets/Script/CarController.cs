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
    public float brakeForce = 3000f;
    public float maxSteerAngle = 20f;

    [Header("Nitro Boost")]
    public float nitroForce = 7000f;
    public KeyCode nitroKey = KeyCode.LeftShift;

    [Header("UI")]
    public TMP_Text speedText;

    [Header("Checkpoint System")]
    public CheckpointManager checkpointManager;

    [Header("Drift Effect")]
    public ParticleSystem driftSmoke;

    private float horizontalInput;
    private float verticalInput;
    private bool isBraking;

    private float currentBrakeForce;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Mobil lebih stabil
        rb.centerOfMass = new Vector3(0, -1f, 0);

        rb.linearDamping = 0.2f;
        rb.angularDamping = 1f;
    }

    void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        HandleBraking();
        UpdateWheels();
        UpdateSpeedUI();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        isBraking = Input.GetKey(KeyCode.Space);

        // Tombol Respawn
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
    }

    void HandleMotor()
    {
        frontLeftCollider.motorTorque = verticalInput * motorForce;
        frontRightCollider.motorTorque = verticalInput * motorForce;

        // Nitro
        if (Input.GetKey(nitroKey))
        {
            rb.AddForce(transform.forward * nitroForce);

            rb.linearDamping = 0.05f;
        }
        else
        {
            rb.linearDamping = 0.2f;
        }
    }

    void HandleSteering()
    {
        float speedFactor =
            rb.linearVelocity.magnitude / 20f;

        float currentSteerAngle =
            Mathf.Lerp(maxSteerAngle, 10f, speedFactor);

        float steerAngle =
            currentSteerAngle * horizontalInput;

        frontLeftCollider.steerAngle = steerAngle;
        frontRightCollider.steerAngle = steerAngle;
    }

    void HandleBraking()
    {
        currentBrakeForce =
            isBraking ? brakeForce : 0f;

        ApplyBraking();

        // Drift Effect
        if (isBraking && rb.linearVelocity.magnitude > 10f)
        {
            // Mainkan asap drift
            if (!driftSmoke.isPlaying)
            {
                driftSmoke.Play();
            }

            // Membuat ban belakang licin
            rearLeftCollider.sidewaysFriction =
                DriftFriction(0.5f);

            rearRightCollider.sidewaysFriction =
                DriftFriction(0.5f);
        }
        else
        {
            // Stop asap drift
            if (driftSmoke.isPlaying)
            {
                driftSmoke.Stop();
            }

            // Ban normal kembali
            rearLeftCollider.sidewaysFriction =
                DriftFriction(1f);

            rearRightCollider.sidewaysFriction =
                DriftFriction(1f);
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
            float speed =
                rb.linearVelocity.magnitude * 3.6f;

            speedText.text =
                "Speed : "
                + Mathf.Round(speed)
                + " KM/H";
        }
    }

    void Respawn()
    {
        // Respawn ke checkpoint terakhir
        if (checkpointManager.currentRespawnPoint != null)
        {
            transform.position =
                checkpointManager.currentRespawnPoint.position
                + Vector3.up * 2f;

            transform.rotation =
                checkpointManager.currentRespawnPoint.rotation;
        }
        else
        {
            // Kalau belum checkpoint
            transform.position = Vector3.zero;
        }

        // Reset velocity
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    WheelFrictionCurve DriftFriction(float stiffness)
    {
        WheelFrictionCurve friction =
            rearLeftCollider.sidewaysFriction;

        friction.stiffness = stiffness;

        return friction;
    }
}