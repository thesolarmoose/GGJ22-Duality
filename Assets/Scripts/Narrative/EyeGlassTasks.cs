using System;
using System.Collections;
using System.Collections.Generic;
using DaysSystem;
using Dialogues.UI;
using UI;
using UnityEngine;
using UnityEngine.Localization;
using Utils;

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
        [SerializeField] private LocalizedString weldTask;
        [SerializeField] private LocalizedString screenCheck;
        [SerializeField] private LocalizedString screenCheckSolved;
            
        private bool _started;
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(delayToStart);
            var tasksText = GetDayTasks();
            ShowText(tasksText);
        }

        public async void ShowText(LocalizedString textToShow)
        {
            
            menu.ShowPanel();
            var localizedText = await textToShow.GetLocalizedStringAsync().Task;
            text.PutText(localizedText);
            _started = true;
        }

        public void ShowPingTask()
        {
            ShowText(pingTask);
        }
        
        public void ShowWeldTask()
        {
            ShowText(weldTask);
        }
        
        public void ShowScreenCheck()
        {
            ShowText(screenCheck);
        }
        
        public void ShowScreenCheckSolved()
        {
            ShowText(screenCheckSolved);
        }

        private void Update()
        {
            if (_started)
            {
                if (!text.IsRunning)
                {
                    // finished
                    StartCoroutine(CoroutineUtils.CoroutineSequence(new List<IEnumerator>
                    {
                        CoroutineUtils.WaitTimeCoroutine(delayToHide),
                        Hide()
                    }));
                }
            }
        }

        private IEnumerator Hide()
        {
            menu.HidePanel();
            while (menu.IsShowing)
            {
                yield return null;
            }
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