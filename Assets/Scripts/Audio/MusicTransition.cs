using System;
using System.Collections;
using UnityEngine;

namespace Audio
{
    public class MusicTransition : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, 1.0f)] private float fadeSpeed;

        private AudioSource _audioSource;
        private float _target;
        private bool _fading;
        
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_audioSource.isPlaying)
                _audioSource.Play();
                
            if (!_fading)
            {
                StartCoroutine(FadeToValue(0, 1));
            }
            else
            {
                _target = 1;
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!_fading)
            {
                StartCoroutine(FadeToValue(1, 0));
            }
            else
            {
                _target = 0;
            }
        }

        private IEnumerator FadeToValue(float from, float to)
        {
            _fading = true;
            _target = to;

            bool keepFading = true;
            while (keepFading)
            {
                _audioSource.volume = from;
                from = Mathf.Lerp(from, _target, fadeSpeed);

                yield return null;

                keepFading = !(Mathf.Abs(from - _target) < Mathf.Epsilon);
            }
            
            _audioSource.volume = _target;
            
            _fading = false;
        }
    }
}