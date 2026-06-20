using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class HelpPathItem : MonoBehaviour
    {
        public UnityEvent OnAnimationPrepare;
        public UnityEvent OnAnimationPlay;
        
        [SerializeField] private Image _image;

        public void SetImage(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public void PrepareAnimation()
        {
            OnAnimationPrepare?.Invoke();
        }
        
        public void PlayAnimation()
        {
            OnAnimationPlay?.Invoke();
        }
    }
}