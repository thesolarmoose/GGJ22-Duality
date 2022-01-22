using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzles
{
    public class HandCursorEventInjector : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private HandCursor cursor;

        private void Start()
        {
            cursor.eventPressed.AddListener(OnPressed);
            cursor.eventReleased.AddListener(OnReleased);
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

        private (GameObject, PointerEventData) GetInteractable(Type type)
        {
            var position = cursor.GetPosition();
            var screenPosition = camera.WorldToScreenPoint(position);
            var injectedEvent = new PointerEventData(EventSystem.current);
            injectedEvent.position = screenPosition;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(injectedEvent, results);

            foreach (var result in results)
            {
                var interactable = result.gameObject.GetComponent(type);
                if (interactable)
                {
                    return (result.gameObject, injectedEvent);
                }
            }
            
            return (null, null);
        }
    }
}