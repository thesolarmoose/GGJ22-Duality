using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzles
{
    public class CodePanelDoor : MonoBehaviour
    {
        [SerializeField] private List<HandCursor> cursors;
        [SerializeField] private float minX;
        [SerializeField] private float maxX;
        [SerializeField] private Rect handleBounds;

        [SerializeField] private float closeAccel;
        [SerializeField] private float maxVel;
        [SerializeField] private float closePosition;
        [SerializeField] private float arriveThreshold;

        private HandCursor _currentCursor;
        private float _currentOffsetX;

        private float _currentVelocity;

        private IEnumerator _closeCoroutine;
        
        private void Start()
        {
            foreach (var cursor in cursors)
            {
                cursor.eventPressed.AddListener(() => OnCursorPressed(cursor));
                cursor.eventReleased.AddListener(() => OnCursorReleased(cursor));
            }
        }

        private void Update()
        {
            // follow cursor
            if (_currentCursor != null)
            {
                var trans = transform;
                var position = trans.position;
                var cursorPosition = _currentCursor.GetPosition();
                position.x = cursorPosition.x + _currentOffsetX;
                trans.position = position;
            }
//            else if ()  // close door
//            {
//                
//            }
        }

        private void OnCursorPressed(HandCursor cursor)
        {
            if (_currentCursor != null)
                return;

            var position = cursor.GetPosition();
            if (!IsInsideHandle(position))
                return;
            
            StopCoroutine(_closeCoroutine);
            _currentCursor = cursor;
            _currentOffsetX = transform.position.x - position.x;
        }

        private void OnCursorReleased(HandCursor cursor)
        {
            if (cursor == _currentCursor)
            {
                _currentCursor = null;
                _currentVelocity = closeAccel;

                _closeCoroutine = CloseCoroutine();
                StartCoroutine(_closeCoroutine);
            }
        }

        private bool IsInsideHandle(Vector2 position)
        {
            return position.x >= handleBounds.xMin &&
                   position.x <= handleBounds.xMax &&
                   position.y >= handleBounds.yMin &&
                   position.y <= handleBounds.yMax;
        }

        private IEnumerator CloseCoroutine()
        {
            var position = transform.position;
            float dir = Mathf.Sign(closePosition - position.x);
            float vel = closeAccel * dir;
            bool arrived = false;
            while (!arrived)
            {
                position.x += vel;
                transform.position = position;

                float diff = closePosition - position.x;
                arrived = Mathf.Abs(diff) <= arriveThreshold;
                
                yield return null;
            }

            position.x = closePosition;
            transform.position = position;
        }
    }
}