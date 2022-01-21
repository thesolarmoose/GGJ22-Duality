using System;
using System.ComponentModel;
using InputActions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Puzzles
{
    public class MouseHandCursor : HandCursor
    {
        [SerializeField] private new Camera camera;

        private GameInputActions _inputActions;
        
        private void Start()
        {
            Initialize();
            _inputActions = new GameInputActions();
            _inputActions.Enable();
            _inputActions.UI.Click.performed += OnClick;

            Cursor.visible = false;
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();
            bool isClicked = value > 0.5f;
            if (isClicked)
            {
                Press();
            }
            else
            {
                Release();
            }
        }

        private void OnEnable()
        {
            _inputActions?.Enable();
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            _inputActions?.Disable();
            Cursor.visible = true;
        }

        private void Update()
        {
            var screenPointerPosition = _inputActions.UI.Point.ReadValue<Vector2>();
            var pointerPosition = camera.ScreenToWorldPoint(screenPointerPosition);
            pointerPosition.z = transform.position.z;
            transform.position = pointerPosition;
        }
    }
}