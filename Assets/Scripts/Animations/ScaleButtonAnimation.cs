using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Animations
{
    public class ScaleButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float _duration = 0.33f;
        [SerializeField] private float _pressedScale = 0.9f;
        [SerializeField] private Transform _scaleTarget;

        private Coroutine _animation;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            ScalePressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ScaleNormal();
        }

        private void ScalePressed()
        {
            StopAnimation();
            _animation = StartCoroutine(Animation(1f, _pressedScale));
        }

        private void ScaleNormal()
        {
            StopAnimation();
            _animation = StartCoroutine(Animation(_pressedScale, 1f));
        }

        public void StopAnimation()
        {
            if (_animation != null)
            {
                StopCoroutine(_animation);
                _animation = null;
            }
        }

        private IEnumerator Animation(float from, float to)
        {
            var a = Vector3.one * from;
            var b = Vector3.one * to;

            yield return AnimationUtils.Animate(_duration, t =>
            {
                _scaleTarget.localScale = Vector3.LerpUnclamped(a, b, t);
            }, Easing.OutBack);
        }
    }
}