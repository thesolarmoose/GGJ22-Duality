using System.Collections.Generic;
using DaysSystem;
using UnityEngine;
using Utils.Attributes;

namespace Services.Analytics
{
    [CreateAssetMenu(fileName = "InteractionEvent", menuName = "Analytics/CustomEvents/InteractionEvent", order = 0)]
    public class InteractionEvent : ScriptableObject, ICustomEvent
    {
        [SerializeField] private string _interactedObject;

        [SerializeField, AutoProperty(AutoPropertyMode.Asset)]
        private DayData _dayData;
        
        public string GetEventName()
        {
            return "gameplay_interaction";
        }

        public IDictionary<string, object> GetEventParams()
        {
            return new Dictionary<string, object>
            {
                {"interactedObject", _interactedObject},
                {"day", _dayData.Day}
            };
        }
    }
}