using System;
using System.Collections;
using System.Collections.Generic;
using DaysSystem;
using Dialogues.UI;
using UI;
using UnityEngine;
using UnityEngine.Localization;

namespace Narrative
{
    public class EyeGlassTasks : MonoBehaviour
    {
        [SerializeField] private PopUpMenu menu;
        [SerializeField] private List<DayTasks> tasks;
        [SerializeField] private DialogueText text;
        [SerializeField] private float delayToStart;
        [SerializeField] private float delayToHide;

        [SerializeField] private LocalizedString pingTask;
            
        private bool _started;
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(delayToStart);
            var tasksText = GetDayTasks();
            ShowText(tasksText);
        }

        public void ShowText(LocalizedString textToShow)
        {
            menu.ShowPanel();
            text.PutText(textToShow.GetLocalizedString());
            _started = true;
        }

        public void ShowPingTask()
        {
            ShowText(pingTask);
        }

        private void Update()
        {
            if (_started)
            {
                if (!text.IsRunning)
                {
                    // finished
                    Invoke(nameof(Hide), delayToHide);
                }
            }
        }

        private void Hide()
        {
            menu.HidePanel();
            _started = false;
        }

        private LocalizedString GetDayTasks()
        {
            var day = DayData.Instance.Day;
            foreach (var task in tasks)
            {
                if (task.day == day)
                {
                    return task.tasksText;
                }
            }

            return tasks[0].tasksText;
        }
    }

    [Serializable]
    public struct DayTasks
    {
        public int day;
        public LocalizedString tasksText;
    }
}