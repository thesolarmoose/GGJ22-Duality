using System.Collections;
using System.Collections.Generic;
using System.IO;
using Character;
using InputActions;
using UnityEngine;
using UnityEngine.Playables;
using Utils;

namespace Narrative
{
    public class FinalcinematicStarter : MonoBehaviour
    {
        [SerializeField] private List<Vector2> targets;
        [SerializeField] private float distanceThreshold;
        [SerializeField] private GameObject weldEffects;
        [SerializeField] private GameObject canvas;
        [SerializeField] private GameObject tittle;
        [SerializeField] private GameObject credits;
        
        [SerializeField] private CharacterMovement character;
        [SerializeField] private PlayerController controller;

        [NaughtyAttributes.Button()]
        public void StartCinematic()
        {
            controller.enabled = false;
            var coroutines = targets.ConvertAll(target => MoveToTarget(target));
            coroutines.Add(CoroutineUtils.ActionCoroutine(() => weldEffects.SetActive(true)));
            coroutines.Add(CoroutineUtils.WaitTimeCoroutine(0.5f));
            coroutines.Add(CoroutineUtils.ActionCoroutine(() => weldEffects.SetActive(false)));
            coroutines.Add(CoroutineUtils.WaitTimeCoroutine(0.1f));
            coroutines.Add(CoroutineUtils.ActionCoroutine(() => weldEffects.SetActive(true)));
            coroutines.Add(CoroutineUtils.WaitTimeCoroutine(0.3f));
            coroutines.Add(CoroutineUtils.ActionCoroutine(() => weldEffects.SetActive(false)));
            coroutines.Add(CoroutineUtils.WaitTimeCoroutine(0.2f));
            coroutines.Add(CoroutineUtils.ActionCoroutine(() => weldEffects.SetActive(true)));
            coroutines.Add(CoroutineUtils.WaitTimeCoroutine(1f));
            coroutines.Add(CoroutineUtils.ActionCoroutine(() => canvas.SetActive(true)));
            coroutines.Add(CoroutineUtils.ActionCoroutine(() => tittle.SetActive(true)));
            coroutines.Add(CoroutineUtils.WaitTimeCoroutine(1.5f));
            coroutines.Add(CoroutineUtils.ActionCoroutine(() => tittle.SetActive(false)));
            coroutines.Add(CoroutineUtils.WaitTimeCoroutine(0.5f));
            coroutines.Add(CoroutineUtils.ActionCoroutine(() => credits.SetActive(true)));

            StartCoroutine(CoroutineUtils.CoroutineSequence(coroutines));
        }

        private IEnumerator MoveToTarget(Vector2 target)
        {
            bool keepMoving = true;
            while (keepMoving)
            {
                var position = character.transform.position;
                float dist = Vector2.Distance(target, position);

                var dir = target - (Vector2) position;
                character.Move(dir);
                yield return null;
                keepMoving = dist > distanceThreshold;
            }

            character.Stop();
            character.transform.position = target;
        }
    }
}