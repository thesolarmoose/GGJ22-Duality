using System;
using UnityEngine;
using UnityEngine.Events;

namespace Puzzles
{
    public class HandCursorEnterExitTrigger : MonoBehaviour
    {
        [SerializeField] private HandCursor _cursor;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private UnityEvent _onEnter;
        [SerializeField] private UnityEvent _onExit;

        private bool _lastFrameWasInside;
        
        private void Update()
        {
            var position = _cursor.Position;
            bool isInside = IsInsideCollider(position);

            if (_lastFrameWasInside ^ isInside)
            {
                var @event = isInside ? _onEnter : _onExit;
                @event.Invoke();
            }

            _lastFrameWasInside = isInside;
        }
        
        private bool IsInsideCollider(Vector2 position)
        {
            var hit = Physics2D.Raycast(position, Vector2.zero);
            bool inside = hit.transform == _collider.transform;
            return inside;
        }
    }
}