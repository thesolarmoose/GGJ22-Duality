using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Utils.Extensions;

namespace UI
{
    public class Interactable : MonoBehaviour
    {
        public UnityEvent eventInteracted;
        public UnityEvent eventLeavedInteractionArea;
        
        [SerializeField] private LayerMask characterMask;
        [SerializeField] private Canvas interactableCanvas;
        [SerializeField] private InputAction action;

        private bool IsShowing => interactableCanvas.gameObject.activeSelf;

        public void ShowCanvas()
        {
            interactableCanvas.gameObject.SetActive(true);
        }

        public void HideCanvas()
        {
            interactableCanvas.gameObject.SetActive(false);
        }

        private void Start()
        {
            HideCanvas();
            action.Enable();
            action.performed += OnInteraction;
        }

        private void OnEnable()
        {
            action?.Enable();
        }

        private void OnDisable()
        {
            action?.Disable();
        }

        private void OnInteraction(InputAction.CallbackContext callbackContext)
        {
            if (IsShowing)
            {
                eventInteracted?.Invoke();
                HideCanvas();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            int otherLayer = other.gameObject.layer;
            if (characterMask.IsLayerInMask(otherLayer))
            {
                ShowCanvas();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            int otherLayer = other.gameObject.layer;
            if (characterMask.IsLayerInMask(otherLayer))
            {
                HideCanvas();
                eventLeavedInteractionArea?.Invoke();
            }
        }
    }
}