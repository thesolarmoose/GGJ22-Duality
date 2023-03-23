﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dialogues.UI
{
    public class DialoguePanel : MonoBehaviour
    {
        private static DialoguePanel _instance;
        public static DialoguePanel Instance => _instance;
        
        public Action eventAllDialoguesProcessed;
        public Action eventDialogueProcessed;
        
        [SerializeField] private GameObject panelContainer;
        [SerializeField] private DialogueText dialogueText;

        [SerializeField] private BoolVariable _pausedVariable;
        [SerializeField] private InputActionReference _nextDialogueAction;
        
        private List<string> _dialogueQueue;
        
        private bool _showingDialogue;
        public bool ShowingDialogue => _showingDialogue;

        private CancellationTokenSource _cts;
        private Task _currentTask;

        private void OnEnable()
        {
            _nextDialogueAction.action.Enable();
            CreateCts();
        }

        private void OnDisable()
        {
            _nextDialogueAction.action.Disable();
            DisposeCts();
        }

        private void CreateCts()
        {
            _cts = new CancellationTokenSource();
        }

        private void DisposeCts()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            _cts.Dispose();
            _cts = null;
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
            _dialogueQueue = new List<string>();
            ProcessQueue(false);

            _nextDialogueAction.action.performed += ctx => NextDialogue();
        }

        private bool ShouldProcessOnPush()
        {
            bool shouldProcess = _dialogueQueue.Count == 0 && !dialogueText.IsRunning;
            return shouldProcess;
        }

        public void PushDialogueSequence(List<string> sequence, bool clearOthers = true)
        {
            if (clearOthers)
            {
                ClearQueue();
            }

            bool shouldProcess = ShouldProcessOnPush();

            foreach (var dialogue in sequence)
            {
                _dialogueQueue.Add(dialogue);
            }
            
            if (shouldProcess)
            {
                ProcessQueue();
            }
        }
        
        public async void PushDialogueSequenceAsync(List<string> sequence)
        {
            PushDialogueSequenceAsync(sequence, null, null);
        }
        
        public async void PushDialogueSequenceAsync(List<string> sequence, Action beforeDialogue, Action afterDialogue)
        {
            await ClosePreviousDialogueAndWait();

            beforeDialogue?.Invoke();
            PushDialogueSequence(sequence);

            var ct = _cts.Token;
            _currentTask = WaitForDialogues(afterDialogue, ct);
            await _currentTask;
        }

        private async Task ClosePreviousDialogueAndWait()
        {
            // dispose and wait for previous task
            if (ShowingDialogue)
            {
                DisposeCts();
                await _currentTask;
                CreateCts();
            }
        }

        private async Task WaitForDialogues(Action afterDialogue, CancellationToken ct)
        {
            while (ShowingDialogue && !ct.IsCancellationRequested)
            {
                await Task.Yield();
            }
            
            afterDialogue?.Invoke();
            await Task.Yield();
        }

        public void ClearQueue()
        {
            _dialogueQueue.Clear();
            ProcessQueue(false);
            dialogueText.ShowAll();
        }

        private void ProcessQueue(bool emitEvents = true)
        {
            if (_dialogueQueue.Count > 0)
            {
                panelContainer.SetActive(true);
                var dialogue = _dialogueQueue[0];
                _dialogueQueue.RemoveAt(0);

                dialogueText.PutText(dialogue);

                _showingDialogue = true;
                
                if (emitEvents)
                {
                    eventDialogueProcessed?.Invoke();
                }
            }
            else
            {
                dialogueText.PutText("");
                panelContainer.SetActive(false);

                _showingDialogue = false;
                
                if (emitEvents)
                {
                    eventAllDialoguesProcessed?.Invoke();
                }
            }
        }

        private void NextDialogue()
        {
            bool isPaused = _pausedVariable.Value;
            if (!isPaused && _showingDialogue)
            {
                if (dialogueText.IsRunning)
                {
                    dialogueText.ShowAll();
                }
                else
                {
                    ProcessQueue();
                }
            }
        }
    }
}