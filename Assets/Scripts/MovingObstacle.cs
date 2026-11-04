using UnityEngine;

/// <summary>
/// Makes obstacles dynamic: moving side-to-side between lanes, bobbing, or rotating.
/// </summary>
[RequireComponent(typeof(Obstacle))]
public class MovingObstacle : MonoBehaviour
{
    public enum MovementPattern
    {
        SideToSide,
        Bobbing,
        Rotate
    }

    public MovementPattern pattern = MovementPattern.SideToSide;
    public float speed = 2.2f;
    public float distance = 1.8f;

    private Vector3 initialPosition;
    private float timeOffset;

    private void Start()
    {
        initialPosition = transform.position;
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        float t = Time.time * speed + timeOffset;

        switch (pattern)
        {
            case MovementPattern.SideToSide:
                float xOffset = Mathf.Sin(t) * distance;
                transform.position = new Vector3(initialPosition.x + xOffset, transform.position.y, transform.position.z);
                break;

            case MovementPattern.Bobbing:
                float yOffset = Mathf.Abs(Mathf.Sin(t)) * distance;
                transform.position = new Vector3(transform.position.x, initialPosition.y + yOffset, transform.position.z);
                break;

            case MovementPattern.Rotate:
                transform.Rotate(0f, speed * 60f * Time.deltaTime, 0f, Space.Self);
                break;
        }
    }
}

