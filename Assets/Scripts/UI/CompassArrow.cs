using UnityEngine;

public class CompassArrow : MonoBehaviour
{
    public Transform player;           // Reference to the player
    public Transform target;           // Reference to the task objective
    public RectTransform compassArrow; // Reference to the CompassArrow

    void Update()
    {
        if (target == null)
        {
            // Hide compass and show when given a target
            compassArrow.gameObject.SetActive(false);
            return;
        }

        compassArrow.gameObject.SetActive(true);

        // Calculate the direction from the player to the target
        Vector3 direction = target.position - player.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the compass arrow to point in the direction of the target
        compassArrow.localRotation = Quaternion.Euler(0, 0, angle - 90);
    }

    // (For the future) Dynamic Targets
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // (For the future) Clear Target
    public void ClearTarget()
    {
        target = null;
    }
}
