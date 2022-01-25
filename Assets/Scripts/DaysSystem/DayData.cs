using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaysSystem
{
    [CreateAssetMenu(fileName = "DayData", menuName = "Days/DayData", order = 0)]
    public class DayData : ScriptableObject
    {
        private static DayData _instance;
        public static DayData Instance => _instance;

        [SerializeField] private int day;

//        [NaughtyAttributes.ShowNativeProperty]
        public int Day => day;

        public void AddDay()
        {
            day++;
        }

        public void AddDayAndRestart()
        {
            AddDay();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void Awake()
        {
            day = 0;
        }

        private void OnEnable()
        {
            if (_instance != null) return;
            _instance = this;
        }

        private void Reset()
        {
            day = 0;
        }
    }
}