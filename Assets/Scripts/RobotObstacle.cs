using UnityEngine;

[RequireComponent(typeof(Obstacle))]
public class RobotObstacle : MonoBehaviour
{
    [Header("Robot Visual Animation")]
    public float hoverSpeed = 3f;
    public float hoverHeight = 0.12f;
    public float eyePulseSpeed = 5f;

    [Header("Robot Parts")]
    public Transform robotBody;
    public Transform leftArm;
    public Transform rightArm;
    public Renderer eyeRenderer;

    private Vector3 initialBodyPos;
    private Color baseEyeColor;

    private void Start()
    {
        if (robotBody != null)
            initialBodyPos = robotBody.localPosition;

        if (eyeRenderer != null && eyeRenderer.sharedMaterial != null)
            baseEyeColor = eyeRenderer.sharedMaterial.color;
    }

    private void Update()
    {
        // 1. Mechanical hovering/bobbing
        if (robotBody != null)
        {
            float newY = initialBodyPos.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
            robotBody.localPosition = new Vector3(initialBodyPos.x, newY, initialBodyPos.z);
        }

        // 2. Robotic arm scanning
        float armAngle = Mathf.Sin(Time.time * hoverSpeed * 1.5f) * 18f;
        if (leftArm != null)
            leftArm.localRotation = Quaternion.Euler(armAngle, 0f, 0f);
        if (rightArm != null)
            rightArm.localRotation = Quaternion.Euler(-armAngle, 0f, 0f);

        // 3. Pulsing menacing red eye
        if (eyeRenderer != null)
        {
            float pulse = 0.7f + Mathf.PingPong(Time.time * eyePulseSpeed, 0.6f);
            Color eyeColor = new Color(baseEyeColor.r * pulse, baseEyeColor.g * pulse, baseEyeColor.b * pulse);
            eyeRenderer.material.color = eyeColor;
        }
    }
}
