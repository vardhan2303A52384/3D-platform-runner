using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Obstacle : MonoBehaviour
{
    public enum ObstacleType
    {
        LowHurdle,
        HighBarrier,
        SideBlock
    }

    [Header("Obstacle Settings")]
    public ObstacleType obstacleType = ObstacleType.LowHurdle;

    private void Awake()
    {
        // Tag this object as Obstacle if not already tagged
        if (!gameObject.CompareTag("Obstacle"))
        {
            try
            {
                gameObject.tag = "Obstacle";
            }
            catch
            {
                // Tag might not be registered yet, Obstacle component check still protects it
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && !player.IsDead)
            {
                player.Die();
            }
        }
    }
}
