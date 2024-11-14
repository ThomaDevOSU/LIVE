using UnityEngine;

public class CompassArrow : MonoBehaviour
{
    public Transform player;           // Reference to the player's transform
    public Transform target;           // Reference to the task objective's transform
    public RectTransform compassArrow; // Reference to the CompassArrow RectTransform

    void Update()
    {
        if (target == null)
        {
            // Hide the compass arrow when there's no target
            compassArrow.gameObject.SetActive(false);
            return;
        }

        // Show the compass arrow when there is a target
        compassArrow.gameObject.SetActive(true);

        // Calculate the direction from the player to the target
        Vector3 direction = target.position - player.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the compass arrow to point in the direction of the target
        compassArrow.localRotation = Quaternion.Euler(0, 0, angle - 90);
    }

    // Optional method to set the target dynamically
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Optional method to clear the target and hide the arrow
    public void ClearTarget()
    {
        target = null;
    }
}
