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

        private InputAction _moveActionClone;
        private InputAction _interactionActionClone;
        
        
        private void Start()
        {
            _moveActionClone = _moveAction.action.Clone();
            _interactionActionClone = _interactionAction.action.Clone();
            _moveActionClone.Enable();
            _interactionActionClone.Enable();
            _interactionActionClone.performed += ctxt => _cursor.Press();
            _interactionActionClone.canceled += ctxt => _cursor.Release();
        }

        private void OnEnable()
        {
            _moveActionClone?.Enable();
            _interactionActionClone?.Enable();
        }

        private void OnDisable()
        {
            _moveActionClone?.Disable();
            _interactionActionClone?.Disable();
        }

        private void Update()
        {
            var moveDir = _moveActionClone.ReadValue<Vector2>();
            var position = transform.position + (Vector3) (moveSpeed * Time.deltaTime * moveDir.normalized);
            _cursor.Move(position);
        }
    }
}