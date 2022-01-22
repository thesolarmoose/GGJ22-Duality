using System.Collections.Generic;
using Dialogues.UI;
using UnityEngine;

namespace Dialogues
{
    public class DialoguePusher : MonoBehaviour
    {
        [SerializeField] private List<string> dialogues;

        public void Push()
        {
            DialoguePanel.Instance.PushDialogueSequence(dialogues);
        }
    }
}