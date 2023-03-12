using System.Collections.Generic;
using DaysSystem;
using UnityEngine;
using Utils.Attributes;

namespace Services.Analytics
{
    [CreateAssetMenu(fileName = "PuzzleSolvedEvent", menuName = "Analytics/CustomEvents/PuzzleSolvedEvent", order = 0)]
    public class PuzzleSolvedEvent : ScriptableObject, ICustomEvent
    {
        [SerializeField] private int _number;

        [SerializeField, AutoProperty(AutoPropertyMode.Asset)]
        private DayData _dayData;
        
        public string GetEventName()
        {
            return "puzzle_solved";
        }

        public IDictionary<string, object> GetEventParams()
        {
            return new Dictionary<string, object>
            {
                {"number", _number},
                {"day", _dayData.Day}
            };
        }
    }
}