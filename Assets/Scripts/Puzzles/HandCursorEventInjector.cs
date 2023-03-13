using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzles
{
    public class HandCursorEventInjector : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private HandCursor cursor;

        private GameObject _lastEntered;
        
        private void OnEnable()
        {
            cursor.eventPressed.AddListener(OnPressed);
            cursor.eventReleased.AddListener(OnReleased);
            cursor.eventMoved.AddListener(OnMoved);
        }

        private void OnDisable()
        {
            cursor.eventPressed.RemoveListener(OnPressed);
            cursor.eventReleased.RemoveListener(OnReleased);
            cursor.eventMoved.RemoveListener(OnMoved);
        }

        private void OnPressed()
        {
            var (interactable, injectedEvent) = GetInteractable(typeof(IPointerDownHandler));
            if (interactable)
            {
                ExecuteEvents.Execute(interactable.gameObject, injectedEvent, ExecuteEvents.pointerDownHandler);
            }
        }

        private void OnReleased()
        {
            var (interactable, injectedEvent) = GetInteractable(typeof(IPointerUpHandler));
            if (interactable)
            {
                ExecuteEvents.Execute(interactable.gameObject, injectedEvent, ExecuteEvents.pointerClickHandler);
                ExecuteEvents.Execute(interactable.gameObject, injectedEvent, ExecuteEvents.pointerUpHandler);
            }
        }
        
        private void OnMoved()
        {
            var (interactable, injectedEvent) = GetInteractable(typeof(IPointerEnterHandler));
            if (interactable != _lastEntered)
            {
                if (interactable)
                {
                    ExecuteEvents.Execute(interactable, injectedEvent, ExecuteEvents.pointerEnterHandler);
                }
                
                if (_lastEntered)
                {
                    ExecuteEvents.Execute(_lastEntered, injectedEvent, ExecuteEvents.pointerExitHandler);
                }
            }
            _lastEntered = interactable;
        }

        private (GameObject, PointerEventData) GetInteractable(Type type)
        {
            var position = cursor.Position;
            var screenPosition = camera.WorldToScreenPoint(position);
            var injectedEvent = new PointerEventData(EventSystem.current);
            injectedEvent.position = screenPosition;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(injectedEvent, results);
            var result = FindFirstRaycast(results);
            return (result.gameObject, injectedEvent);
        }
        
        protected static RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
        {
            var candidatesCount = candidates.Count;
            for (var i = 0; i < candidatesCount; ++i)
            {
                if (candidates[i].gameObject == null)
                    continue;

                return candidates[i];
            }
            return new RaycastResult();
        }
        
    }
}