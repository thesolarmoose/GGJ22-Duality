using System;
using Unity.Services.Analytics;
using UnityEngine;

namespace Services.Analytics
{
    public class AnalyticsEventTrigger : MonoBehaviour
    {
        [SerializeReference, SubclassSelector] private ICustomEvent _event;

        public void TriggerAnalyticsEvent()
        {
            var eventName = _event.GetEventName();
            var eventParams = _event.GetEventParams();
            
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