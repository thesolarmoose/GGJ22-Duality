﻿using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using InputActions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Utils;

namespace Puzzles
{
    public class PuzzleWeld : MonoBehaviour
    {
        public UnityEvent eventPuzzleSolved;
        
        [SerializeField] private new Camera camera;
        [SerializeField] private Transform robotHand;
        
        [SerializeField] private List<CablePair> leftCablesPairs;
        [SerializeField] private List<CablePair> rightCablesPairs;

        [SerializeField] private Rect leftBounds;
        [SerializeField] private Rect rightBounds;
        
        [SerializeField] private float distanceToWeldCable;
        [SerializeField] private float distanceToCutCable;
        [SerializeField] private float timeToWeld;

        [SerializeField] private GameObject sparks;
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private LayerMask cablesMask;
        
        private GameInputActions _inputActions;
        
        private bool _clickIsDown;
        private CablePlug _currentDraggingPlug;
        private Dictionary<CablePlug, CableSlot> _connectedPlugs;
        private bool _isWelding;
        private float _startWeldingTime;
        private (CablePlug, CableSlot, bool) _currentWeldingPair;
        
        private bool IsDragging => _currentDraggingPlug != null;
        
        private void Start()
        {
            _connectedPlugs = new Dictionary<CablePlug, CableSlot>();
            
            _inputActions = new GameInputActions();
            _inputActions.Enable();
            _inputActions.UI.Click.performed += OnClick;
            _inputActions.UI.Point.performed += OnPointerMove;
            _inputActions.Player.Interaction.performed += StartWeld;
            _inputActions.Player.Interaction.canceled += StopWeld;

            audioSource.clip = GameSounds.Instance.puzzle2Weld;
            sparks.SetActive(false);
        }

        private void OnEnable()
        {
            _inputActions?.Enable();
            Reset();
        }

        private void OnDisable()
        {
            _inputActions?.Disable();
        }

        private void Reset()
        {
            var position = Vector3.zero;
            position.z = robotHand.localPosition.z;
            robotHand.localPosition = position;
            
            _clickIsDown = false;
            _currentDraggingPlug = null;
            _isWelding = false;
            audioSource.Stop();
            _currentWeldingPair = (default, default, false);
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            _clickIsDown = context.ReadValue<float>() > 0.5f;

            bool plugClicked = false;
            
            if (_clickIsDown)
            {
                var cablePlug = GetPlugAtPointer();
                if (cablePlug)
                {
                    _currentDraggingPlug = cablePlug;
                    plugClicked = true;
                }
            }
            
            if (!plugClicked)
            {
                _currentDraggingPlug = null;
            }
        }
        
        private void OnPointerMove(InputAction.CallbackContext context)
        {
            if (_clickIsDown && IsDragging)
            {
                var position = GetPointerToWorldPosition();
                var (closesSlot, distance) = GetClosestSlot(position);
                bool isFarEnough = distance > distanceToCutCable;
                bool isConnected = IsPlugConnected(_currentDraggingPlug);
                if (isConnected && isFarEnough)
                {
                    DisconnectPlug(_currentDraggingPlug);
                    var sounds = GameSounds.Instance;
                    sounds.PlaySound(sounds.puzzle2CutCable);
                }

                if (!isConnected || isFarEnough)
                {
                    SetPlugPosition(_currentDraggingPlug, position);
                }
            }
        }
        
        private void StartWeld(InputAction.CallbackContext context)
        {
            Debug.Log("start welding");
            _isWelding = true;
            audioSource.Play();
        }
        
        private void StopWeld(InputAction.CallbackContext context)
        {
            Debug.Log("stop welding");
            _isWelding = false;
            audioSource.Stop();
            _currentWeldingPair = (default, default, false);
            
            sparks.SetActive(false);
        }

        private void Update()
        {
            if (_isWelding)
            {
                var position = robotHand.transform.position;
                var (plug, slot, exists) = GetWeldablePairAtPosition(position);
                bool isWeldingTheSame = false;
                if (exists)
                {
                    Debug.Log("Exists plug and slot");
                    var (previousPlug, previousSlot, existsPrevious) = _currentWeldingPair;
                    bool isSame = previousPlug == plug && previousSlot == slot;
                    isWeldingTheSame = existsPrevious && isSame;
                    if (isWeldingTheSame)
                    {
                        float elapsed = Time.time - _startWeldingTime;
                        bool isConnected = IsPlugConnected(plug);
                        if (elapsed >= timeToWeld && !isConnected)
                        {
                            Debug.Log("connect");
                            ConnectPlugToSlot(plug, slot);
                            CheckSolveCondition();
                        }
                    }
                    
                    sparks.SetActive(true);
                }
                else
                {
                    sparks.SetActive(false);
                }

                if (!isWeldingTheSame)
                {
                    _startWeldingTime = Time.time;
                    _currentWeldingPair = (plug, slot, exists);
                }
            }
        }

        private List<CablePair> GetAllCablePairs()
        {
            var cablesPairs = new List<CablePair>();
            cablesPairs.AddRange(leftCablesPairs);
            cablesPairs.AddRange(rightCablesPairs);
            return cablesPairs;
        }
        
        private void SetPlugPosition(CablePlug plug, Vector3 position)
        {
            var bounds = GetPlugBounds(plug);
            var panelPosition = transform.position;
            var transf = plug.transform;
            position.z = transf.position.z;
            position.x = Mathf.Clamp(
                position.x,
                bounds.xMin + panelPosition.x,
                bounds.xMax + panelPosition.x);
            position.y = Mathf.Clamp(
                position.y,
                bounds.yMin + panelPosition.y,
                bounds.yMax + panelPosition.y);
            transf.position = position;
        }
        
        private void CheckSolveCondition()
        {
//            bool sameCount = _connectedPlugs.Count == cablesPairs.Count;
            bool sameCount = _connectedPlugs.Count == leftCablesPairs.Count + rightCablesPairs.Count;
            if (sameCount)
            {
                bool win = true;
                var cablesPairs = GetAllCablePairs();
                for (int i = 0; i < cablesPairs.Count && win; i++)
                {
                    var cablePair = cablesPairs[i];
                    var plug = cablePair.cablePlug;
                    var slot = cablePair.cableSlot;

                    var connectedSlot = _connectedPlugs[plug];
                    if (slot != connectedSlot)
                    {
                        win = false;
                    }
                }

                if (win)
                {
                    StartCoroutine(CoroutineUtils.CoroutineSequence(new List<IEnumerator>
                    {
                        CoroutineUtils.WaitTimeCoroutine(0.5f),
                        CoroutineUtils.ActionCoroutine(() => eventPuzzleSolved?.Invoke())
                    }));
                }
            }
        }

        private void ConnectPlugToSlot(CablePlug plug, CableSlot slot)
        {
            var connectPosition = slot.transform.position;
            SetPlugPosition(plug, connectPosition);
            _connectedPlugs.Add(plug, slot);
            AudioSource.PlayClipAtPoint(GameSounds.Instance.puzzle1CablesPlugIn, Vector3.zero);
        }

        private void DisconnectPlug(CablePlug plug)
        {
            _connectedPlugs.Remove(plug);
            AudioSource.PlayClipAtPoint(GameSounds.Instance.puzzle1CablesPlugOut, Vector3.zero);
        }

        private bool IsSlotConnected(CableSlot slot)
        {
            return _connectedPlugs.ContainsValue(slot);
        }
        
        private bool IsPlugConnected(CablePlug plug)
        {
            return _connectedPlugs.ContainsKey(plug);
        }

        private CablePlug GetPlugAtSlot(CableSlot slot)
        {
            foreach (var plug in _connectedPlugs.Keys)
            {
                var connectedSlot = _connectedPlugs[plug];
                if (slot == connectedSlot)
                    return plug;
            }

            return null;
        }
        
        private CablePlug GetPlugAtPointer()
        {
            CablePlug cablePlug = null;
            var worldPosition = GetPointerToWorldPosition();
            var hit = Physics2D.Raycast(
                worldPosition,
                Vector2.zero,
                Single.PositiveInfinity,
                    cablesMask
                );
            if (hit)
            {
                cablePlug = hit.transform.GetComponent<CablePlug>();
            }

            return cablePlug;
        }
        
        private Vector3 GetPointerToWorldPosition()
        {
            var screenPosition = _inputActions.UI.Point.ReadValue<Vector2>();
            var worldPosition = camera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        private (CablePlug, CableSlot, bool) GetWeldablePairAtPosition(Vector3 position)
        {
            var (plug, plugDistance) = GetClosestPlug(position);
            var (slot, slotDistance) = GetClosestSlot(position);
            if (plugDistance < distanceToWeldCable && slotDistance < distanceToWeldCable)
            {
                var plugPosition = plug.transform.position;
                var slotPosition = slot.transform.position;
                float distanceBetween = Vector3.Distance(plugPosition, slotPosition);
                if (distanceBetween < distanceToWeldCable)
                {
                    return (plug, slot, true);
                }
            }
            
            return (default, default, false);
        } 
        
        private (CableSlot, float) GetClosestSlot(Vector3 position)
        {
            float distance = Single.MaxValue;
            CableSlot closestSlot = default;
            var cablesPairs = GetAllCablePairs();
            foreach (var cablePair in cablesPairs)
            {
                var slot = cablePair.cableSlot;
                var slotPosition = slot.transform.position;
                position.z = slotPosition.z;
                float dist = (position - slotPosition).magnitude;
                if (dist < distance)
                {
                    distance = dist;
                    closestSlot = slot;
                }
            }

            return (closestSlot, distance);
        }
        
        private (CablePlug, float) GetClosestPlug(Vector3 position)
        {
            float distance = Single.MaxValue;
            CablePlug closestPlug = default;
            var cablesPairs = GetAllCablePairs();
            foreach (var cablePair in cablesPairs)
            {
                var plug = cablePair.cablePlug;
                var plugPosition = plug.transform.position;
                position.z = plugPosition.z;
                float dist = (position - plugPosition).magnitude;
                if (dist < distance)
                {
                    distance = dist;
                    closestPlug = plug;
                }
            }

            return (closestPlug, distance);
        }

        private bool IsPlugInPairsList(CablePlug plug, List<CablePair> pairs)
        {
            foreach (var pair in pairs)
            {
                if (pair.cablePlug == plug)
                {
                    return true;
                }
            }

            return false;
        }

        private Rect GetPlugBounds(CablePlug plug)
        {
            if (IsPlugInPairsList(plug, leftCablesPairs))
            {
                return leftBounds;
            }
            else
            {
                return rightBounds;
            }
        }
    }
}