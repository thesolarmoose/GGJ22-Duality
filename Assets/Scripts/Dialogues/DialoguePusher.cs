using System;
using System.Collections.Generic;
using Dialogues.UI;
using UnityEngine;
using UnityEngine.Localization;

namespace Dialogues
{
    public class DialoguePusher : MonoBehaviour
    {
        [SerializeField] private List<LocalizedString> dialogues;
        [SerializeField] private bool pushOnStart;

        private void Start()
        {
            if (pushOnStart)
                Push();
        }

        public void Push()
        {
            DialoguePanel.Instance.PushDialogueSequence(dialogues.ConvertAll(d => d.GetLocalizedString()));
        }
    }
}