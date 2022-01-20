using System;
using InputActions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Puzzles
{
    public class MouseHandCursor : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite clickSprite;
            
        private SpriteRenderer _spriteRenderer;

        private GameInputActions _inputActions;
        
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _inputActions = new GameInputActions();
            _inputActions.Enable();
            _inputActions.UI.Click.performed += OnClick;

            Cursor.visible = false;
            _spriteRenderer.sprite = idleSprite;
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();
            bool isClicked = value > 0.5f;
            if (isClicked)
            {
                _spriteRenderer.sprite = clickSprite;
            }
            else
            {
                _spriteRenderer.sprite = idleSprite;
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