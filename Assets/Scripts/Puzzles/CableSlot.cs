using UnityEngine;

namespace Puzzles
{
    public class CableSlot : MonoBehaviour
    {
        [SerializeField] private GameObject _weldSPot;

        public void Connect()
        {
            _weldSPot.SetActive(true);
        }

        public void Disconnect()
        {
            _weldSPot.SetActive(false);
        }
    }
}