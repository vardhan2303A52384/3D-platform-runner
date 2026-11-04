using UnityEngine;

/// <summary>
/// Cinematic dynamic camera controller for PlatformRunner3D.
/// Features:
/// - Asymmetric vertical jump parallax (allows character to leap in frame with true elevation)
/// - Speed-dependent optical FOV expansion for authentic velocity perception
/// - Slide elevation & pitch compensation for obstacle sightlines
/// - Momentum-based lane banking and organic running head-bob
/// - Screen shake integration
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [Header("Target & Positioning")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 4.2f, -6.5f);

    [Header("Follow Dynamics")]
    public float smoothSpeed = 12f;
    public float verticalAirSmooth = 4.5f;
    public float lookAheadDistance = 6.5f;
    public float lookUpOffset = 1.1f;

    [Header("Cinematic Enhancements")]
    public float laneRollAngle = 4.0f;
    public float headBobSpeed = 14f;
    public float headBobAmount = 0.045f;
    public float baseFOV = 60f;
    public float maxSpeedFOV = 67f;

    private float currentRoll = 0f;
    private float prevTargetX = 0f;
    private Camera cam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            baseFOV = cam.fieldOfView;
            maxSpeedFOV = baseFOV + 7f;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
            else return;
        }

        PlayerController pc = PlayerController.ActivePlayer;
        bool isRunning = pc != null && pc.isRunning && !pc.IsDead;
        bool isGrounded = pc != null && pc.GetComponent<CharacterController>() != null && pc.GetComponent<CharacterController>().isGrounded;

        // 1. Asymmetric Vertical Follow Position
        // Air tracking is softer, allowing the character to physically leap higher in viewport
        float yTrackingSpeed = isGrounded ? smoothSpeed : verticalAirSmooth;
        float desiredY = target.position.y + offset.y;

        // Slide compensation: slightly lower camera height and tilt up
        if (pc != null && pc.isSliding)
        {
            desiredY -= 0.35f;
        }

        float targetX = Mathf.Lerp(transform.position.x, target.position.x + offset.x, smoothSpeed * Time.deltaTime);
        float targetY = Mathf.Lerp(transform.position.y, desiredY, yTrackingSpeed * Time.deltaTime);
        float targetZ = target.position.z + offset.z;

        // 2. Running Head-bob (active only while grounded)
        float headBob = 0f;
        if (isRunning && isGrounded)
        {
            float speedRatio = pc.forwardSpeed / 12f;
            headBob = Mathf.Sin(Time.time * headBobSpeed * speedRatio) * headBobAmount;
        }

        // 3. Screen Shake Offset
        Vector3 shakeOffset = Vector3.zero;
        if (ScreenShakeEffect.Instance != null)
        {
            shakeOffset = ScreenShakeEffect.Instance.CurrentShakeOffset;
        }

        transform.position = new Vector3(targetX, targetY + headBob, targetZ) + shakeOffset;

        // 4. Dynamic Speed FOV
        if (cam != null && pc != null)
        {
            float speedFactor = Mathf.InverseLerp(12f, 28f, pc.CurrentForwardSpeed);
            float targetFOV = Mathf.Lerp(baseFOV, maxSpeedFOV, speedFactor);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 3.5f);
        }

        // 5. LookAhead targeting
        float slidePitchBonus = (pc != null && pc.isSliding) ? 0.35f : 0f;
        Vector3 lookTarget = target.position + (Vector3.forward * lookAheadDistance) + (Vector3.up * (lookUpOffset + slidePitchBonus));
        transform.LookAt(lookTarget);

        // 6. Dynamic Banking / Roll on lane shifts
        float deltaX = target.position.x - prevTargetX;
        prevTargetX = target.position.x;
        float targetRoll = Mathf.Clamp(deltaX * -laneRollAngle * 10f, -laneRollAngle, laneRollAngle);
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.deltaTime * 9f);

        Vector3 euler = transform.eulerAngles;
        euler.z = currentRoll;
        transform.eulerAngles = euler;
    }
}
