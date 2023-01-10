using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzles
{
    public class RopeColorSetter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Color _enterColor;
        [SerializeField] private Color _exitColor;

        private void SetColor(Color color)
        {
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = color;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetColor(_enterColor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetColor(_exitColor);
        }
    }
}