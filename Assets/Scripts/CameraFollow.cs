using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float speed;

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
        transform.position = newPosition;
    }
}