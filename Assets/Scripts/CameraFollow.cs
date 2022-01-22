using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float speed;
    [SerializeField] private Rect bounds;

    private void LateUpdate()
    {
        MoveTowards();
    }

    private void MoveTowards()
    {
        var targetPosition = target.position;
        var selfPosition = transform.position;

        Vector3 newPosition = Vector2.Lerp(selfPosition, targetPosition, speed);
        newPosition.z = selfPosition.z;
        newPosition.x = Mathf.Clamp(newPosition.x, bounds.xMin, bounds.xMax);
        newPosition.y = Mathf.Clamp(newPosition.y, bounds.yMin, bounds.yMax);
        transform.position = newPosition;
    }
}