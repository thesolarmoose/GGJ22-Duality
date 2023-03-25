using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using Utils.Attributes;
using Vector3 = UnityEngine.Vector3;

namespace Puzzles
{
    public class HandCursor : MonoBehaviour
    {
        public UnityEvent eventPressed;
        public UnityEvent eventReleased;
        public UnityEvent eventMoved;
        
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite clickSprite;
        [SerializeField, AutoProperty(AutoPropertyMode.Scene)] private Camera _camera;

        private SpriteRenderer _spriteRenderer;
        private Bounds _bounds;

        public Vector3 Position
        {
            get => transform.position;
            private set => transform.position = ClampPositionToBounds(value);
        }

        private void Awake()
        {
            _bounds = new Bounds();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Reset();
        }

        private void OnDisable()
        {
            Reset();
        }

        private void Reset()
        {
            _spriteRenderer.sprite = idleSprite;
        }

        private void ComputeScreenBounds()
        {
            var pos = _camera.transform.position;
            var aspect = _camera.aspect;
            var height = _camera.orthographicSize * 2;
            var width = height * aspect;

            var boundsSize = new Vector3(width, height, 1);
            _bounds.center = pos;
            _bounds.size = boundsSize;
        }

        private Vector3 ClampPositionToBounds(Vector3 position, bool recompute = true)
        {
            if (recompute)
            {
                ComputeScreenBounds();
            }

            position.x = Mathf.Clamp(position.x, _bounds.min.x, _bounds.max.x);
            position.y = Mathf.Clamp(position.y, _bounds.min.y, _bounds.max.y);
            return position;
        }

        public void Press()
        {
            _spriteRenderer.sprite = clickSprite;
            eventPressed.Invoke();
        }

        public void Release()
        {
            _spriteRenderer.sprite = idleSprite;
            eventReleased.Invoke();
        }

        public void Move(Vector3 position)
        {
            Position = position;
            eventMoved.Invoke();
        }
    }
}