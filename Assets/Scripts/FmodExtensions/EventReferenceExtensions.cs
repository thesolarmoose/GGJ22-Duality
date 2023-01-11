using FMODUnity;

namespace FmodExtensions
{
    public static class EventReferenceExtensions
    {
        public static void PlayEvent(this EventReference @event)
        {
            FMODUnity.RuntimeManager.CreateInstance(@event).start();
        }
    }
}