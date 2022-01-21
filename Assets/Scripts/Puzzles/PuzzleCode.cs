using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Puzzles
{
    public class PuzzleCode : MonoBehaviour
    {
        public UnityEvent eventPuzzleSolved;
        
        [SerializeField] private CodePanel codePanel;
        [SerializeField] private float resetTime;
        [SerializeField] private string puzzleCode;

        private float _timeToReset;
        
        private void Start()
        {
            codePanel.eventCodeChanged.AddListener(OnCodeChanged);
        }

        private void Update()
        {
            if (_timeToReset <= Time.time)
            {
                codePanel.Clear();
            }
        }

        private void OnCodeChanged(string code)
        {
            if (code.Length == 1)
            {
                _timeToReset = Time.time + resetTime;
            }
            
            CheckSolveCondition(code);
        }

        private void CheckSolveCondition(string code)
        {
            bool win = code == puzzleCode;
            if (win)
            {
                print("code win");
                StartCoroutine(CoroutineUtils.CoroutineSequence(new List<IEnumerator>
                {
                    CoroutineUtils.WaitTimeCoroutine(0.5f),
                    CoroutineUtils.ActionCoroutine(() => eventPuzzleSolved?.Invoke())
                }));
            }
        }
    }
}