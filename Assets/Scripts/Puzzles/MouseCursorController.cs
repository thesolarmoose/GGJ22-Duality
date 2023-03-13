using InputActions;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Attributes;

namespace Puzzles
{
    public class MouseCursorController : MonoBehaviour
    {
        [SerializeField, AutoProperty(AutoPropertyMode.Scene)] private Camera _camera;
        [SerializeField] private HandCursor _cursor;

        private GameInputActions _inputActions;
        
        private void Start()
        {
            _inputActions = new GameInputActions();
            _inputActions.Enable();
            _inputActions.UI.Click.performed += OnClick;
            _inputActions.UI.Point.performed += OnMove;
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();
            bool isClicked = value > 0.5f;
            if (isClicked)
            {
                _cursor.Press();
            }
            else
            {
                _cursor.Release();
            }
        }

        private void OnEnable()
        {
            _inputActions?.Enable();
        }

        private void OnDisable()
        {
            _inputActions?.Disable();
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            var screenPointerPosition = _inputActions.UI.Point.ReadValue<Vector2>();
            var pointerPosition = _camera.ScreenToWorldPoint(screenPointerPosition);
            pointerPosition.z = transform.position.z;
            _cursor.Move(pointerPosition);
        }
    }
}