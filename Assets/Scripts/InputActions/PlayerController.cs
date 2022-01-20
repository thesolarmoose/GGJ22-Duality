using Character;
using UnityEngine;

namespace InputActions
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterMovement character;

        private GameInputActions _inputActions;

        private void Start()
        {
            _inputActions = new GameInputActions();
            _inputActions.Enable();
        }

        private void FixedUpdate()
        {
            var value = _inputActions.Player.Move.ReadValue<Vector2>();
            character.Move(value);
        }

        private void OnEnable()
        {
            _inputActions?.Enable();
        }

        private void OnDisable()
        {
            _inputActions?.Disable();
        }
    }
}