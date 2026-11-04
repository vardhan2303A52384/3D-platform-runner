using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController ActivePlayer { get; private set; }

    [Header("Speed Settings")]
    public float forwardSpeed = 12f;
    public float maxSpeed = 26f;
    public float speedIncreaseRate = 0.08f;

    [Header("Lane Settings")]
    public float laneDistance = 3f;
    public float laneChangeSpeed = 18f;

    [Header("Jump & Physics Settings")]
    public float jumpHeight = 2.2f;
    public float gravity = -26f;
    public float fastFallGravity = -45f;

    [Header("Slide Settings")]
    public float slideDuration = 0.75f;
    public bool isSliding = false;

    [Header("Health & Lives (3 Hearts)")]
    public int maxHealth = 3;
    public int currentHealth = 3;
    public float damageInvulnerabilityTime = 1.5f;

    [Header("Power-Ups Active")]
    public bool hasMagnet = false;
    public float magnetRadius = 14f;
    public bool hasShield = false;
    public GameObject shieldVisual;
    public bool hasSpeedBoost = false;
    public bool has2xMultiplier = false;
    public bool isInvincible = false;

    [Header("Run State")]
    public bool isRunning = false;

    private int currentLane = 1;
    private CharacterController controller;
    private float verticalVelocity = 0f;
    private bool isDead = false;
    private bool isInvulnerable = false;
    private float initialSpeed;
    private Renderer[] playerRenderers;

    // Slide state tracking
    private float slideTimer = 0f;
    private float normalControllerHeight = 2.0f;
    private Vector3 normalControllerCenter = new Vector3(0f, 1.0f, 0f);
    private float slideControllerHeight = 0.85f;
    private Vector3 slideControllerCenter = new Vector3(0f, 0.45f, 0f);

    // Swipe controls tracking
    private Vector2 touchStartPos;
    private bool isSwiping = false;
    private const float SwipeThreshold = 35f;

    // Footstep audio & dust cadence
    private float footstepTimer = 0f;
    private float laneChangeVelocity = 0f;
    private bool wasGrounded = true;
    private float previousVerticalVelocity = 0f;

    public bool IsDead => isDead;
    public float CurrentVerticalVelocity => verticalVelocity;
    public float CurrentLateralVelocity => laneChangeVelocity;
    public float CurrentForwardSpeed => hasSpeedBoost ? forwardSpeed * 1.4f : forwardSpeed;

    private void Awake()
    {
        ActivePlayer = this;
        controller = GetComponent<CharacterController>();
        if (controller != null)
        {
            normalControllerHeight = controller.height;
            normalControllerCenter = controller.center;
        }
        initialSpeed = forwardSpeed;
        playerRenderers = GetComponentsInChildren<Renderer>();

        // Ensure trail effect exists
        if (GetComponentInChildren<PlayerTrailEffect>() == null)
        {
            gameObject.AddComponent<PlayerTrailEffect>();
        }

        // Remove ground contact shadow if present
        PlayerGroundShadow existingShadow = GetComponent<PlayerGroundShadow>();
        if (existingShadow != null)
        {
            Destroy(existingShadow);
        }
        GameObject contactShadowObj = GameObject.Find("GroundContactShadow");
        if (contactShadowObj != null)
        {
            Destroy(contactShadowObj);
        }
    }

    private void Start()
    {
        currentLane = 1;
        isDead = false;
        currentHealth = maxHealth;
        UpdateShieldVisual();
    }

    public void StartRunning()
    {
        isRunning = true;
        isDead = false;
        forwardSpeed = initialSpeed;
        currentHealth = maxHealth;
        isInvulnerable = false;
        isSliding = false;
        laneChangeVelocity = 0f;
        previousVerticalVelocity = 0f;
        wasGrounded = true;
        ClearAllPowerUps();

        if (PerkShopManager.Instance != null)
        {
            currentHealth += PerkShopManager.Instance.GetExtraMaxHealth();
            maxHealth = currentHealth;
            magnetRadius = PerkShopManager.Instance.GetMagnetRadius();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth);
        }

        if (ComboSystem.Instance != null)
        {
            ComboSystem.Instance.ResetForNewRun();
        }

        if (MilestoneSystem.Instance != null)
        {
            MilestoneSystem.Instance.ResetForNewRun();
        }

        if (DailyMissionSystem.Instance != null)
        {
            DailyMissionSystem.Instance.ResetRunState();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMusic();
        }
    }

    public void StopRunning()
    {
        isRunning = false;
    }

    private void Update()
    {
        if (isDead || !isRunning) return;

        // Speed acceleration over time
        float effectiveMaxSpeed = hasSpeedBoost ? maxSpeed * 1.5f : maxSpeed;
        if (forwardSpeed < effectiveMaxSpeed)
        {
            forwardSpeed += speedIncreaseRate * Time.deltaTime;
        }

        HandleInput();
        UpdateSlide();
        MovePlayer();
        UpdateFootsteps();
    }

    private void HandleInput()
    {
        // 1. Keyboard Controls
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            ShiftLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ShiftLane(1);
        }

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && controller.isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (controller.isGrounded)
            {
                StartSlide();
            }
            else
            {
                // Fast fall in mid-air
                verticalVelocity = fastFallGravity * 0.45f;
            }
        }

        // 2. Mouse / Touch Swipe Controls
        HandleSwipeInput();
    }

    private void HandleSwipeInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            isSwiping = true;
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            Vector2 delta = (Vector2)Input.mousePosition - touchStartPos;

            if (delta.magnitude >= SwipeThreshold)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    // Horizontal swipe
                    if (delta.x > 0f) ShiftLane(1);
                    else ShiftLane(-1);
                }
                else
                {
                    // Vertical swipe
                    if (delta.y > 0f && controller.isGrounded)
                    {
                        Jump();
                    }
                    else if (delta.y < 0f)
                    {
                        if (controller.isGrounded) StartSlide();
                        else verticalVelocity = fastFallGravity * 0.45f;
                    }
                }
            }
        }
    }

    private void ShiftLane(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, 0, 2);
    }

    private void Jump()
    {
        if (isSliding) EndSlide();

        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        BoyCharacterAnimator anim = GetComponentInChildren<BoyCharacterAnimator>();
        if (anim != null)
        {
            anim.OnJumpInitiated();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayJump();
        }
    }

    private void StartSlide()
    {
        if (isSliding) return;

        isSliding = true;
        slideTimer = slideDuration;

        if (controller != null)
        {
            controller.height = slideControllerHeight;
            controller.center = slideControllerCenter;
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySlide();
        }

        if (CollectionVFX.Instance != null)
        {
            CollectionVFX.Instance.PlaySlideDust(transform.position);
        }
    }

    private void UpdateSlide()
    {
        if (!isSliding) return;

        slideTimer -= Time.deltaTime;
        if (slideTimer <= 0f)
        {
            EndSlide();
        }
    }

    private void EndSlide()
    {
        if (!isSliding) return;

        isSliding = false;
        if (controller != null)
        {
            controller.height = normalControllerHeight;
            controller.center = normalControllerCenter;
        }
    }

    private void MovePlayer()
    {
        float targetX = (currentLane - 1) * laneDistance;
        float smoothTime = 0.10f; // Responsive and smooth lane transition
        float maxLateralSpeed = laneChangeSpeed * 1.5f;
        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref laneChangeVelocity, smoothTime, maxLateralSpeed);
        float deltaX = newX - transform.position.x;

        bool isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            if (!wasGrounded)
            {
                // Landing impact event
                float impactSpeed = Mathf.Abs(previousVerticalVelocity);
                if (impactSpeed > 4f)
                {
                    if (SoundManager.Instance != null)
                    {
                        SoundManager.Instance.PlayLanding(impactSpeed);
                    }
                    if (CollectionVFX.Instance != null)
                    {
                        CollectionVFX.Instance.PlayLandingDust(transform.position, impactSpeed);
                    }
                    if (impactSpeed > 14f && ScreenShakeEffect.Instance != null)
                    {
                        ScreenShakeEffect.Instance.ShakeLanding();
                    }
                }
            }

            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
        }
        else
        {
            float appliedGravity = (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) ? fastFallGravity : gravity;
            previousVerticalVelocity = verticalVelocity;
            verticalVelocity += appliedGravity * Time.deltaTime;
        }

        wasGrounded = isGrounded;

        float speed = hasSpeedBoost ? forwardSpeed * 1.4f : forwardSpeed;
        float deltaZ = speed * Time.deltaTime;

        Vector3 movement = new Vector3(deltaX, verticalVelocity * Time.deltaTime, deltaZ);
        controller.Move(movement);
    }

    /// <summary>
    /// Triggered directly and synchronously by BoyCharacterAnimator on physical foot strikes
    /// </summary>
    public void OnFootstrike(Vector3 footWorldPos, bool isLeft = true)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayFootstep(isLeft);
        }
        if (CollectionVFX.Instance != null)
        {
            CollectionVFX.Instance.PlayFootstepPuff(footWorldPos);
        }
    }

    private void UpdateFootsteps()
    {
        // Footstep audio and particle effects are synchronized directly with animation foot strikes in BoyCharacterAnimator
    }

    // --- Damage & Health System ---

    public void TakeDamage(int damage = 1)
    {
        if (isDead || isInvulnerable) return;

        // Invincibility or SpeedBoost destroys obstacles without taking damage
        if (isInvincible || hasSpeedBoost)
        {
            return;
        }

        // Shield absorbs hit
        if (hasShield)
        {
            hasShield = false;
            UpdateShieldVisual();
            StartCoroutine(InvulnerabilityRoutine(1.0f));

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayShieldBreak();
            }

            if (ScreenShakeEffect.Instance != null)
            {
                ScreenShakeEffect.Instance.ShakeHit();
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowStatusMessage("🛡 SHIELD BROKEN!");
            }
            return;
        }

        // Take damage
        currentHealth -= damage;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayHit();
        }

        if (ScreenShakeEffect.Instance != null)
        {
            ScreenShakeEffect.Instance.ShakeHit();
        }

        if (CollectionVFX.Instance != null)
        {
            CollectionVFX.Instance.PlayDamageEffect(transform.position);
        }

        if (FloatingTextSpawner.Instance != null)
        {
            FloatingTextSpawner.Instance.SpawnDamageText(transform.position);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine(damageInvulnerabilityTime));
        }
    }

    private IEnumerator InvulnerabilityRoutine(float duration)
    {
        isInvulnerable = true;
        float elapsed = 0f;
        float blinkInterval = 0.12f;

        while (elapsed < duration)
        {
            SetRenderersVisible(false);
            yield return new WaitForSeconds(blinkInterval);
            SetRenderersVisible(true);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval * 2f;
        }

        SetRenderersVisible(true);
        isInvulnerable = false;
    }

    private void SetRenderersVisible(bool visible)
    {
        if (playerRenderers == null) return;
        foreach (var r in playerRenderers)
        {
            if (r != null && (shieldVisual == null || r.gameObject != shieldVisual))
            {
                r.enabled = visible;
            }
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isRunning = false;
        verticalVelocity = 0f;
        if (isSliding) EndSlide();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayDeath();
        }

        if (ScreenShakeEffect.Instance != null)
        {
            ScreenShakeEffect.Instance.ShakeDeath();
        }

        if (CollectionVFX.Instance != null)
        {
            CollectionVFX.Instance.PlayDeathEffect(transform.position);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void Revive(int restoredHearts = 3)
    {
        isDead = false;
        isRunning = true;
        currentHealth = Mathf.Clamp(restoredHearts, 1, maxHealth);
        verticalVelocity = 0f;

        // Clear lane obstruction at revive point
        Collider[] nearby = Physics.OverlapSphere(transform.position, 8f);
        foreach (var col in nearby)
        {
            if (col.CompareTag("Obstacle") || col.GetComponent<Obstacle>() != null)
            {
                Destroy(col.gameObject);
            }
        }

        StartCoroutine(InvulnerabilityRoutine(3.0f));

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMilestone();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth);
            UIManager.Instance.ShowStatusMessage("✨ REVIVED! ✨");
        }
    }

    // --- Power-Ups Implementation ---

    public void ActivatePowerUp(PowerUpType type, float duration)
    {
        // Check perks for extended duration
        if (PerkShopManager.Instance != null)
        {
            if (type == PowerUpType.Magnet) duration = PerkShopManager.Instance.GetMagnetDuration();
            else if (type == PowerUpType.SpeedBoost) duration = PerkShopManager.Instance.GetSpeedBoostDuration();
            else if (type == PowerUpType.ScoreMultiplier) duration = PerkShopManager.Instance.GetMultiplierDuration();
        }

        switch (type)
        {
            case PowerUpType.Magnet:
                StartCoroutine(MagnetRoutine(duration));
                break;
            case PowerUpType.Shield:
                hasShield = true;
                UpdateShieldVisual();
                if (UIManager.Instance != null)
                    UIManager.Instance.ShowPowerUpActive(type, 999f);
                break;
            case PowerUpType.SpeedBoost:
                StartCoroutine(SpeedBoostRoutine(duration));
                break;
            case PowerUpType.ScoreMultiplier:
                StartCoroutine(ScoreMultiplierRoutine(duration));
                break;
            case PowerUpType.Invincibility:
                StartCoroutine(InvincibilityRoutine(duration));
                break;
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPowerUp();
        }

        if (ScreenShakeEffect.Instance != null)
        {
            ScreenShakeEffect.Instance.ShakePowerUp();
        }

        if (CollectionVFX.Instance != null)
        {
            CollectionVFX.Instance.PlayPowerUpCollect(transform.position, type);
        }

        if (DailyMissionSystem.Instance != null)
        {
            DailyMissionSystem.Instance.OnPowerUpCollected();
        }

        if (UIManager.Instance != null && type != PowerUpType.Shield)
        {
            UIManager.Instance.ShowPowerUpActive(type, duration);
        }
    }

    private IEnumerator MagnetRoutine(float duration)
    {
        hasMagnet = true;
        yield return new WaitForSeconds(duration);
        hasMagnet = false;
    }

    private IEnumerator SpeedBoostRoutine(float duration)
    {
        hasSpeedBoost = true;
        yield return new WaitForSeconds(duration);
        hasSpeedBoost = false;
    }

    private IEnumerator ScoreMultiplierRoutine(float duration)
    {
        has2xMultiplier = true;
        yield return new WaitForSeconds(duration);
        has2xMultiplier = false;
    }

    private IEnumerator InvincibilityRoutine(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }

    public void UpdateShieldVisual()
    {
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(hasShield);
        }
    }

    private void ClearAllPowerUps()
    {
        hasMagnet = false;
        hasShield = false;
        hasSpeedBoost = false;
        has2xMultiplier = false;
        isInvincible = false;
        UpdateShieldVisual();
    }

    // --- Collisions ---

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isDead || !isRunning) return;

        if (hit.gameObject.CompareTag("Obstacle") || hit.collider.GetComponent<Obstacle>() != null)
        {
            if (isInvincible || hasSpeedBoost)
            {
                if (CollectionVFX.Instance != null) CollectionVFX.Instance.PlayDamageEffect(hit.point);
                Destroy(hit.gameObject);
                return;
            }

            // If player is sliding and obstacle is high barrier, sliding clears under it!
            Obstacle obs = hit.collider.GetComponent<Obstacle>();
            if (isSliding && obs != null && obs.obstacleType == Obstacle.ObstacleType.HighBarrier)
            {
                // Ducked safely under high barrier!
                return;
            }

            if (hit.point.y > transform.position.y - (controller.height * 0.35f))
            {
                TakeDamage(1);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead || !isRunning) return;

        if (other.CompareTag("Obstacle") || other.GetComponent<Obstacle>() != null)
        {
            if (isInvincible || hasSpeedBoost)
            {
                if (CollectionVFX.Instance != null) CollectionVFX.Instance.PlayDamageEffect(other.transform.position);
                Destroy(other.gameObject);
                return;
            }

            Obstacle obs = other.GetComponent<Obstacle>();
            if (isSliding && obs != null && obs.obstacleType == Obstacle.ObstacleType.HighBarrier)
            {
                // Ducked safely under high barrier!
                return;
            }

            TakeDamage(1);
        }
        else if (other.CompareTag("Coin") || other.GetComponent<Coin>() != null)
        {
            Coin coin = other.GetComponent<Coin>();
            if (coin != null) coin.Collect();
        }
        else if (other.GetComponent<Gem>() != null)
        {
            Gem gem = other.GetComponent<Gem>();
            if (gem != null) gem.Collect();
        }
    }
}
