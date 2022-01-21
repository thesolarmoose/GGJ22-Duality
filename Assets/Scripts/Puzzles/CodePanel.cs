using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzles
{
    public class CodePanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI codeText;
        [SerializeField] private List<PanelButton> buttons;
        [SerializeField] private int maxDigits;

        private string _currentString;

        private bool IsAtMax => _currentString.Length >= maxDigits;
        
        private void Start()
        {
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
//            if ()
            _currentString += digit;
            CheckSolveCondition();
        }

        private void CheckSolveCondition()
        {
            
        }

        private void UpdateText()
        {
            codeText.text = _currentString.PadRight(maxDigits);
        }
    }
}