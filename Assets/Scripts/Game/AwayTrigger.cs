using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class AwayTrigger : MonoBehaviour
    {
        public UnityEvent OnAwayFirst;
        public UnityEvent OnAwaySecond;
        
        [SerializeField] private float _firstThreshold = 7f;
        [SerializeField] private float _secondThreshold = 14f;

        private int _step;
        private float _timeAway;
        
        private void Update()
        {
            if (Input.touchCount > 0 || Input.anyKeyDown)
            {
                ResetState();
                return;
            }
            
            _timeAway += Time.deltaTime;
            if (_timeAway > _firstThreshold && _step == 0)
            {
                OnAwayFirst?.Invoke();
                _step++;
            }
            else if (_timeAway > _secondThreshold && _step == 1)
            {
                OnAwaySecond?.Invoke();
                ResetState();
            }
        }

        private void ResetState()
        {
            _step = 0;
            _timeAway = 0;
        }
    }
}