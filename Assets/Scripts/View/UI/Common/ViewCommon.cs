using DG.Tweening;
using UnityEngine;

namespace Laughter.Poker.View.UI.Common
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ViewCommon : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        public bool IsLock
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = !value;
        }
        
        private void Awake()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }

        public void Present()
        {
            _canvasGroup.DOFade(1, 0.3f);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Dismiss()
        {
            _canvasGroup.DOFade(0, 0.3f);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        private void Reset()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
    }
}
