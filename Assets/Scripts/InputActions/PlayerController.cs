using System.Collections.Generic;
using Character;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace InputActions
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterMovement character;

        [SerializeField] private List<BoolVariable> _stoppingStates;

        private GameInputActions _inputActions;
        private bool CanMove => _stoppingStates.TrueForAll(state => !state.Value);

        private void Start()
        {
            _inputActions = new GameInputActions();
            _inputActions.Enable();
        }

        private void FixedUpdate()
        {
            var value = CanMove ? _inputActions.Player.Move.ReadValue<Vector2>() : Vector2.zero;
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