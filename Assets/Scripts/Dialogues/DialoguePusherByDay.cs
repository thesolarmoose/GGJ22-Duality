using System;
using System.Collections.Generic;
using DaysSystem;
using Dialogues.UI;
using UnityEngine;

namespace Dialogues
{
    public class DialoguePusherByDay : MonoBehaviour
    {
        [SerializeField] private List<DayDialogue> dialogues;

        public void Push()
        {
            DialoguePanel.Instance.PushDialogueSequence(GetDayDialogue());
        }

        private List<string> GetDayDialogue()
        {
            foreach (var dayDialogue in dialogues)
            {
                if (dayDialogue.day == DayData.Instance.Day)
                {
                    return dayDialogue.dialogues;
                }
            }
            
            return new List<string>();
        }
    }

    [Serializable]
    public struct DayDialogue
    {
        public int day;
        public List<string> dialogues;
    }
}