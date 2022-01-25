using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float speed;
    [SerializeField] private Rect bounds;

    private Camera _camera;
    private Rect _positionBounds;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        MoveTowards();
    }

    private void MoveTowards()
    {
        ComputePositionBounds();
        var targetPosition = target.position;
        var selfPosition = transform.position;

        Vector3 newPosition = Vector2.Lerp(selfPosition, targetPosition, speed);
        newPosition.z = selfPosition.z;
        newPosition.x = Mathf.Clamp(newPosition.x, _positionBounds.xMin, _positionBounds.xMax);
        newPosition.y = Mathf.Clamp(newPosition.y, _positionBounds.yMin, _positionBounds.yMax);
        transform.position = newPosition;
    }

    private void ComputePositionBounds()
    {
        float aspect = _camera.aspect;
        float height = _camera.orthographicSize * 2;
        float halfHeight = height * 0.5f;
        float width = height * aspect;
        float halfWidth = width * 0.5f;

        float xMin = bounds.xMin + halfWidth;
        float xMax = bounds.xMax - halfWidth;
        float yMin = bounds.yMin + halfHeight;
        float yMax = bounds.yMax - halfHeight;
        _positionBounds = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }
}