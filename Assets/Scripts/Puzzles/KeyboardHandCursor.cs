using InputActions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Puzzles
{
    public class KeyboardHandCursor : MonoBehaviour
    {
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite clickSprite;

        [SerializeField] private float moveSpeed;
            
        private SpriteRenderer _spriteRenderer;

        private GameInputActions _inputActions;
        
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _inputActions = new GameInputActions();
            _inputActions.Enable();
            _inputActions.Player.Interaction.performed += ctxt => _spriteRenderer.sprite = clickSprite;
            _inputActions.Player.Interaction.canceled += ctxt => _spriteRenderer.sprite = idleSprite;
            _spriteRenderer.sprite = idleSprite;
        }

        private void OnEnable()
        {
            _inputActions?.Enable();
        }

        private void OnDisable()
        {
            _inputActions?.Disable();
            if (_spriteRenderer)
                _spriteRenderer.sprite = idleSprite;
        }

        private void Update()
        {
            var moveDir = _inputActions.Player.Move.ReadValue<Vector2>();
            var position = transform.position + (Vector3) (moveSpeed * Time.deltaTime * moveDir.normalized);
            transform.position = position;
        }
    }
}