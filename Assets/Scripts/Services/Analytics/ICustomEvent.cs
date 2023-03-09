using System.Collections.Generic;

namespace Services.Analytics
{
    public interface ICustomEvent
    {
        string GetEventName();
        IDictionary<string, object> GetEventParams();
    }
}