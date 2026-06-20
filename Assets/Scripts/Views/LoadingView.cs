using UnityEngine;

namespace Views
{
    public class LoadingView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _content;
        [SerializeField] private GameObject _loading;

        public void Show()
        {
            _content.alpha = 0;
            _content.blocksRaycasts = false;
            _content.interactable = false;
            
            _loading.SetActive(true);
        }
        
        public void Hide()
        {
            _content.alpha = 1;
            _content.blocksRaycasts = true;
            _content.interactable = true;
            
            _loading.SetActive(false);
        }
    }
}