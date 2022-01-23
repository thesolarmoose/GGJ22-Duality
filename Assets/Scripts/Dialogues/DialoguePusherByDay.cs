using System;
using System.Collections.Generic;
using DaysSystem;
using Dialogues.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace Dialogues
{
    public class DialoguePusherByDay : MonoBehaviour
    {
        [SerializeField] private List<DayDialogue> dialogues;

        [SerializeField] private bool pushOnStart;
        
        public UnityEvent eventOnDialogueEnds;

        private void Start()
        {
            if (pushOnStart)
                Push();
        }
        
        public void Push()
        {
            DialoguePanel.Instance.PushDialogueSequence(GetDayDialogue());
            DialoguePanel.Instance.eventAllDialoguesProcessed += OnDialoguesProcessed;
        }

        private void OnDialoguesProcessed()
        {
            eventOnDialogueEnds?.Invoke();
            DialoguePanel.Instance.eventAllDialoguesProcessed -= OnDialoguesProcessed;
        }

        private List<string> GetDayDialogue()
        {
            foreach (var dayDialogue in dialogues)
            {
                if (dayDialogue.day == DayData.Instance.Day)
                {
                    return dayDialogue.dialogues.ConvertAll(d => d.GetLocalizedString());
                }
            }
            
            return new List<string>();
        }
    }

    [Serializable]
    public struct DayDialogue
    {
        public int day;
        public List<LocalizedString> dialogues;
    }
}