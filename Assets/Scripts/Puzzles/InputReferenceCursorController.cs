using InputActions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Puzzles
{
    public class InputReferenceCursorController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _interactionAction;
        [SerializeField] private HandCursor _cursor;

        private void Start()
        {
            _moveAction.action.Enable();
            _interactionAction.action.Enable();
            _interactionAction.action.performed += ctxt => _cursor.Press();
            _interactionAction.action.canceled += ctxt => _cursor.Release();
        }

        private void OnEnable()
        {
            _moveAction.action.Enable();
            _interactionAction.action.Enable();
        }

        private void OnDisable()
        {
            _moveAction.action.Disable();
            _interactionAction.action.Disable();
        }

        private void Update()
        {
            var moveDir = _moveAction.action.ReadValue<Vector2>();
            var position = transform.position + (Vector3) (moveSpeed * Time.deltaTime * moveDir.normalized);
            _cursor.Move(position);
        }
    }
}