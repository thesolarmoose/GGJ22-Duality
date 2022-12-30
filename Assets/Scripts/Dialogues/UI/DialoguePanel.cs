using System;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

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
        [SerializeField] private float processTimeOut;

        [SerializeField] private BoolVariable _pausedVariable;
        [SerializeField] private KeyCode nextKey;
        
        private List<string> _dialogueQueue;
        private float _nextTimeToProcess;
        private bool _showingDialogue;

        public bool ShowingDialogue => _showingDialogue;

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
            _nextTimeToProcess = Time.time + processTimeOut;
        }

        private bool ShouldProcessOnPush()
        {
            bool shouldProcess = _dialogueQueue.Count == 0 && !dialogueText.IsRunning;
            return shouldProcess;
        }
        
        public void PushDialogue(string dialogue)
        {
            bool shouldProcess = ShouldProcessOnPush();
            _dialogueQueue.Add(dialogue);

            if (shouldProcess)
            {
                ProcessQueue();
            }
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

                _nextTimeToProcess = Time.time + processTimeOut;
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

        private void Update()
        {
            bool pressedNextKey = Input.GetKeyDown(nextKey);
            bool isPaused = _pausedVariable.Value;
            if (pressedNextKey && !isPaused)
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

            if (Time.time >= _nextTimeToProcess && ShowingDialogue && !isPaused)
            {
                ProcessQueue();
            }
        }
    }
}