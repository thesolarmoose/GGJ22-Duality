using System;
using TNRD;
using Unity.Services.Analytics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Services.Analytics
{
    public class AnalyticsEventTrigger : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<ICustomEvent> _event;

        public SerializableInterface<ICustomEvent> Events
        {
            get => _event;
            set => _event = value;
        }

        public void TriggerAnalyticsEvent()
        {
            TriggerAnalyticsEvent(_event.Value);
        }
        
        public void TriggerAnalyticsEvent(Object evtObj)
        {
            if (evtObj is ICustomEvent evt)
            {
                TriggerAnalyticsEvent(evt);
            }
        }
        
        public void TriggerAnalyticsEvent(ICustomEvent evt)
        {
            var eventName = evt.GetEventName();
            var eventParams = evt.GetEventParams();
        
            AnalyticsService.Instance.CustomData(eventName, eventParams);

            try
            {
                AnalyticsService.Instance.Flush();
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}