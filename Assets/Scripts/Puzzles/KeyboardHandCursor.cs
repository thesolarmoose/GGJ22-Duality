using InputActions;
using UnityEngine;

namespace Puzzles
{
    public class KeyboardHandCursor : HandCursor
    {
        [SerializeField] private float moveSpeed;
            
        private GameInputActions _inputActions;
        
        private void Start()
        {
            Initialize();
            _inputActions = new GameInputActions();
            _inputActions.Enable();
            _inputActions.Player.Interaction.performed += ctxt => Press();
            _inputActions.Player.Interaction.canceled += ctxt => Release();
        }

        private void OnEnable()
        {
            _inputActions?.Enable();
        }

        private void OnDisable()
        {
            _inputActions?.Disable();
            Reset();
        }

        private void Update()
        {
            var moveDir = _inputActions.Player.Move.ReadValue<Vector2>();
            var position = transform.position + (Vector3) (moveSpeed * Time.deltaTime * moveDir.normalized);
            transform.position = position;
        }
    }
}