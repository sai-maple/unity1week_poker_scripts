using DG.Tweening;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Master;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.View.UI.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Laughter.Poker.View.Game
{
    public class HintView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _icon;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _hintText;
        [SerializeField] private HintMaster _hintMaster;

        public void Initialize(CardHint cardHint)
        {
            _icon.sprite = _hintMaster.Get(cardHint.Type);
            _hintText.text = cardHint.GetLabel();
            _canvasGroup.alpha = 0;
        }

        public void Initialize(TrendHintType trendHintType)
        {
            _hintText.text = trendHintType.ToLabel();
            _canvasGroup.alpha = 0;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _canvasGroup.DOFade(1, 0.3f);
            AudioView.Instance.PlayOneShot(Sounds.Hint);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _canvasGroup.DOFade(0, 0.3f);
        }
    }
}
