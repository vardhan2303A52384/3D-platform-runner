using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
    [Header("Coin Attributes")]
    public int value = 1;

    [Header("Visual Animation")]
    public float rotationSpeed = 140f;
    public float bobbingSpeed = 3.5f;
    public float bobbingHeight = 0.2f;

    [Header("Magnet Attraction")]
    public float magnetFlySpeed = 22f;

    private Vector3 initialPosition;
    private bool collected = false;
    private Transform targetPlayer;

    private void Awake()
    {
        if (!gameObject.CompareTag("Coin"))
        {
            try { gameObject.tag = "Coin"; } catch {}
        }
    }

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);

        if (targetPlayer != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position + Vector3.up * 0.8f, magnetFlySpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPlayer.position + Vector3.up * 0.8f) < 0.7f)
            {
                Collect();
            }
        }
        else
        {
            float newY = initialPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            CheckMagnetAttraction();
        }
    }

    private void CheckMagnetAttraction()
    {
        if (PlayerController.ActivePlayer != null && PlayerController.ActivePlayer.hasMagnet)
        {
            float dist = Vector3.Distance(transform.position, PlayerController.ActivePlayer.transform.position);
            if (dist <= PlayerController.ActivePlayer.magnetRadius)
            {
                targetPlayer = PlayerController.ActivePlayer.transform;
            }
        }
    }

    public void Collect()
    {
        if (collected) return;
        collected = true;

        int multiplier = 1;
        if (ComboSystem.Instance != null)
        {
            ComboSystem.Instance.RegisterCoinPickup();
            multiplier = ComboSystem.Instance.comboMultiplier;
        }

        int finalValue = value * multiplier;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoin(finalValue);
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayCoin(multiplier);
        }

        if (CollectionVFX.Instance != null)
        {
            CollectionVFX.Instance.PlayCoinCollect(transform.position);
        }

        if (FloatingTextSpawner.Instance != null)
        {
            FloatingTextSpawner.Instance.SpawnCoinText(transform.position, finalValue, multiplier);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            Collect();
        }
    }
}
