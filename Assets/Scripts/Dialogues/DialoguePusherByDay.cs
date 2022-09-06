using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        
        public async void Push()
        {
            var localizedDialogues = await GetDayDialogue();
            DialoguePanel.Instance.PushDialogueSequence(localizedDialogues);
            DialoguePanel.Instance.eventAllDialoguesProcessed += OnDialoguesProcessed;
        }

        private void OnDialoguesProcessed()
        {
            eventOnDialogueEnds?.Invoke();
            DialoguePanel.Instance.eventAllDialoguesProcessed -= OnDialoguesProcessed;
        }

        private async Task<List<string>> GetDayDialogue()
        {
            foreach (var dayDialogue in dialogues)
            {
                if (dayDialogue.day == DayData.Instance.Day)
                {
                    var tasks = dayDialogue.dialogues.ConvertAll(d => d.GetLocalizedStringAsync().Task);
                    var localizedDialogues = await Task.WhenAll(tasks);
                    return localizedDialogues.ToList();
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