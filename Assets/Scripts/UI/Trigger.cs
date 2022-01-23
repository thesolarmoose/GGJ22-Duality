using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace UI
{
    public class Trigger : MonoBehaviour
    {
        public UnityEvent eventTriggerEnter;
        public UnityEvent eventTriggerExit;
        
        [SerializeField] private LayerMask characterMask;

        private void Start()
        {
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            int otherLayer = other.gameObject.layer;
            if (characterMask.IsLayerInMask(otherLayer))
            {
                eventTriggerEnter?.Invoke();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            int otherLayer = other.gameObject.layer;
            if (characterMask.IsLayerInMask(otherLayer))
            {
                eventTriggerExit?.Invoke();
            }
        }
    }
}