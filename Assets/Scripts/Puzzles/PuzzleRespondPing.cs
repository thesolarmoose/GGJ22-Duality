using System;
using System.Collections.Generic;
using Audio;
using Codice.Client.BaseCommands;
using Codice.CM.SEIDInfo;
using HarmonyLib;
using InputActions;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Puzzles
{
    public class PuzzleRespondPing : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float volumePing;
        [SerializeField] private float volumeRespond;
        
        [SerializeField] private SpriteRenderer lightARenderer;
        [SerializeField] private SpriteRenderer lightBRenderer;
        
        [SerializeField] private Sprite lightSprite;
        [SerializeField] private Color lightAColor;
        [SerializeField] private Color lightAColorPressed;
        [SerializeField] private Color lightBColor;
        [SerializeField] private Color lightBColorPressed;

        [SerializeField] private float pingRate;
        [SerializeField] private float responseTime;

        [SerializeField] private int responsesInRowForEvent;

        public UnityEvent eventResponsesInARow;
        public UnityEvent eventFailed;

        private GameInputActions _inputActions;
        private bool _isPinging;
        private float _lastTimePing;
        private int _currentPingIndex;
        private bool _responded;
        private bool _respondedWell;
        private int _responsesInARow;

        private Dictionary<int, (SpriteRenderer, Color, Color, AudioClip)> _indexMap;
        
        private void Start()
        {
            _inputActions = new GameInputActions();
            _inputActions.Enable();
            _inputActions.PuzzlePing.Ping0.performed += context => HandleResponse(0);
            _inputActions.PuzzlePing.Ping1.performed += context => HandleResponse(1);
            
            _indexMap = new Dictionary<int, (SpriteRenderer, Color, Color, AudioClip)>
            {
                [0] = (lightARenderer, lightAColor, lightAColorPressed, GameSounds.Instance.puzzlePing0),
                [1] = (lightBRenderer, lightBColor, lightBColorPressed, GameSounds.Instance.puzzlePing1)
            };
        }
        
        private void OnEnable()
        {
            _inputActions?.Enable();
        }

        private void OnDisable()
        {
            _inputActions?.Disable();
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
            
            audioSource.clip = sound;
            audioSource.volume = volumePing;
            audioSource.Play();
            
            Invoke(nameof(ClearSprite), responseTime);
            Invoke(nameof(CheckFailure), responseTime + 0.01f);
            Invoke(nameof(Ping), pingRate);
        }

        private void HandleResponse(int index)
        {
            bool inResponseWindow = _lastTimePing + responseTime >= Time.time;
            bool correctIndex = index == _currentPingIndex;
            if (inResponseWindow && correctIndex && !_responded)
            {
                var (spriteRenderer, _, color, _) = _indexMap[index];
                spriteRenderer.sprite = lightSprite;
                spriteRenderer.color = color;
                _respondedWell = true;
                _responsesInARow++;
                audioSource.volume = volumeRespond;
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
            Debug.Log("Fail");
        }

        private void ClearSprite()
        {
            lightARenderer.sprite = null;
            lightBRenderer.sprite = null;
        }
    }
}