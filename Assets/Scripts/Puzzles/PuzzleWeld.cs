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
    public class PuzzleWeld : MonoBehaviour
    {
        public UnityEvent eventPuzzleSolved;
        
        [SerializeField] private HandCursor _leftRobotHand;
        [SerializeField] private HandCursor _rightHumanHand;

        [SerializeField] private List<SlotPair> slotPairs;
        [SerializeField] private List<CablePlugId> leftCablesPlugs;
        [SerializeField] private List<CablePlugId> rightCablesPlugs;

        [SerializeField] private Rect leftBounds;
        [SerializeField] private Rect rightBounds;
        
        [SerializeField] private float distanceToWeldCable;
        [SerializeField] private float distanceToCutCable;
        [SerializeField] private float timeToWeld;

        [SerializeField] private GameObject sparks;

        [SerializeField] private LayerMask cablesMask;

        [Header("Sounds")]
        [SerializeField] private StudioEventEmitter _weldSoundEmitter;
        [SerializeField] private EventReference _weldCableSound;
        [SerializeField] private EventReference _cutCableSound;
        [SerializeField] private EventReference _solvedSound;
        
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
            
            _rightHumanHand.eventPressed.AddListener(OnRightHandPressed);
            _rightHumanHand.eventReleased.AddListener(OnRightHandReleased);
            _rightHumanHand.eventMoved.AddListener(OnRightHandMoved);
            _leftRobotHand.eventPressed.AddListener(StartWeld);
            _leftRobotHand.eventReleased.AddListener(StopWeld);

            sparks.SetActive(false);
        }

        private void OnEnable()
        {
            Reset();
        }

        private void Reset()
        {
            var position = Vector3.zero;
            position.z = _leftRobotHand.Position.z;
            _leftRobotHand.Move(position);
            _rightHumanHand.Move(position);
            
            _clickIsDown = false;
            _currentDraggingPlug = null;
            _isWelding = false;
            _weldSoundEmitter.Stop();
            _currentWeldingPair = (default, default, false);
        }

        private void OnRightHandPressed()
        {
            _clickIsDown = true;

            bool plugClicked = false;
            
            var cablePlug = GetPlugAtPointer();
            if (cablePlug)
            {
                _currentDraggingPlug = cablePlug;
                plugClicked = true;
            }
            
            if (!plugClicked)
            {
                _currentDraggingPlug = null;
            }
        }

        private void OnRightHandReleased()
        {
            _clickIsDown = false;
            _currentDraggingPlug = null;
        }
        
        private void OnRightHandMoved()
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
                }

                if (!isConnected || isFarEnough)
                {
                    SetPlugPosition(_currentDraggingPlug, position);
                }
            }
        }
        
        private void StartWeld()
        {
            _isWelding = true;
            _weldSoundEmitter.Play();
        }
        
        private void StopWeld()
        {
            _isWelding = false;
            _weldSoundEmitter.Stop();
            _currentWeldingPair = (default, default, false);
            
            sparks.SetActive(false);
        }

        private void Update()
        {
            if (_isWelding)
            {
                var position = _leftRobotHand.Position;
                var (plug, slot, exists) = GetWeldablePairAtPosition(position);
                bool isWeldingTheSame = false;
                if (exists)
                {
                    var (previousPlug, previousSlot, existsPrevious) = _currentWeldingPair;
                    bool isSame = previousPlug == plug && previousSlot == slot;
                    isWeldingTheSame = existsPrevious && isSame;
                    if (isWeldingTheSame)
                    {
                        float elapsed = Time.time - _startWeldingTime;
                        bool isConnected = IsPlugConnected(plug);
                        if (elapsed >= timeToWeld && !isConnected)
                        {
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

        private List<CablePlugId> GetAllCablePlugs()
        {
            var cablesPairs = new List<CablePlugId>();
            cablesPairs.AddRange(leftCablesPlugs);
            cablesPairs.AddRange(rightCablesPlugs);
            return cablesPairs;
        }

        private List<CableSlot> GetAllSlots()
        {
            var slots = new List<CableSlot>();
            foreach (var slotPair in slotPairs)
            {
                slots.Add(slotPair.leftSlot);
                slots.Add(slotPair.rightSlot);
            }

            return slots;
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
            bool sameCount = _connectedPlugs.Count == leftCablesPlugs.Count + rightCablesPlugs.Count;
            if (sameCount)
            {
                bool win = true;
                for (int i = 0; i < slotPairs.Count && win; i++)
                {
                    var slotPair = slotPairs[i];
                    var leftPlug = GetPlugAtSlot(slotPair.leftSlot);
                    var rightPlug = GetPlugAtSlot(slotPair.rightSlot);

                    int leftId = GetPlugId(leftPlug);
                    int rightId = GetPlugId(rightPlug);

                    bool firstAlternative = (leftId == slotPair.idA && rightId == slotPair.idB);
                    bool secondAlternative = (leftId == slotPair.idB && rightId == slotPair.idA);
                    win = firstAlternative || secondAlternative;
                }

                if (win)
                {
                    StartCoroutine(CoroutineUtils.CoroutineSequence(new List<IEnumerator>
                    {
                        CoroutineUtils.WaitTimeCoroutine(0.5f),
                        CoroutineUtils.ActionCoroutine(() =>
                        {
                            _solvedSound.PlayEvent();
                        }),
                        CoroutineUtils.ActionCoroutine(() => eventPuzzleSolved?.Invoke())
                    }));
                }
            }
        }

        private void ConnectPlugToSlot(CablePlug plug, CableSlot slot)
        {
            var connectPosition = slot.transform.position;
            SetPlugPosition(plug, connectPosition);
            slot.Connect();
            _connectedPlugs.Add(plug, slot);
            _weldCableSound.PlayEvent();
        }

        private void DisconnectPlug(CablePlug plug)
        {
            var slot = _connectedPlugs[plug];
            slot.Disconnect();
            _connectedPlugs.Remove(plug);
            _cutCableSound.PlayEvent();
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
            return _rightHumanHand.Position;
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
            var cablesPairs = GetAllCablePlugs();
            foreach (var slot in GetAllSlots())
            {
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
            var plugsIds = GetAllCablePlugs();
            foreach (var plugId in plugsIds)
            {
                var plug = plugId.plug;
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

        private bool IsPlugInPairsList(CablePlug plug, List<CablePlugId> pairs)
        {
            foreach (var pair in pairs)
            {
                if (pair.plug == plug)
                {
                    return true;
                }
            }

            return false;
        }

        private Rect GetPlugBounds(CablePlug plug)
        {
            if (IsPlugInPairsList(plug, leftCablesPlugs))
            {
                return leftBounds;
            }
            else
            {
                return rightBounds;
            }
        }

        private int GetPlugId(CablePlug plug)
        {
            var plugs = GetAllCablePlugs();
            foreach (var plugId in plugs)
            {
                if (plugId.plug == plug)
                    return plugId.id;
            }
            
            throw new ArgumentException("$Plug {plug} does not exists");
        }
    }

    [Serializable]
    public struct CablePlugId
    {
        public CablePlug plug;
        public int id;
    }

    [Serializable]
    public struct SlotPair
    {
        public CableSlot leftSlot;
        public CableSlot rightSlot;
        public int idA;
        public int idB;
    }
}