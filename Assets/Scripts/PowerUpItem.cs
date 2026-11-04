using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerUpItem : MonoBehaviour
{
    [Header("Power-Up Settings")]
    public PowerUpType powerUpType = PowerUpType.Magnet;
    public float duration = 10f;

    [Header("Visual Animation")]
    public float rotationSpeed = 100f;
    public float bobbingSpeed = 3f;
    public float bobbingHeight = 0.25f;

    private Vector3 initialPosition;
    private bool collected = false;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);

        float newY = initialPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && !player.IsDead)
        {
            collected = true;
            player.ActivatePowerUp(powerUpType, duration);
            Destroy(gameObject);
        }
    }
}
