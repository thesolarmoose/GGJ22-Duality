using System;
using System.Collections;
using System.Collections.Generic;
using FmodExtensions;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
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

        [SerializeField] private HandCursor _humanHand;

        [Header("Sounds")]
        [SerializeField] private EventReference _puzzleSolved;
        [SerializeField] private EventReference _cablePluggedIn;
        [SerializeField] private EventReference _cablePluggedOut;
        [SerializeField] private EventReference _cableGrab;
        [SerializeField] private EventReference _cableDrop;
        
        private bool _clickIsDown;
        private CablePlug _currentDraggingPlug;
        private Dictionary<CablePlug, CableSlot> _connectedPlugs;
            
        private bool IsDragging => _currentDraggingPlug != null;
        
        private void Start()
        {
            _connectedPlugs = new Dictionary<CablePlug, CableSlot>();
            _humanHand.eventPressed.AddListener(OnHandPressed);
            _humanHand.eventReleased.AddListener(OnHandReleased);
            _humanHand.eventMoved.AddListener(OnHandMoved);
        }

        private void OnHandPressed()
        {
            _clickIsDown = true;

            bool plugClicked = false;
            
            var cablePlug = GetPlugAtPointer();
            if (cablePlug)
            {
                _currentDraggingPlug = cablePlug;
                plugClicked = true;
                _cableGrab.PlayEvent();
            }
            
            if (!plugClicked)
            {
                _currentDraggingPlug = null;
            }
        }

        private void OnHandReleased()
        {
            if (_currentDraggingPlug != null)  // if was grabbing a cable
            {
                _cableDrop.PlayEvent();
            }
            _clickIsDown = false;
            _currentDraggingPlug = null;
        }
        
        private void OnHandMoved()
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
                        CoroutineUtils.ActionCoroutine(() =>
                        {
                            _puzzleSolved.PlayEvent();
                        }),
                        CoroutineUtils.ActionCoroutine(() => eventPuzzleSolved?.Invoke()),
                    }));
                }
            }
        }

        private void OnDisable()
        {
            _clickIsDown = false;
            _currentDraggingPlug = null;
        }

        private void ConnectPlugToSlot(CablePlug plug, CableSlot slot)
        {
            var connectPosition = slot.transform.position + connectSlotOffset;
            SetPlugPosition(plug, connectPosition);
            _connectedPlugs.Add(plug, slot);
            _cablePluggedIn.PlayEvent();
        }

        private void DisconnectPlug(CablePlug plug)
        {
            _connectedPlugs.Remove(plug);
            _cablePluggedOut.PlayEvent();
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
            return _humanHand.Position;
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