using UnityEngine;

/// <summary>
/// Master Biomechanical Skeletal Animation Engine for PlatformRunner3D.
/// Features studio-grade procedural animation kinematics:
/// - True human gait cycle with stance-phase compression, push-off, swing knee drive, and ankle toe-roll
/// - Anti-foot-sliding velocity-to-stride synchronization
/// - Multi-phase jump kinematics (Takeoff, Apex weightlessness, Fall anticipation)
/// - Physically accurate landing impact absorption with damped spring knee squash and recovery
/// - Aerodynamic baseball-style slide with ground skimming
/// - Living, breathing idle with diaphragmatic respiration, weight shifting, and head/eye micro-gaze
/// - Contralateral arm pumping with dynamic elbow flexion and torso counter-rotation
/// - Momentum-based lane banking and vestibulo-ocular head stabilization
/// - 100% synchronized footstep audio and particle emission at the physical contact frame
/// </summary>
public class BoyCharacterAnimator : MonoBehaviour
{
    [Header("Main Skeleton Transforms")]
    public Transform bodyRoot;
    public Transform head;
    public Transform torso;
    public Transform leftArm;
    public Transform rightArm;
    public Transform leftForearm;
    public Transform rightForearm;
    public Transform leftHand;
    public Transform rightHand;
    public Transform leftLeg;
    public Transform rightLeg;
    public Transform leftLowerLeg;
    public Transform rightLowerLeg;
    public Transform leftShoe;
    public Transform rightShoe;
    public Transform leftPupil;
    public Transform rightPupil;

    [Header("Secondary Physics Transforms")]
    public Transform hairFringe;
    public Transform hood;
    public Transform backpack;

    [Header("Kinematic Tuning")]
    [Tooltip("Base stride cadence frequency")]
    public float runCycleSpeed = 16.5f;
    [Tooltip("Base thigh swing angle in degrees")]
    public float legSwingAngle = 46f;
    [Tooltip("Base shoulder arm swing angle in degrees")]
    public float armSwingAngle = 40f;
    [Tooltip("Base vertical running bounce height in meters")]
    public float bodyBounceHeight = 0.095f;
    [Tooltip("Physical base stride length in meters")]
    public float baseStrideLength = 2.25f;

    [Header("References")]
    public PlayerController playerController;
    public CharacterController characterController;

    // Initial Rest Poses
    private Vector3 initBodyPos;
    private Quaternion initBodyRot;
    private Quaternion initHeadRot;
    private Quaternion initTorsoRot;
    private Quaternion initLeftArmRot;
    private Quaternion initRightArmRot;
    private Quaternion initLeftForearmRot;
    private Quaternion initRightForearmRot;
    private Quaternion initLeftHandRot;
    private Quaternion initRightHandRot;
    private Quaternion initLeftLegRot;
    private Quaternion initRightLegRot;
    private Quaternion initLeftLowerLegRot;
    private Quaternion initRightLowerLegRot;
    private Quaternion initLeftShoeRot;
    private Quaternion initRightShoeRot;
    private Vector3 initLeftPupilPos;
    private Vector3 initRightPupilPos;
    private Quaternion initFringeRot;
    private Quaternion initHoodRot;
    private Quaternion initBackpackRot;
    private float jumpAnticipationTimer = 0f;

    // Running Gait State
    private float gaitPhase = 0f;
    private bool leftFootStruck = false;
    private bool rightFootStruck = false;

    // Lateral Banking & Roll Dynamics
    private float currentBankingAngle = 0f;
    private float currentLateralOffset = 0f;
    private float lateralVelocitySmooth = 0f;

    // Landing Impact & Spring Dynamics
    private bool wasGrounded = true;
    private float preLandingVerticalVelocity = 0f;
    private float landingSquash = 0f;
    private float landingSquashVelocity = 0f;

    // Jump State Tracking
    private float airTime = 0f;

    // Slide State Smoothing
    private float slideBlendWeight = 0f;
    private float slideDustTimer = 0f;

    // Idle Dynamics State
    private float idleTimer = 0f;
    private float blinkTimer = 0f;
    private float eyeShiftTimer = 0f;
    private Vector2 currentEyeOffset = Vector2.zero;

    // State Blending Weights
    private float weightIdle = 1f;
    private float weightRun = 0f;
    private float weightJump = 0f;
    private float weightFall = 0f;
    private float weightSlide = 0f;
    private float weightDeath = 0f;

    private void Awake()
    {
        AutoDiscoverSkeletonBinds();
    }

    private void Start()
    {
        AutoDiscoverSkeletonBinds();
        RecordInitialRestPoses();

        if (playerController == null)
            playerController = GetComponentInParent<PlayerController>();

        if (characterController == null)
            characterController = GetComponentInParent<CharacterController>();

        wasGrounded = characterController != null && characterController.isGrounded;
    }

    /// <summary>
    /// Recursively discovers and binds any joints not explicitly assigned in the inspector.
    /// </summary>
    public void AutoDiscoverSkeletonBinds()
    {
        if (bodyRoot == null)
        {
            Transform found = transform.Find("BodyRoot");
            if (found != null) bodyRoot = found;
        }

        if (bodyRoot != null)
        {
            if (head == null)
            {
                Transform found = bodyRoot.Find("Head");
                if (found != null) head = found;
            }
            if (torso == null)
            {
                Transform found = bodyRoot.Find("Torso");
                if (found != null) torso = found;
            }
            if (leftArm == null)
            {
                Transform found = bodyRoot.Find("LeftArmPivot");
                if (found != null) leftArm = found;
            }
            if (rightArm == null)
            {
                Transform found = bodyRoot.Find("RightArmPivot");
                if (found != null) rightArm = found;
            }
        }

        // Elbows & Forearms
        if (leftArm != null && leftForearm == null)
        {
            Transform found = FindRecursive(leftArm, "ForearmPivot");
            if (found != null) leftForearm = found;
        }
        if (rightArm != null && rightForearm == null)
        {
            Transform found = FindRecursive(rightArm, "ForearmPivot");
            if (found != null) rightForearm = found;
        }

        // Hands
        if (leftForearm != null && leftHand == null)
        {
            Transform found = FindRecursive(leftForearm, "Hand");
            if (found != null) leftHand = found;
        }
        if (rightForearm != null && rightHand == null)
        {
            Transform found = FindRecursive(rightForearm, "Hand");
            if (found != null) rightHand = found;
        }

        // Hips & Legs
        if (leftLeg == null)
        {
            Transform found = transform.Find("LeftLegPivot");
            if (found != null) leftLeg = found;
        }
        if (rightLeg == null)
        {
            Transform found = transform.Find("RightLegPivot");
            if (found != null) rightLeg = found;
        }

        // Knees & Lower Legs
        if (leftLeg != null && leftLowerLeg == null)
        {
            Transform found = FindRecursive(leftLeg, "LeftLowerLeg");
            if (found != null) leftLowerLeg = found;
        }
        if (rightLeg != null && rightLowerLeg == null)
        {
            Transform found = FindRecursive(rightLeg, "RightLowerLeg");
            if (found != null) rightLowerLeg = found;
        }

        // Shoes & Feet
        if (leftLowerLeg != null && leftShoe == null)
        {
            Transform found = FindRecursive(leftLowerLeg, "Shoe");
            if (found != null) leftShoe = found;
        }
        if (rightLowerLeg != null && rightShoe == null)
        {
            Transform found = FindRecursive(rightLowerLeg, "Shoe");
            if (found != null) rightShoe = found;
        }

        // Eye Pupils for gaze
        if (head != null)
        {
            if (leftPupil == null) leftPupil = FindRecursive(head, "LeftPupil");
            if (rightPupil == null) rightPupil = FindRecursive(head, "RightPupil");
            if (hairFringe == null) hairFringe = FindRecursive(head, "HairFringe");
        }

        // Secondary Physics objects
        if (bodyRoot != null && hood == null) hood = FindRecursive(bodyRoot, "Hood");
        if (torso != null && backpack == null) backpack = FindRecursive(torso, "Backpack");
    }

    private Transform FindRecursive(Transform root, string targetName)
    {
        if (root == null) return null;
        if (root.name.Equals(targetName, System.StringComparison.OrdinalIgnoreCase)) return root;

        for (int i = 0; i < root.childCount; i++)
        {
            Transform res = FindRecursive(root.GetChild(i), targetName);
            if (res != null) return res;
        }
        return null;
    }

    private void RecordInitialRestPoses()
    {
        if (bodyRoot != null)
        {
            initBodyPos = bodyRoot.localPosition;
            initBodyRot = bodyRoot.localRotation;
        }
        if (head != null) initHeadRot = head.localRotation;
        if (torso != null) initTorsoRot = torso.localRotation;

        if (leftArm != null) initLeftArmRot = leftArm.localRotation;
        if (rightArm != null) initRightArmRot = rightArm.localRotation;
        if (leftForearm != null) initLeftForearmRot = leftForearm.localRotation;
        if (rightForearm != null) initRightForearmRot = rightForearm.localRotation;
        if (leftHand != null) initLeftHandRot = leftHand.localRotation;
        if (rightHand != null) initRightHandRot = rightHand.localRotation;

        if (leftLeg != null) initLeftLegRot = leftLeg.localRotation;
        if (rightLeg != null) initRightLegRot = rightLeg.localRotation;
        if (leftLowerLeg != null) initLeftLowerLegRot = leftLowerLeg.localRotation;
        if (rightLowerLeg != null) initRightLowerLegRot = rightLowerLeg.localRotation;
        if (leftShoe != null) initLeftShoeRot = leftShoe.localRotation;
        if (rightShoe != null) initRightShoeRot = rightShoe.localRotation;

        if (leftPupil != null) initLeftPupilPos = leftPupil.localPosition;
        if (rightPupil != null) initRightPupilPos = rightPupil.localPosition;
        if (hairFringe != null) initFringeRot = hairFringe.localRotation;
        if (hood != null) initHoodRot = hood.localRotation;
        if (backpack != null) initBackpackRot = backpack.localRotation;
    }

    private void Update()
    {
        if (playerController == null) return;

        UpdateGroundedAndLandingDynamics();
        UpdateStateBlendWeights();

        if (playerController.IsDead)
        {
            ApplyDeathKinematics();
        }
        else
        {
            EvaluateProceduralLocomotion();
            UpdateHeadEyeGazeTracking();
        }
    }

    // =========================================================================
    // LANDING & PHYSICAL CONTACT DETECTION
    // =========================================================================

    public void OnJumpInitiated()
    {
        jumpAnticipationTimer = 0.085f;
    }

    private void UpdateGroundedAndLandingDynamics()
    {
        bool isGrounded = characterController != null && characterController.isGrounded;
        float vY = playerController.CurrentVerticalVelocity;

        if (!wasGrounded && isGrounded)
        {
            // Landing Impact Event!
            float impactSpeed = Mathf.Abs(preLandingVerticalVelocity);
            float impactNormalized = Mathf.Clamp(impactSpeed / 24f, 0.15f, 0.42f);

            // Proportional squat compression
            landingSquash = impactNormalized;
            landingSquashVelocity = -impactNormalized * 8f;

            if (impactSpeed > 4f && CollectionVFX.Instance != null)
            {
                CollectionVFX.Instance.PlayLandingDust(transform.position, impactSpeed);
            }
        }

        if (!isGrounded)
        {
            preLandingVerticalVelocity = vY;
            airTime += Time.deltaTime;
        }
        else
        {
            airTime = 0f;
        }

        wasGrounded = isGrounded;

        // Damped spring harmonic oscillator for landing recovery
        // F = -k*x - c*v
        float springK = 130f;
        float dampingC = 18f;
        float springForce = -springK * landingSquash - dampingC * landingSquashVelocity;
        landingSquashVelocity += springForce * Time.deltaTime;
        landingSquash += landingSquashVelocity * Time.deltaTime;

        if (Mathf.Abs(landingSquash) < 0.002f && Mathf.Abs(landingSquashVelocity) < 0.01f)
        {
            landingSquash = 0f;
            landingSquashVelocity = 0f;
        }
    }

    // =========================================================================
    // STATE BLENDING ENGINE
    // =========================================================================

    private void UpdateStateBlendWeights()
    {
        bool isDead = playerController.IsDead;
        bool isRunning = playerController.isRunning;
        bool isSliding = playerController.isSliding;
        bool isGrounded = characterController != null && characterController.isGrounded;
        float vY = playerController.CurrentVerticalVelocity;

        float targetIdle = 0f;
        float targetRun = 0f;
        float targetJump = 0f;
        float targetFall = 0f;
        float targetSlide = 0f;
        float targetDeath = 0f;

        if (isDead)
        {
            targetDeath = 1f;
        }
        else if (!isRunning)
        {
            targetIdle = 1f;
        }
        else if (isSliding)
        {
            targetSlide = 1f;
        }
        else if (!isGrounded)
        {
            if (vY >= -2f) targetJump = 1f;
            else targetFall = 1f;
        }
        else
        {
            targetRun = 1f;
        }

        float blendRate = Time.deltaTime * 14f;
        weightIdle = Mathf.MoveTowards(weightIdle, targetIdle, blendRate);
        weightRun = Mathf.MoveTowards(weightRun, targetRun, blendRate);
        weightJump = Mathf.MoveTowards(weightJump, targetJump, blendRate);
        weightFall = Mathf.MoveTowards(weightFall, targetFall, blendRate);
        weightSlide = Mathf.MoveTowards(weightSlide, targetSlide, blendRate);
        weightDeath = Mathf.MoveTowards(weightDeath, targetDeath, blendRate);
    }

    // =========================================================================
    // KINEMATIC EVALUATION & BLENDING
    // =========================================================================

    private void EvaluateProceduralLocomotion()
    {
        // Compute speed-matched cadence and anti-foot-sliding stride
        float speed = playerController.CurrentForwardSpeed;
        float speedRatio = Mathf.Max(0.5f, speed / 12f);

        // In human sprinting, stride length expands with speed:
        float dynamicStride = baseStrideLength * Mathf.Pow(speedRatio, 0.48f);
        // Frequency in radians per second to ensure zero foot-sliding against track:
        float omega = (Mathf.PI * speed) / dynamicStride;

        if (weightRun > 0.001f)
        {
            gaitPhase += Time.deltaTime * omega;
            if (gaitPhase >= Mathf.PI * 2f)
            {
                gaitPhase -= Mathf.PI * 2f;
                leftFootStruck = false;
                rightFootStruck = false;
            }
        }

        // Accumulate Transforms for each limb
        Vector3 bodyPos = initBodyPos;
        Quaternion bodyRot = initBodyRot;
        Quaternion headRot = initHeadRot;
        Quaternion torsoRot = initTorsoRot;

        Quaternion leftArmRot = initLeftArmRot;
        Quaternion rightArmRot = initRightArmRot;
        Quaternion leftForearmRot = initLeftForearmRot;
        Quaternion rightForearmRot = initRightForearmRot;

        Quaternion leftLegRot = initLeftLegRot;
        Quaternion rightLegRot = initRightLegRot;
        Quaternion leftLowerLegRot = initLeftLowerLegRot;
        Quaternion rightLowerLegRot = initRightLowerLegRot;
        Quaternion leftShoeRot = initLeftShoeRot;
        Quaternion rightShoeRot = initRightShoeRot;

        // 1. Calculate RUN Pose
        if (weightRun > 0.001f)
        {
            EvaluateRunPose(gaitPhase, speed, speedRatio,
                out Vector3 rBodyPos, out Quaternion rBodyRot, out Quaternion rHeadRot, out Quaternion rTorsoRot,
                out Quaternion rLArm, out Quaternion rRArm, out Quaternion rLForearm, out Quaternion rRForearm,
                out Quaternion rLLeg, out Quaternion rRLeg, out Quaternion rLLowerLeg, out Quaternion rRLowerLeg,
                out Quaternion rLShoe, out Quaternion rRShoe);

            bodyPos = Vector3.Lerp(bodyPos, rBodyPos, weightRun);
            bodyRot = Quaternion.Slerp(bodyRot, rBodyRot, weightRun);
            headRot = Quaternion.Slerp(headRot, rHeadRot, weightRun);
            torsoRot = Quaternion.Slerp(torsoRot, rTorsoRot, weightRun);
            leftArmRot = Quaternion.Slerp(leftArmRot, rLArm, weightRun);
            rightArmRot = Quaternion.Slerp(rightArmRot, rRArm, weightRun);
            leftForearmRot = Quaternion.Slerp(leftForearmRot, rLForearm, weightRun);
            rightForearmRot = Quaternion.Slerp(rightForearmRot, rRForearm, weightRun);
            leftLegRot = Quaternion.Slerp(leftLegRot, rLLeg, weightRun);
            rightLegRot = Quaternion.Slerp(rightLegRot, rRLeg, weightRun);
            leftLowerLegRot = Quaternion.Slerp(leftLowerLegRot, rLLowerLeg, weightRun);
            rightLowerLegRot = Quaternion.Slerp(rightLowerLegRot, rRLowerLeg, weightRun);
            leftShoeRot = Quaternion.Slerp(leftShoeRot, rLShoe, weightRun);
            rightShoeRot = Quaternion.Slerp(rightShoeRot, rRShoe, weightRun);
        }

        // 2. Calculate JUMP ASCENT & APEX Pose
        if (weightJump > 0.001f)
        {
            EvaluateJumpPose(playerController.CurrentVerticalVelocity,
                out Vector3 jBodyPos, out Quaternion jBodyRot, out Quaternion jHeadRot,
                out Quaternion jLArm, out Quaternion jRArm, out Quaternion jLForearm, out Quaternion jRForearm,
                out Quaternion jLLeg, out Quaternion jRLeg, out Quaternion jLLowerLeg, out Quaternion jRLowerLeg,
                out Quaternion jLShoe, out Quaternion jRShoe);

            bodyPos = Vector3.Lerp(bodyPos, jBodyPos, weightJump);
            bodyRot = Quaternion.Slerp(bodyRot, jBodyRot, weightJump);
            headRot = Quaternion.Slerp(headRot, jHeadRot, weightJump);
            leftArmRot = Quaternion.Slerp(leftArmRot, jLArm, weightJump);
            rightArmRot = Quaternion.Slerp(rightArmRot, jRArm, weightJump);
            leftForearmRot = Quaternion.Slerp(leftForearmRot, jLForearm, weightJump);
            rightForearmRot = Quaternion.Slerp(rightForearmRot, jRForearm, weightJump);
            leftLegRot = Quaternion.Slerp(leftLegRot, jLLeg, weightJump);
            rightLegRot = Quaternion.Slerp(rightLegRot, jRLeg, weightJump);
            leftLowerLegRot = Quaternion.Slerp(leftLowerLegRot, jLLowerLeg, weightJump);
            rightLowerLegRot = Quaternion.Slerp(rightLowerLegRot, jRLowerLeg, weightJump);
            leftShoeRot = Quaternion.Slerp(leftShoeRot, jLShoe, weightJump);
            rightShoeRot = Quaternion.Slerp(rightShoeRot, jRShoe, weightJump);
        }

        // 3. Calculate FALL DESCENT Pose
        if (weightFall > 0.001f)
        {
            EvaluateFallPose(playerController.CurrentVerticalVelocity,
                out Vector3 fBodyPos, out Quaternion fBodyRot, out Quaternion fHeadRot,
                out Quaternion fLArm, out Quaternion fRArm, out Quaternion fLForearm, out Quaternion fRForearm,
                out Quaternion fLLeg, out Quaternion fRLeg, out Quaternion fLLowerLeg, out Quaternion fRLowerLeg,
                out Quaternion fLShoe, out Quaternion fRShoe);

            bodyPos = Vector3.Lerp(bodyPos, fBodyPos, weightFall);
            bodyRot = Quaternion.Slerp(bodyRot, fBodyRot, weightFall);
            headRot = Quaternion.Slerp(headRot, fHeadRot, weightFall);
            leftArmRot = Quaternion.Slerp(leftArmRot, fLArm, weightFall);
            rightArmRot = Quaternion.Slerp(rightArmRot, fRArm, weightFall);
            leftForearmRot = Quaternion.Slerp(leftForearmRot, fLForearm, weightFall);
            rightForearmRot = Quaternion.Slerp(rightForearmRot, fRForearm, weightFall);
            leftLegRot = Quaternion.Slerp(leftLegRot, fLLeg, weightFall);
            rightLegRot = Quaternion.Slerp(rightLegRot, fRLeg, weightFall);
            leftLowerLegRot = Quaternion.Slerp(leftLowerLegRot, fLLowerLeg, weightFall);
            rightLowerLegRot = Quaternion.Slerp(rightLowerLegRot, fRLowerLeg, weightFall);
            leftShoeRot = Quaternion.Slerp(leftShoeRot, fLShoe, weightFall);
            rightShoeRot = Quaternion.Slerp(rightShoeRot, fRShoe, weightFall);
        }

        // 4. Calculate SLIDE Pose
        if (weightSlide > 0.001f)
        {
            EvaluateSlidePose(
                out Vector3 sBodyPos, out Quaternion sBodyRot, out Quaternion sHeadRot,
                out Quaternion sLArm, out Quaternion sRArm, out Quaternion sLForearm, out Quaternion sRForearm,
                out Quaternion sLLeg, out Quaternion sRLeg, out Quaternion sLLowerLeg, out Quaternion sRLowerLeg,
                out Quaternion sLShoe, out Quaternion sRShoe);

            bodyPos = Vector3.Lerp(bodyPos, sBodyPos, weightSlide);
            bodyRot = Quaternion.Slerp(bodyRot, sBodyRot, weightSlide);
            headRot = Quaternion.Slerp(headRot, sHeadRot, weightSlide);
            leftArmRot = Quaternion.Slerp(leftArmRot, sLArm, weightSlide);
            rightArmRot = Quaternion.Slerp(rightArmRot, sRArm, weightSlide);
            leftForearmRot = Quaternion.Slerp(leftForearmRot, sLForearm, weightSlide);
            rightForearmRot = Quaternion.Slerp(rightForearmRot, sRForearm, weightSlide);
            leftLegRot = Quaternion.Slerp(leftLegRot, sLLeg, weightSlide);
            rightLegRot = Quaternion.Slerp(rightLegRot, sRLeg, weightSlide);
            leftLowerLegRot = Quaternion.Slerp(leftLowerLegRot, sLLowerLeg, weightSlide);
            rightLowerLegRot = Quaternion.Slerp(rightLowerLegRot, sRLowerLeg, weightSlide);
            leftShoeRot = Quaternion.Slerp(leftShoeRot, sLShoe, weightSlide);
            rightShoeRot = Quaternion.Slerp(rightShoeRot, sRShoe, weightSlide);

            // Emit slide friction dust trail
            slideDustTimer += Time.deltaTime;
            if (slideDustTimer >= 0.06f && CollectionVFX.Instance != null && leftShoe != null)
            {
                slideDustTimer = 0f;
                CollectionVFX.Instance.PlaySlideDust(leftShoe.position);
            }
        }

        // 5. Calculate IDLE Pose
        if (weightIdle > 0.001f)
        {
            EvaluateIdlePose(
                out Vector3 iBodyPos, out Quaternion iBodyRot, out Quaternion iHeadRot,
                out Quaternion iLArm, out Quaternion iRArm, out Quaternion iLForearm, out Quaternion iRForearm,
                out Quaternion iLLeg, out Quaternion iRLeg, out Quaternion iLLowerLeg, out Quaternion iRLowerLeg,
                out Quaternion iLShoe, out Quaternion iRShoe);

            bodyPos = Vector3.Lerp(bodyPos, iBodyPos, weightIdle);
            bodyRot = Quaternion.Slerp(bodyRot, iBodyRot, weightIdle);
            headRot = Quaternion.Slerp(headRot, iHeadRot, weightIdle);
            leftArmRot = Quaternion.Slerp(leftArmRot, iLArm, weightIdle);
            rightArmRot = Quaternion.Slerp(rightArmRot, iRArm, weightIdle);
            leftForearmRot = Quaternion.Slerp(leftForearmRot, iLForearm, weightIdle);
            rightForearmRot = Quaternion.Slerp(rightForearmRot, iRForearm, weightIdle);
            leftLegRot = Quaternion.Slerp(leftLegRot, iLLeg, weightIdle);
            rightLegRot = Quaternion.Slerp(rightLegRot, iRLeg, weightIdle);
            leftLowerLegRot = Quaternion.Slerp(leftLowerLegRot, iLLowerLeg, weightIdle);
            rightLowerLegRot = Quaternion.Slerp(rightLowerLegRot, iRLowerLeg, weightIdle);
            leftShoeRot = Quaternion.Slerp(leftShoeRot, iLShoe, weightIdle);
            rightShoeRot = Quaternion.Slerp(rightShoeRot, iRShoe, weightIdle);
        }

        // 6. Apply Landing Impact Compression (Squash & Knee Flex)
        if (landingSquash > 0.001f)
        {
            // Lower body drops down
            bodyPos.y -= landingSquash * 0.42f;

            // Knees flex to absorb downward momentum
            float kneeAbsorption = landingSquash * 55f;
            leftLowerLegRot = leftLowerLegRot * Quaternion.Euler(kneeAbsorption, 0f, 0f);
            rightLowerLegRot = rightLowerLegRot * Quaternion.Euler(kneeAbsorption, 0f, 0f);

            // Torso flexes forward slightly
            bodyRot = bodyRot * Quaternion.Euler(landingSquash * 22f, 0f, 0f);

            // Arms sweep backward for balance
            float armStabilize = landingSquash * 35f;
            leftArmRot = leftArmRot * Quaternion.Euler(armStabilize, 0f, -armStabilize * 0.4f);
            rightArmRot = rightArmRot * Quaternion.Euler(armStabilize, 0f, armStabilize * 0.4f);
        }

        // 7. Jump Anticipation (Pre-takeoff athletic squat)
        if (jumpAnticipationTimer > 0f)
        {
            jumpAnticipationTimer -= Time.deltaTime;
            float squatFactor = Mathf.Clamp01(jumpAnticipationTimer / 0.085f);
            bodyPos.y -= squatFactor * 0.11f;
            leftLowerLegRot = leftLowerLegRot * Quaternion.Euler(squatFactor * 28f, 0f, 0f);
            rightLowerLegRot = rightLowerLegRot * Quaternion.Euler(squatFactor * 28f, 0f, 0f);
            leftArmRot = leftArmRot * Quaternion.Euler(squatFactor * 20f, 0f, 0f);
            rightArmRot = rightArmRot * Quaternion.Euler(squatFactor * 20f, 0f, 0f);
        }

        // 8. Secondary Inertial Follow-Through Physics (Hair, Backpack, Hoodie)
        if (weightRun > 0.1f)
        {
            if (hairFringe != null)
            {
                float fringeFlutter = Mathf.Sin(gaitPhase * 2f - 0.7f) * 6.5f;
                hairFringe.localRotation = initFringeRot * Quaternion.Euler(fringeFlutter, 0f, 0f);
            }
            if (backpack != null)
            {
                float packBob = Mathf.Sin(gaitPhase * 2f - 0.45f) * 4.5f;
                backpack.localRotation = initBackpackRot * Quaternion.Euler(packBob, 0f, 0f);
            }
            if (hood != null)
            {
                float hoodFlutter = Mathf.Sin(gaitPhase * 2f - 0.6f) * 5.0f;
                hood.localRotation = initHoodRot * Quaternion.Euler(hoodFlutter, 0f, 0f);
            }
        }
        else if (weightIdle > 0.1f)
        {
            if (hairFringe != null) hairFringe.localRotation = Quaternion.Slerp(hairFringe.localRotation, initFringeRot, Time.deltaTime * 6f);
            if (backpack != null) backpack.localRotation = Quaternion.Slerp(backpack.localRotation, initBackpackRot, Time.deltaTime * 6f);
            if (hood != null) hood.localRotation = Quaternion.Slerp(hood.localRotation, initHoodRot, Time.deltaTime * 6f);
        }

        // Apply to Bone Transforms
        if (bodyRoot != null)
        {
            bodyRoot.localPosition = bodyPos;
            bodyRoot.localRotation = bodyRot;
        }
        if (head != null) head.localRotation = headRot;
        if (torso != null) torso.localRotation = torsoRot;

        if (leftArm != null) leftArm.localRotation = leftArmRot;
        if (rightArm != null) rightArm.localRotation = rightArmRot;
        if (leftForearm != null) leftForearm.localRotation = leftForearmRot;
        if (rightForearm != null) rightForearm.localRotation = rightForearmRot;

        if (leftLeg != null) leftLeg.localRotation = leftLegRot;
        if (rightLeg != null) rightLeg.localRotation = rightLegRot;
        if (leftLowerLeg != null) leftLowerLeg.localRotation = leftLowerLegRot;
        if (rightLowerLeg != null) rightLowerLeg.localRotation = rightLowerLegRot;
        if (leftShoe != null) leftShoe.localRotation = leftShoeRot;
        if (rightShoe != null) rightShoe.localRotation = rightShoeRot;
    }

    // =========================================================================
    // 1. RUNNING GAIT POSE EVALUATION
    // =========================================================================

    private void EvaluateRunPose(float phase, float speed, float speedRatio,
        out Vector3 bPos, out Quaternion bRot, out Quaternion hRot, out Quaternion tRot,
        out Quaternion lArm, out Quaternion rArm, out Quaternion lForearm, out Quaternion rForearm,
        out Quaternion lLeg, out Quaternion rLeg, out Quaternion lLowerLeg, out Quaternion rLowerLeg,
        out Quaternion lShoe, out Quaternion rShoe)
    {
        // Phased gait angles (Left = phase, Right = phase + PI)
        float phaseL = phase;
        float phaseR = (phase + Mathf.PI) % (Mathf.PI * 2f);

        // Stride amplitude scales with sprint intensity
        float currentLegSwing = Mathf.Lerp(legSwingAngle, legSwingAngle * 1.25f, Mathf.InverseLerp(12f, 26f, speed));
        float currentArmSwing = Mathf.Lerp(armSwingAngle, armSwingAngle * 1.30f, Mathf.InverseLerp(12f, 26f, speed));

        // Evaluate Biomechanical Leg Kinematics
        EvaluateLimbGait(phaseL, currentLegSwing, out float hipL, out float kneeL, out float ankleL, out bool footStrikeL);
        EvaluateLimbGait(phaseR, currentLegSwing, out float hipR, out float kneeR, out float ankleR, out bool footStrikeR);

        // Trigger synchronized footstrike events at the exact physical contact frame
        if (footStrikeL && !leftFootStruck)
        {
            leftFootStruck = true;
            if (playerController != null)
            {
                Vector3 footPos = leftShoe != null ? leftShoe.position : transform.position;
                playerController.OnFootstrike(footPos, true);
            }
        }
        else if (!footStrikeL)
        {
            leftFootStruck = false;
        }

        if (footStrikeR && !rightFootStruck)
        {
            rightFootStruck = true;
            if (playerController != null)
            {
                Vector3 footPos = rightShoe != null ? rightShoe.position : transform.position;
                playerController.OnFootstrike(footPos, false);
            }
        }
        else if (!footStrikeR)
        {
            rightFootStruck = false;
        }

        // Apply Thigh, Knee, and Ankle Angles
        lLeg = initLeftLegRot * Quaternion.Euler(hipL, 0f, 0f);
        rLeg = initRightLegRot * Quaternion.Euler(hipR, 0f, 0f);

        lLowerLeg = Quaternion.Euler(kneeL, 0f, 0f);
        rLowerLeg = Quaternion.Euler(kneeR, 0f, 0f);

        lShoe = Quaternion.Euler(ankleL, 0f, 0f);
        rShoe = Quaternion.Euler(ankleR, 0f, 0f);

        // Contralateral Arm Pumping Kinematics
        // Left arm pumps forward with Right leg
        float armAngleL = -Mathf.Sin(phaseL) * currentArmSwing;
        float armAngleR = Mathf.Sin(phaseL) * currentArmSwing;

        // Dynamic elbow flexion: as arm drives forward, elbow flexes up to 92 degrees; opens on backstroke
        float elbowFlexL = Mathf.Lerp(30f, 92f, Mathf.InverseLerp(-currentArmSwing, currentArmSwing, armAngleL));
        float elbowFlexR = Mathf.Lerp(30f, 92f, Mathf.InverseLerp(-currentArmSwing, currentArmSwing, armAngleR));

        // Medial inward swing arc toward sternum
        float inwardL = Mathf.Clamp(armAngleL * 0.15f, 0f, 8f);
        float inwardR = Mathf.Clamp(armAngleR * 0.15f, 0f, 8f);

        lArm = initLeftArmRot * Quaternion.Euler(armAngleL - 8f, inwardL, -10f);
        rArm = initRightArmRot * Quaternion.Euler(armAngleR - 8f, -inwardR, 10f);

        lForearm = Quaternion.Euler(-elbowFlexL, 0f, 0f);
        rForearm = Quaternion.Euler(-elbowFlexR, 0f, 0f);

        // Body & Spine Dynamics
        // Double-frequency vertical bounce (2 steps per gait cycle) with gravity parabolic arc
        float bounceSine = Mathf.Sin(phase * 2f);
        float bounceHeight = Mathf.Abs(bounceSine) * bodyBounceHeight;

        // Forward sprint lean proportional to forward velocity
        float forwardLean = Mathf.Lerp(8.5f, 17.5f, Mathf.InverseLerp(12f, 28f, speed));

        // Pelvic counter-twist against shoulder swing
        float pelvicTwist = Mathf.Sin(phase) * 5.2f;

        // Lateral COM sway toward supporting leg (1x frequency)
        float lateralSway = Mathf.Cos(phase) * 0.024f;

        // Lane change banking roll and lateral momentum tilt
        float lateralVel = playerController.CurrentLateralVelocity;
        float targetRoll = Mathf.Clamp(lateralVel * -1.8f, -14f, 14f);
        currentBankingAngle = Mathf.Lerp(currentBankingAngle, targetRoll, Time.deltaTime * 12f);

        bPos = initBodyPos + new Vector3(lateralSway, bounceHeight, 0f);
        bRot = initBodyRot * Quaternion.Euler(forwardLean, pelvicTwist, currentBankingAngle);

        tRot = initTorsoRot * Quaternion.Euler(0f, -pelvicTwist * 0.4f, 0f);

        // Neck Vestibulo-Ocular Stabilization (keeps gaze level with horizon)
        float headPitchCompensation = -forwardLean * 0.55f - (bounceSine * 2.2f);
        float headRollCompensation = -currentBankingAngle * 0.5f;
        hRot = initHeadRot * Quaternion.Euler(headPitchCompensation, -pelvicTwist * 0.3f, headRollCompensation);
    }

    /// <summary>
    /// Biomechanical human running phase calculations for a single leg:
    /// Stance phase (0 to PI): heel contact, weight absorption compression, push-off.
    /// Swing phase (PI to 2*PI): explosive knee tuck drive, forward reach.
    /// </summary>
    private void EvaluateLimbGait(float phase, float maxThighAngle,
        out float hipAngle, out float kneeAngle, out float ankleAngle, out bool footStrike)
    {
        footStrike = false;

        if (phase < Mathf.PI)
        {
            // === STANCE PHASE (Foot on track pushing backward) ===
            float t = phase / Mathf.PI; // 0 (contact) to 1 (toe-off)

            // Hip rotates from forward (+maxThighAngle) to backward (-maxThighAngle)
            hipAngle = Mathf.Lerp(maxThighAngle, -maxThighAngle, Mathf.SmoothStep(0f, 1f, t));

            // Knee flexes to absorb impact at mid-stance (t ~ 0.35), then extends during push-off
            if (t < 0.4f)
            {
                float midT = t / 0.4f;
                kneeAngle = Mathf.Lerp(12f, 26f, Mathf.Sin(midT * Mathf.PI));
            }
            else
            {
                float pushT = (t - 0.4f) / 0.6f;
                kneeAngle = Mathf.Lerp(26f, 6f, pushT);
            }

            // Ankle roll: heel-strike dorsiflexion (+14 deg) -> flat (0 deg) -> toe push-off (-30 deg)
            if (t < 0.25f)
            {
                ankleAngle = Mathf.Lerp(14f, 0f, t / 0.25f);
                if (t < 0.12f) footStrike = true;
            }
            else
            {
                ankleAngle = Mathf.Lerp(0f, -32f, (t - 0.25f) / 0.75f);
            }
        }
        else
        {
            // === SWING PHASE (Foot airborne driving forward) ===
            float t = (phase - Mathf.PI) / Mathf.PI; // 0 to 1

            // Hip flexes forward from -maxThighAngle back to +maxThighAngle
            hipAngle = Mathf.Lerp(-maxThighAngle, maxThighAngle, Mathf.SmoothStep(0f, 1f, t));

            // Knee tucks sharply (up to 88 deg) to clear the ground, then extends forward
            if (t < 0.55f)
            {
                kneeAngle = Mathf.Lerp(6f, 88f, Mathf.Sin((t / 0.55f) * (Mathf.PI * 0.5f)));
            }
            else
            {
                float reachT = (t - 0.55f) / 0.45f;
                kneeAngle = Mathf.Lerp(88f, 12f, reachT);
            }

            // Ankle stays neutral/dorsiflexed during flight to avoid toe stubbing
            ankleAngle = Mathf.Lerp(-32f, 12f, Mathf.SmoothStep(0f, 1f, t));
        }
    }

    // =========================================================================
    // 2. JUMP ASCENT & APEX POSE EVALUATION
    // =========================================================================

    private void EvaluateJumpPose(float vY,
        out Vector3 bPos, out Quaternion bRot, out Quaternion hRot,
        out Quaternion lArm, out Quaternion rArm, out Quaternion lForearm, out Quaternion rForearm,
        out Quaternion lLeg, out Quaternion rLeg, out Quaternion lLowerLeg, out Quaternion rLowerLeg,
        out Quaternion lShoe, out Quaternion rShoe)
    {
        // Normalized jump elevation factor
        float launchFactor = Mathf.Clamp01(vY / 12f);

        // Body arches upward and forward
        bPos = initBodyPos + new Vector3(0f, 0.08f * launchFactor, 0f);
        bRot = initBodyRot * Quaternion.Euler(Mathf.Lerp(8f, -6f, launchFactor), 0f, 0f);
        hRot = initHeadRot * Quaternion.Euler(Mathf.Lerp(-4f, 8f, launchFactor), 0f, 0f);

        // Arms thrust upward to generate upward propulsion
        float armUpPitch = Mathf.Lerp(-45f, -78f, launchFactor);
        float armOutSpread = Mathf.Lerp(22f, 14f, launchFactor);
        lArm = initLeftArmRot * Quaternion.Euler(armUpPitch, 0f, -armOutSpread);
        rArm = initRightArmRot * Quaternion.Euler(armUpPitch, 0f, armOutSpread);

        // Elbows flex slightly for aerodynamic elevation
        float elbowFlex = Mathf.Lerp(45f, 65f, launchFactor);
        lForearm = Quaternion.Euler(-elbowFlex, 0f, 0f);
        rForearm = Quaternion.Euler(-elbowFlex, 0f, 0f);

        // Lead leg lifts high, trailing leg extends back
        float leadHip = Mathf.Lerp(-32f, -48f, launchFactor);
        float trailHip = Mathf.Lerp(24f, 36f, launchFactor);
        lLeg = initLeftLegRot * Quaternion.Euler(leadHip, 0f, -4f);
        rLeg = initRightLegRot * Quaternion.Euler(trailHip, 0f, 4f);

        lLowerLeg = Quaternion.Euler(Mathf.Lerp(52f, 75f, launchFactor), 0f, 0f);
        rLowerLeg = Quaternion.Euler(Mathf.Lerp(30f, 45f, launchFactor), 0f, 0f);

        lShoe = Quaternion.Euler(15f, 0f, 0f);
        rShoe = Quaternion.Euler(20f, 0f, 0f);
    }

    // =========================================================================
    // 3. FALL DESCENT POSE EVALUATION
    // =========================================================================

    private void EvaluateFallPose(float vY,
        out Vector3 bPos, out Quaternion bRot, out Quaternion hRot,
        out Quaternion lArm, out Quaternion rArm, out Quaternion lForearm, out Quaternion rForearm,
        out Quaternion lLeg, out Quaternion rLeg, out Quaternion lLowerLeg, out Quaternion rLowerLeg,
        out Quaternion lShoe, out Quaternion rShoe)
    {
        float fallIntensity = Mathf.Clamp01(Mathf.Abs(vY) / 25f);

        // Body straightens and prepares for impact
        bPos = initBodyPos + new Vector3(0f, -0.04f * fallIntensity, 0f);
        bRot = initBodyRot * Quaternion.Euler(12f + fallIntensity * 6f, 0f, 0f);

        // Head looks downward toward landing platform
        hRot = initHeadRot * Quaternion.Euler(-8f, 0f, 0f);

        // Arms flare back and outward to maintain aerodynamic balance against wind
        float armBack = 35f + fallIntensity * 20f;
        float armFlare = 32f + fallIntensity * 12f;
        lArm = initLeftArmRot * Quaternion.Euler(armBack, 0f, -armFlare);
        rArm = initRightArmRot * Quaternion.Euler(armBack, 0f, armFlare);

        lForearm = Quaternion.Euler(-25f, 0f, 0f);
        rForearm = Quaternion.Euler(-25f, 0f, 0f);

        // Legs reach downward and widen slightly to catch the ground
        lLeg = initLeftLegRot * Quaternion.Euler(-14f, 0f, -6f);
        rLeg = initRightLegRot * Quaternion.Euler(10f, 0f, 6f);

        // Knees slightly bent ready for impact absorption
        lLowerLeg = Quaternion.Euler(28f + fallIntensity * 15f, 0f, 0f);
        rLowerLeg = Quaternion.Euler(32f + fallIntensity * 15f, 0f, 0f);

        // Toes dorsiflex up to ensure heel/midfoot landing
        lShoe = Quaternion.Euler(16f, 0f, 0f);
        rShoe = Quaternion.Euler(16f, 0f, 0f);
    }

    // =========================================================================
    // 4. SLIDE POSE EVALUATION
    // =========================================================================

    private void EvaluateSlidePose(
        out Vector3 bPos, out Quaternion bRot, out Quaternion hRot,
        out Quaternion lArm, out Quaternion rArm, out Quaternion lForearm, out Quaternion rForearm,
        out Quaternion lLeg, out Quaternion rLeg, out Quaternion lLowerLeg, out Quaternion rLowerLeg,
        out Quaternion lShoe, out Quaternion rShoe)
    {
        // Drop center-of-mass deeply under obstacles
        bPos = initBodyPos + new Vector3(0f, -0.48f, -0.16f);
        bRot = initBodyRot * Quaternion.Euler(34f, -12f, 10f);

        // Head remains tilted forward and up to watch track ahead
        hRot = initHeadRot * Quaternion.Euler(-26f, 8f, -8f);

        // Lead leg (Left) shoots straight forward with heel on track
        lLeg = initLeftLegRot * Quaternion.Euler(-82f, 0f, 0f);
        lLowerLeg = Quaternion.Euler(6f, 0f, 0f);
        lShoe = Quaternion.Euler(22f, 0f, 0f); // Toe pointing up

        // Trailing leg (Right) tucks under pelvis
        rLeg = initRightLegRot * Quaternion.Euler(48f, 0f, 14f);
        rLowerLeg = Quaternion.Euler(90f, 0f, 0f);
        rShoe = Quaternion.Euler(-15f, 0f, 0f);

        // Left arm braces downward near track surface
        lArm = initLeftArmRot * Quaternion.Euler(60f, 0f, -22f);
        lForearm = Quaternion.Euler(-18f, 0f, 0f);

        // Right arm tucked for aerodynamic clearance
        rArm = initRightArmRot * Quaternion.Euler(45f, 0f, 28f);
        rForearm = Quaternion.Euler(-35f, 0f, 0f);
    }

    // =========================================================================
    // 5. IDLE POSE EVALUATION
    // =========================================================================

    private void EvaluateIdlePose(
        out Vector3 bPos, out Quaternion bRot, out Quaternion hRot,
        out Quaternion lArm, out Quaternion rArm, out Quaternion lForearm, out Quaternion rForearm,
        out Quaternion lLeg, out Quaternion rLeg, out Quaternion lLowerLeg, out Quaternion rLowerLeg,
        out Quaternion lShoe, out Quaternion rShoe)
    {
        idleTimer += Time.deltaTime;

        // Diaphragmatic respiratory breathing rhythm (2.4s cycle)
        float breathCycle = Mathf.Sin(idleTimer * 2.6f);
        float breathY = breathCycle * 0.022f;
        float breathSpinePitch = breathCycle * 1.5f;

        // Weight-shift cycle between left and right feet (3.6s cycle)
        float weightShift = Mathf.Sin(idleTimer * 1.7f);
        float lateralSway = weightShift * 0.026f;
        float hipRoll = weightShift * 2.2f;

        bPos = initBodyPos + new Vector3(lateralSway, breathY, 0f);
        bRot = initBodyRot * Quaternion.Euler(breathSpinePitch, 0f, hipRoll);

        // Gentle head sway looking casually ahead
        float headGlance = Mathf.Sin(idleTimer * 0.85f) * 4.5f;
        hRot = initHeadRot * Quaternion.Euler(-breathSpinePitch * 0.5f, headGlance, -hipRoll * 0.5f);

        // Arms rest naturally at sides with slight elbow relaxation
        float armBreathSway = breathCycle * 1.8f;
        lArm = initLeftArmRot * Quaternion.Euler(armBreathSway, 0f, -6f - hipRoll);
        rArm = initRightArmRot * Quaternion.Euler(armBreathSway, 0f, 6f - hipRoll);

        lForearm = Quaternion.Euler(-16f, 0f, 0f);
        rForearm = Quaternion.Euler(-16f, 0f, 0f);

        // Supporting leg straightens, resting leg flexes slightly
        float leftLegFlex = Mathf.Max(0f, -weightShift) * 7f;
        float rightLegFlex = Mathf.Max(0f, weightShift) * 7f;

        lLeg = initLeftLegRot * Quaternion.Euler(-leftLegFlex * 0.5f, 0f, 0f);
        rLeg = initRightLegRot * Quaternion.Euler(-rightLegFlex * 0.5f, 0f, 0f);

        lLowerLeg = Quaternion.Euler(leftLegFlex, 0f, 0f);
        rLowerLeg = Quaternion.Euler(rightLegFlex, 0f, 0f);

        lShoe = initLeftShoeRot;
        rShoe = initRightShoeRot;
    }

    // =========================================================================
    // 6. EYE GAZE & MICRO-SACCADES
    // =========================================================================

    private void UpdateHeadEyeGazeTracking()
    {
        if (leftPupil == null && rightPupil == null) return;

        eyeShiftTimer -= Time.deltaTime;
        if (eyeShiftTimer <= 0f)
        {
            eyeShiftTimer = Random.Range(1.8f, 4.2f);
            // Micro-saccade horizontal and vertical eye glance
            currentEyeOffset = new Vector2(Random.Range(-0.015f, 0.015f), Random.Range(-0.008f, 0.008f));
        }

        Vector3 eyeDisp = new Vector3(currentEyeOffset.x, currentEyeOffset.y, 0f);

        if (leftPupil != null)
            leftPupil.localPosition = Vector3.Lerp(leftPupil.localPosition, initLeftPupilPos + eyeDisp, Time.deltaTime * 10f);

        if (rightPupil != null)
            rightPupil.localPosition = Vector3.Lerp(rightPupil.localPosition, initRightPupilPos + eyeDisp, Time.deltaTime * 10f);
    }

    // =========================================================================
    // 7. DEATH COLLAPSE KINEMATICS
    // =========================================================================

    private void ApplyDeathKinematics()
    {
        float t = Time.deltaTime * 7f;

        if (bodyRoot != null)
        {
            bodyRoot.localPosition = Vector3.Lerp(bodyRoot.localPosition,
                initBodyPos + new Vector3(0f, -0.52f, -0.38f), t);
            bodyRoot.localRotation = Quaternion.Lerp(bodyRoot.localRotation,
                Quaternion.Euler(-74f, 18f, 0f), t);
        }

        if (head != null)
            head.localRotation = Quaternion.Lerp(head.localRotation, Quaternion.Euler(25f, -20f, 0f), t);

        if (leftArm != null)
            leftArm.localRotation = Quaternion.Lerp(leftArm.localRotation, Quaternion.Euler(-92f, 0f, -42f), t);
        if (rightArm != null)
            rightArm.localRotation = Quaternion.Lerp(rightArm.localRotation, Quaternion.Euler(-88f, 0f, 38f), t);

        if (leftLeg != null)
            leftLeg.localRotation = Quaternion.Lerp(leftLeg.localRotation, Quaternion.Euler(22f, 0f, -15f), t);
        if (rightLeg != null)
            rightLeg.localRotation = Quaternion.Lerp(rightLeg.localRotation, Quaternion.Euler(-18f, 0f, 15f), t);

        if (leftLowerLeg != null)
            leftLowerLeg.localRotation = Quaternion.Lerp(leftLowerLeg.localRotation, Quaternion.Euler(35f, 0f, 0f), t);
        if (rightLowerLeg != null)
            rightLowerLeg.localRotation = Quaternion.Lerp(rightLowerLeg.localRotation, Quaternion.Euler(45f, 0f, 0f), t);
    }
}
