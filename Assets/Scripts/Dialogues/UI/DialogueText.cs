using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Dialogues.UI
{
    public class DialogueText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textUi;
        [SerializeField] private float letterAppearCooldown;

        [SerializeField] private GameObject interactionHint;

        private bool _isRunning;
        private IEnumerator _coroutine;
        private Action _eventCurrentTextEnded;

        public bool IsRunning => _isRunning;

        public void PutText(string text)
        {
            if (_isRunning)
            {
                StopCoroutine(_coroutine);
                FinishCharactersShowing();
            }

            HideHint();
            textUi.text = text;

            if (text != "")
            {
                _coroutine = ShowCharacterByCharacter();
                StartCoroutine(_coroutine);
            }
        }

        public async Task PutTextAsync(string text, CancellationToken ct)
        {
            PutText(text);
            while (_isRunning && !ct.IsCancellationRequested)
            {
                await Task.Yield();
            }
            
            ShowAll();
            await Task.Yield();
        }

        public void PutText(string text, Action onTextFinishedShowing)
        {
            PutText(text);
            _eventCurrentTextEnded += onTextFinishedShowing;
        }

        public void ShowAll()
        {
            if (_isRunning)
            {
                StopCoroutine(_coroutine);
                FinishCharactersShowing();
            }
            
            ShowHint();
            var textColor = textUi.color;
            textColor.a = 1;
            textUi.color = textColor;
        }

        public void Clear()
        {
            PutText("");
        }

        private void FinishCharactersShowing()
        {
            _eventCurrentTextEnded?.Invoke();
            _eventCurrentTextEnded = null;
            _isRunning = false;
        }
        
        private IEnumerator ShowCharacterByCharacter()
        {
            _isRunning = true;

            DisappearText();
            yield return null;  // wait one frame for TMPro to render text
            
            float nextTimeToShowCharacter = 0.0f;
            
            int characterCount = textUi.textInfo.characterCount;
            for (int i = 0; i < characterCount; i++)
            {
                while (Time.realtimeSinceStartup < nextTimeToShowCharacter)
                {
                    yield return null;
                }
                
                SetCharacterAlpha(i, 255);
                nextTimeToShowCharacter = Time.realtimeSinceStartup + letterAppearCooldown;
            }
            
            ShowHint();

            FinishCharactersShowing();
        }

        private void SetCharacterAlpha(int characterIndex, byte alpha) {
            var charInfo = textUi.textInfo.characterInfo[characterIndex];
            int meshIndex = charInfo.materialReferenceIndex; 
            int vertexIndex = charInfo.vertexIndex;

            Color32[] vertexColors = textUi.textInfo.meshInfo[meshIndex].colors32;

            if(charInfo.isVisible)
            {
                var color = vertexColors[vertexIndex + 0];
                color.a = alpha;
                vertexColors[vertexIndex + 0] = color;
                vertexColors[vertexIndex + 1] = color;
                vertexColors[vertexIndex + 2] = color;
                vertexColors[vertexIndex + 3] = color;
            }

            textUi.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }

        private void DisappearText()
        {
            var textColor = textUi.color;
            textColor.a = 0;
            textUi.color = textColor;
        }

        private void ShowHint()
        {
            if (textUi.text != "")
            {
                interactionHint.SetActive(true);
            }
        }

        private void HideHint()
        {
            interactionHint.SetActive(false);
        }
    }
}
