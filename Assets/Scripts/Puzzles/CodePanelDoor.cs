using System.Collections;
using System.Collections.Generic;
using Audio;
using Codice.Client.Commands.TransformerRule;
using UnityEngine;

namespace Puzzles
{
    public class CodePanelDoor : MonoBehaviour
    {
        [SerializeField] private List<HandCursor> cursors;
        [SerializeField] private Transform handle;
        [SerializeField] private float maxDisplacement;
        [SerializeField] private float closeAccel;
        [SerializeField] private float maxVel;

        private float _minX;
        private float _maxX;
        private HandCursor _currentCursor;
        private float _currentOffsetX;

        private IEnumerator _closeCoroutine;
        
        private void Start()
        {
            _minX = transform.localPosition.x;
            _maxX = _minX + maxDisplacement;
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
                var localPosition = trans.localPosition;
                localPosition.x = Mathf.Clamp(localPosition.x, _minX, _maxX);
                trans.localPosition = localPosition;
            }
        }

        private void OnCursorPressed(HandCursor cursor)
        {
            if (_currentCursor != null)
                return;

            var position = cursor.GetPosition();
            if (!IsInsideHandle(position))
                return;
            
            if (_closeCoroutine != null)
                StopCoroutine(_closeCoroutine);
            _currentCursor = cursor;
            _currentOffsetX = transform.position.x - position.x;
            
            var sounds = GameSounds.Instance;
            sounds.PlaySound(sounds.puzzle3OpenDoor);
        }

        private void OnCursorReleased(HandCursor cursor)
        {
            if (cursor == _currentCursor)
            {
                _currentCursor = null;
                _closeCoroutine = CloseCoroutine();
                StartCoroutine(_closeCoroutine);
            }
        }

        private bool IsInsideHandle(Vector2 position)
        {
            var hit = Physics2D.Raycast(position, Vector2.zero);
            bool inside = hit.transform == handle;
            return inside;
        }

        private IEnumerator CloseCoroutine()
        {
            var sounds = GameSounds.Instance;
            sounds.PlaySound(sounds.puzzle3CloseDoor);
            
            var position = transform.localPosition;
            int dir = (int) Mathf.Sign(_minX - position.x);
            float accel = dir * closeAccel;
            float vel = accel;
            bool arrived = false;
            while (!arrived)
            {
                vel = Mathf.Min(maxVel, vel + accel);
                position.x += vel;
                transform.localPosition = position;

                float diff = position.x - _minX;
                int diffSign = (int) Mathf.Sign(diff);
                arrived = diffSign == dir;
                
                yield return null;
            }

            position.x = _minX;
            transform.localPosition = position;
        }
    }
}