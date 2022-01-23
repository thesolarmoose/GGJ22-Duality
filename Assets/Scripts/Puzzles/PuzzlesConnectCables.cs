using System;
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
    public class PuzzlesConnectCables : MonoBehaviour
    {
        public UnityEvent eventPuzzleSolved;
        
        [SerializeField] private new Camera camera;
        [SerializeField] private List<CablePair> cablesPairs;

        [SerializeField] private float distanceToSnap;
        [SerializeField] private Vector3 connectSlotOffset;

        [SerializeField] private LayerMask cablesMask;
        
        private GameInputActions _inputActions;

        private bool _clickIsDown;
        private CablePlug _currentDraggingPlug;
        private Dictionary<CablePlug, CableSlot> _connectedPlugs;
            
        private bool IsDragging => _currentDraggingPlug != null;
        
        private void Start()
        {
            _connectedPlugs = new Dictionary<CablePlug, CableSlot>();
            
            _inputActions = new GameInputActions();
            _inputActions.Enable();
            _inputActions.UI.Click.performed += OnClick;
            _inputActions.UI.Point.performed += OnPointerMove;
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
                var (closesSlot, distance) = GetClosesSlot(position);
                bool isCloseEnough = distance <= distanceToSnap;
                bool isConnected = IsSlotConnected(closesSlot);
                bool isSamePlug = false;
                if (isConnected)
                {
                    var connectedPlug = GetPlugAtSlot(closesSlot);
                    isSamePlug = connectedPlug == _currentDraggingPlug;
                }
                
                if (isCloseEnough && !isConnected)
                {
                    if (IsPlugConnected(_currentDraggingPlug))
                    {
                        var connectedSlot = _connectedPlugs[_currentDraggingPlug];
                        if (connectedSlot != closesSlot)
                            DisconnectPlug(_currentDraggingPlug);
                    }
                    
                    ConnectPlugToSlot(_currentDraggingPlug, closesSlot);

                    CheckSolveCondition();
                }
                else if (!isCloseEnough || !isSamePlug)
                {
                    if (IsPlugConnected(_currentDraggingPlug))
                    {
                        DisconnectPlug(_currentDraggingPlug);
                    }

                    SetPlugPosition(_currentDraggingPlug, position);
                }
            }
        }

        private void SetPlugPosition(CablePlug plug, Vector3 position)
        {
            var transf = plug.transform;
            position.z = transf.position.z;
            transf.position = position;
        }
        
        private void CheckSolveCondition()
        {
            bool sameCount = _connectedPlugs.Count == cablesPairs.Count;
            if (sameCount)
            {
                bool win = true;
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

        private void OnEnable()
        {
            _inputActions?.Enable();
        }

        private void OnDisable()
        {
            _inputActions?.Disable();
            _clickIsDown = false;
            _currentDraggingPlug = null;
        }

        private void ConnectPlugToSlot(CablePlug plug, CableSlot slot)
        {
            var connectPosition = slot.transform.position + connectSlotOffset;
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
                cablesMask);
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

        private CablePair GetSlotsPair(CableSlot slot)
        {
            foreach (var cablePair in cablesPairs)
            {
                if (cablePair.cableSlot == slot)
                    return cablePair;
            }
            
            throw new ArgumentException($"Slot {slot} does not exists");
        }

        private (CableSlot, float) GetClosesSlot(Vector3 position)
        {
            float distance = Single.MaxValue;
            CableSlot closestSlot = default;
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
    }

    [Serializable]
    public struct CablePair
    {
        public CablePlug cablePlug;
        public CableSlot cableSlot;
    }
}