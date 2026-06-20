using System.Collections;
using UnityEngine;
using Utils;

namespace Animations
{
    public class ScaleUpAnimation : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.33f;
        [SerializeField] private float _targetScale = 1f;
        
        private Coroutine _animation;

        public void Play()
        {
            PlayCoroutine();
        }
        
        public Coroutine PlayCoroutine()
        {
            Stop();
            return _animation = StartCoroutine(Animation());
        }

        public void Stop()
        {
            if (_animation != null)
            {
                StopCoroutine(_animation);
                _animation = null;
            }
        }

        public void JumpStart()
        {
            Stop();
            transform.localScale = AnimationUtils.AlmostZero;
        }

        private IEnumerator Animation()
        {
            var a = AnimationUtils.AlmostZero;
            var b = Vector3.one * _targetScale;

            yield return AnimationUtils.Animate(_duration, t =>
            {
                transform.localScale = Vector3.LerpUnclamped(a, b, t);
            }, Easing.QuadOut);
        }
    }
}