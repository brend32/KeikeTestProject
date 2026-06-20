using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameStyleSetter : MonoBehaviour
    {
        [SerializeField] private Image _help;
        [SerializeField] private LineStyleSetter[] _lineRenderers;

        public void SetImage(Sprite sprite, Color color)
        {
            _help.sprite = sprite;
            foreach (var lineStyleSetter in _lineRenderers)
            {
                lineStyleSetter.SetImage(sprite, color);
            }
        }

        public void SetWidth(float width)
        {
            foreach (var lineStyleSetter in _lineRenderers)
            {
                lineStyleSetter.SetWidth(width);
            }
        }
    }
}