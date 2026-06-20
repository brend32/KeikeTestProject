using System.Collections;
using UnityEngine;
using Utils;

namespace Animations
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeAwayAnimation : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.33f;
        
        private CanvasGroup _canvasGroup;
        
        private Coroutine _animation;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

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
            _canvasGroup.alpha = 1;
        }

        private IEnumerator Animation()
        {
            var a = 1;
            var b = 0;

            yield return AnimationUtils.Animate(_duration, t =>
            {
                _canvasGroup.alpha = Mathf.LerpUnclamped(a, b, t);
            }, Easing.QuadOut);
        }
    }
}