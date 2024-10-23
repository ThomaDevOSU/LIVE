using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Reference to the player
    public Vector3 offset; // Offset to maintain between player and camera
    public float smoothSpeed = 0.125f; // Smoothness factor

    private void LateUpdate()
    {
        // Calculate the desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}
