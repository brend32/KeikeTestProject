using System;
using System.Collections;
using Animations;
using UnityEngine;
using Utils;
using Zenject;

namespace Game
{
    public class PointerHelpAnimation : MonoBehaviour
    {
        [SerializeField] private FadeAwayAnimation _animation;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _duration = 2;
        [SerializeField] private float _fadeDelay = 0.4f;
        
        [Inject] private DrawDraggingZone _draggingZone;

        private Coroutine _coroutine;

        private void OnEnable()
        {
            CancelAnimation();
            _draggingZone.OnStartedDragging.AddListener(CancelAnimation);
        }
        
        private void OnDisable()
        {
            if (_draggingZone)
            {
                _draggingZone.OnStartedDragging.RemoveListener(CancelAnimation);
            }
        }

        public void CancelAnimation()
        {
            _animation.Stop();
            _canvasGroup.alpha = 0;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
        
        [EasyButtons.Button]
        public void Play()
        {
            CancelAnimation();
            _canvasGroup.alpha = 1;
            _coroutine = StartCoroutine(Animation());
        }

        private IEnumerator Animation()
        {
            yield return AnimationUtils.Animate(_duration, t =>
            {
                transform.position = _draggingZone.GetWorldPositionOnPath(t);
            });
            yield return new WaitForSeconds(_fadeDelay);
            yield return _animation.PlayCoroutine();
        }
    }
}