using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Services.Analytics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Services.Analytics
{
    [CreateAssetMenu(fileName = "EventTrigger", menuName = "Analytics/EventTrigger", order = 0)]
    public class AnalyticsEventTriggerAsset : ScriptableObject
    {
        public void TriggerAnalyticsEvent(Object evtObj)
        {
            if (evtObj is ICustomEvent evt)
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
}