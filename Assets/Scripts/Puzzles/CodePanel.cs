using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Puzzles
{
    public class CodePanel : MonoBehaviour
    {
        public UnityEvent<string> eventCodeChanged;
        
        [SerializeField] private TextMeshProUGUI codeText;
        [SerializeField] private List<PanelButton> buttons;
        [SerializeField] private int maxDigits;

        private string _currentString = "";

        private bool IsAtMax => _currentString.Length >= maxDigits;
        
        private void Start()
        {
            UpdateText();
            foreach (var button in buttons)
            {
                button.eventClicked.AddListener(OnButtonClicked);
            }
        }

        private void OnButtonClicked(string digit)
        {
            AddDigit(digit);
        }

        private void AddDigit(string digit)
        {
            if (!IsAtMax)
            {
                SetText(_currentString + digit, true);
            }
        }

        private void SetText(string text, bool notifyEvent)
        {
            _currentString = text;
            UpdateText();
            if (notifyEvent)
            {
                eventCodeChanged?.Invoke(_currentString);
            }
        }

        private void UpdateText()
        {
            codeText.text = _currentString.PadRight(maxDigits, '_');
        }

        public void Clear()
        {
            SetText("", false);
        }
    }
}