using System;
using UnityEngine;
using UnityEngine.Events;

namespace Puzzles
{
    public class HandCursor : MonoBehaviour
    {
        public UnityEvent eventPressed;
        public UnityEvent eventReleased;
        public UnityEvent eventMoved;
        
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite clickSprite;

        private SpriteRenderer _spriteRenderer;

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        private void Awake()
        {
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