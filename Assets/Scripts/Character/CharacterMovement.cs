using UnityEngine;

namespace Character
{
    public class CharacterMovement : MonoBehaviour
    {
        public static int SpeedHashId = Animator.StringToHash("speed");
        public static int DirHashId = Animator.StringToHash("dir");

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;

        [SerializeField] private FMODUnity.StudioEventEmitter _stepSound;
//        [SerializeField] private AudioSource audioSource;
//        [SerializeField] private AudioClip stepClip;
        
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

            animator.SetFloat(DirHashId, dir.x);
//            if (dir.x < 0)
//            {
//            } else if (dir.x > 0)
//            {
//                animator.SetLayerWeight(1, 1);
//            }
        }
        
        public void Stop()
        {
            Move(Vector2.zero);
        }

        private void Update()
        {
            animator.SetFloat(SpeedHashId, _rb.velocity.magnitude);
        }

        public void PlayStepSound()
        {
            _stepSound.Play();
//            audioSource.PlayOneShot(stepClip);
        }
    }
}