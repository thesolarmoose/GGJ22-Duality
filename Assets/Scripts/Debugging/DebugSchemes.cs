using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Input;

namespace Debugging
{
    public class DebugSchemes : MonoBehaviour
    {
        private void Start()
        {
            CallEvents(InputSchemeObserverAsset.Instance.CurrentScheme);
        }

        private void OnEnable()
        {
            InputSchemeObserverAsset.Instance.OnSchemeChanged += CallEvents;
        }

        private void OnDisable()
        {
            InputSchemeObserverAsset.Instance.OnSchemeChanged -= CallEvents;
        }

        private void CallEvents(InputControlScheme scheme)
        {
            UnityEngine.Debug.Log(scheme);
        }
    }
}