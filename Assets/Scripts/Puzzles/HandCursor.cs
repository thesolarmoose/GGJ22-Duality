using System;
using UnityEngine;
using UnityEngine.Events;

namespace Puzzles
{
    public abstract class HandCursor : MonoBehaviour
    {
        public UnityEvent eventPressed;
        public UnityEvent eventReleased;
        
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite clickSprite;

        private SpriteRenderer _spriteRenderer;

        protected void Initialize()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = idleSprite;
        }

        protected void Reset()
        {
            if (_spriteRenderer)
                _spriteRenderer.sprite = idleSprite;
        }

        protected void Press()
        {
            _spriteRenderer.sprite = clickSprite;
            eventPressed?.Invoke();
        }

        protected void Release()
        {
            _spriteRenderer.sprite = idleSprite;
            eventReleased?.Invoke();
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}