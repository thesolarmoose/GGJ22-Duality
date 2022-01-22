using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Puzzles
{
    public class PanelButton : MonoBehaviour
    {
        public UnityEvent<string> eventClicked;    
        
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;
        
        private string _digit;

        private void Start()
        {
            _digit = text.text;
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            eventClicked?.Invoke(_digit);
        }
    }
}