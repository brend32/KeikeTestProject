using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Game
{
    public class LineStyleSetter : MonoBehaviour
    {
        [SerializeField] private Image _mask;
        [SerializeField] private UILineRendererList _lineRenderer;
        
        public void SetImage(Sprite shape, Color color)
        {
            _mask.sprite = shape;
            _mask.color = color;
            _lineRenderer.color = color;
        }

        public void SetWidth(float width)
        {
            _lineRenderer.LineThickness = width;
        }
    }
}