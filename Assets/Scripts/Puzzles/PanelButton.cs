using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Puzzles
{
    public class PanelButton : MonoBehaviour
    {
        public UnityEvent<string> eventClicked;    
        
        [SerializeField] private string digit;
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;

        public string Digit => digit;

        private void Start()
        {
            text.text = digit;
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            eventClicked?.Invoke(digit);
        }
    }
}