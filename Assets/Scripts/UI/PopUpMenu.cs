using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    public class PopUpMenu : MonoBehaviour
    {
        public Action eventShowed;
        public Action eventHidden;
        
        [SerializeField] private Transform menu;

        [SerializeField] private Vector3 initPosition;
        [SerializeField] private Vector3 endPosition;
        
        [SerializeField] private float timeToShow;
        [SerializeField] private float timeToHide;

        [SerializeField] private InputAction cancelAction;
        [SerializeField] private bool cancellable;

        private bool _moving;
        
        private void Start()
        {
            cancelAction.Enable();
            cancelAction.performed += context =>
            {
                if (cancellable)
                    HidePanel();
            };
            HideImmediately();
        }

        private void OnEnable()
        {
            cancelAction?.Enable();
        }

        private void OnDisable()
        {
            cancelAction?.Disable();
        }

        [NaughtyAttributes.Button]
        public void ShowPanel()
        {
            bool alreadyShowing = menu.gameObject.activeSelf;
            if (_moving || alreadyShowing)
                return;
            menu.gameObject.SetActive(true);
            StartCoroutine(MoveToTarget(
                initPosition,
                endPosition,
                timeToShow, () =>
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
                eventShowed?.Invoke();
            }));
        }
        
        [NaughtyAttributes.Button]
        public void HidePanel()
        {
            bool alreadyHidden = !menu.gameObject.activeSelf;
            if (_moving || alreadyHidden)
                return;
            StartCoroutine(MoveToTarget(
                endPosition, 
                initPosition, 
                timeToHide, () =>
            {
                HideImmediately();
                eventHidden?.Invoke();
            }));
        }

        private void HideImmediately()
        {
            menu.localPosition = endPosition;
            menu.gameObject.SetActive(false);
        }
        
        public void HidePanel(float time)
        {
            Invoke(nameof(HidePanel), time);
        }

        private IEnumerator MoveToTarget(Vector3 fromPosition, Vector3 targetPosition, float time, Action atEnd)
        {
            _moving = true;

            float startTime = Time.time;
            float endTime = startTime + time;
            while (Time.time <= endTime)
            {
                float normTime = (Time.time - startTime) / time;
                var position = Vector3.Lerp(fromPosition, targetPosition, normTime);
                menu.localPosition = position;
                yield return null;
            }

            menu.localPosition = targetPosition;
            atEnd();
            
            _moving = false;
        }
    }
}