using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Analytics
{
    [Serializable]
    public class InteractionEvent : ICustomEvent
    {
        [SerializeField] private string _interactedObject;
        
        public string GetEventName()
        {
            return "gameplay_interaction";
        }

        public IDictionary<string, object> GetEventParams()
        {
            return new Dictionary<string, object>
            {
                {"interactedObject", _interactedObject}
            };
        }
    }
}