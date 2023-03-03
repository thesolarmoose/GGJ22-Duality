using System.Collections.Generic;
using FmodExtensions;
using FMODUnity;
using InputActions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Puzzles
{
    public class PuzzleRespondPing : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer lightARenderer;
        [SerializeField] private SpriteRenderer lightBRenderer;
        
        [SerializeField] private Sprite lightSprite;
        [SerializeField] private Color lightAColor;
        [SerializeField] private Color lightAColorPressed;
        [SerializeField] private Color lightBColor;
        [SerializeField] private Color lightBColorPressed;
        [SerializeField] private EventReference _pingASound;
        [SerializeField] private EventReference _pingBSound;

        [SerializeField] private float pingRate;
        [SerializeField] private float responseTime;

        [SerializeField] private int responsesInRowForEvent;

        [SerializeField] private InputAction _leftPingInputAction;
        [SerializeField] private InputAction _rightPingInputAction;

        public UnityEvent eventResponsesInARow;
        public UnityEvent eventFailed;

        private bool _isPinging;
        private float _lastTimePing;
        private int _currentPingIndex;
        private bool _responded;
        private bool _respondedWell;
        private int _responsesInARow;

        private Dictionary<int, (SpriteRenderer, Color, Color, EventReference)> _indexMap;
        
        private void Start()
        {
            _leftPingInputAction.Enable();
            _rightPingInputAction.Enable();
            _leftPingInputAction.performed += context => HandleResponse(0);
            _rightPingInputAction.performed += context => HandleResponse(1);

            _indexMap = new Dictionary<int, (SpriteRenderer, Color, Color, EventReference)>
            {
                [0] = (lightARenderer, lightAColor, lightAColorPressed, _pingASound),
                [1] = (lightBRenderer, lightBColor, lightBColorPressed, _pingBSound)
            };
        }
        
        private void OnEnable()
        {
            _leftPingInputAction?.Enable();
            _rightPingInputAction?.Enable();
        }

        private void OnDisable()
        {
            _leftPingInputAction?.Disable();
            _rightPingInputAction?.Disable();
            StopPing();
        }

        public void StartPing()
        {
            _isPinging = true;
            Ping();
        }
        
        public void StopPing()
        {
            _isPinging = false;
            ClearSprite();
        }

        private void Ping()
        {
            if (!_isPinging)
                return;

            int randomIndex = Random.Range(0, 2);
            var (spriteRenderer, color, _, sound) = _indexMap[randomIndex];
            spriteRenderer.sprite = lightSprite;
            spriteRenderer.color = color;
            _lastTimePing = Time.time;
            _currentPingIndex = randomIndex;
            _responded = false;
            _respondedWell = false;
            
            sound.PlayEvent();
            
            Invoke(nameof(ClearSprite), responseTime);
            Invoke(nameof(CheckFailure), responseTime + 0.01f);
            Invoke(nameof(Ping), pingRate);
        }

        private void HandleResponse(int index)
        {
            Debug.Log($"HandleResponse: {index}");
            bool inResponseWindow = _lastTimePing + responseTime >= Time.time;
            bool correctIndex = index == _currentPingIndex;
            if (inResponseWindow && correctIndex && !_responded)
            {
                var (spriteRenderer, _, color, _) = _indexMap[index];
                spriteRenderer.sprite = lightSprite;
                spriteRenderer.color = color;
                _respondedWell = true;
                _responsesInARow++;
                CheckResponsesInARow();
            }
            else
            {
                Fail();
            }
            
            _responded = true;
        }

        private void CheckResponsesInARow()
        {
            if (_responsesInARow >= responsesInRowForEvent)
            {
                eventResponsesInARow?.Invoke();
            }
        }

        private void CheckFailure()
        {
            if (!_respondedWell)
            {
                Fail();
            }
        }

        private void Fail()
        {
            _responsesInARow = 0;
            eventFailed?.Invoke();
        }

        private void ClearSprite()
        {
            lightARenderer.sprite = null;
            lightBRenderer.sprite = null;
        }
    }
}