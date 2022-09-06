using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dialogues.UI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;

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

        public async void Push()
        {
            var tasks = dialogues.ConvertAll(d =>
            {
                var operation = d.GetLocalizedStringAsync();
                return operation.Task;
            });
            
            var allTask = Task.WhenAll(tasks);

            var localizedDialogues = await allTask;
            DialoguePanel.Instance.PushDialogueSequence(localizedDialogues.ToList());
        }
    }
}