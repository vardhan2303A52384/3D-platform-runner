using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Gem : MonoBehaviour
{
    [Header("Gem Attributes")]
    public int gemValue = 1;
    public float rotationSpeed = 160f;
    public float bobbingSpeed = 4f;
    public float bobbingHeight = 0.25f;

    [Header("Magnet Attraction")]
    public float magnetFlySpeed = 20f;

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
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position + Vector3.up * 0.9f, magnetFlySpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPlayer.position + Vector3.up * 0.9f) < 0.8f)
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

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddGem(gemValue);
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayGem();
        }

        if (CollectionVFX.Instance != null)
        {
            CollectionVFX.Instance.PlayGemCollect(transform.position);
        }

        if (FloatingTextSpawner.Instance != null)
        {
            FloatingTextSpawner.Instance.SpawnGemText(transform.position, gemValue);
        }

        if (DailyMissionSystem.Instance != null)
        {
            DailyMissionSystem.Instance.OnGemCollected();
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
