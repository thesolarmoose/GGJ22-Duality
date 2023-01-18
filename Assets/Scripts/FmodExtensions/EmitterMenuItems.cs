using FMODUnity;
using UnityEditor;

namespace FmodExtensions
{
    public static class EmitterMenuItems
    {
        [MenuItem("GameObject/FMOD Extensions/Play emitter's event")]
        public static void PlayEmitter()
        {
            bool onlyOneSelected = Selection.count == 1;
            var gameObject = Selection.activeGameObject;
            bool isGameObject = gameObject != null;
            if (onlyOneSelected && isGameObject)
            {
                bool hasAudioEmitter = gameObject.TryGetComponent<StudioEventEmitter>(out var emitter);
                if (hasAudioEmitter)
                {
                    emitter.Play();
                }
            }
        }
        
        [MenuItem("GameObject/FMOD Extensions/Play emitter's event", true)]
        public static bool PlayEmitterValidation()
        {
            bool onlyOneSelected = Selection.count == 1;
            var gameObject = Selection.activeGameObject;
            bool isGameObject = gameObject != null;
            if (onlyOneSelected && isGameObject)
            {
                bool hasAudioEmitter = gameObject.TryGetComponent<StudioEventEmitter>(out var _);
                if (hasAudioEmitter)
                {
                    return true;
                }
            }

            return false;
        }
    }
}