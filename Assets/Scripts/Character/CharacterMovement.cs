using UnityEditor.Animations;
using UnityEngine;

namespace Character
{
    public class CharacterMovement : MonoBehaviour
    {
        public static int SpeedHashId = Animator.StringToHash("speed");

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private AnimatorController leftAnimatorController;
        [SerializeField] private AnimatorOverrideController rightAnimatorController;
    
        [SerializeField] private float speed;
    
        private Rigidbody2D _rb;
        
        public float Speed => speed;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Move(Vector2 dir)
        {
            if (!enabled)
                return;
            _rb.velocity = dir.normalized * speed;

            if (dir.x < 0)
            {
                animator.runtimeAnimatorController = leftAnimatorController;
            } else if (dir.x > 0)
            {
                animator.runtimeAnimatorController = rightAnimatorController;
            }
        }
        
        public void Stop()
        {
            Move(Vector2.zero);
        }

        private void Update()
        {
            animator.SetFloat(SpeedHashId, _rb.velocity.magnitude);
        }

    }
}