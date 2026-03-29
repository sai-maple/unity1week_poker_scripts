using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Laughter.Poker.Domain.Master;
using Laughter.Poker.Domain.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Laughter.Poker.View.UI.Custom
{
    /// <summary>
    /// 購入できるスキルのカード
    /// </summary>
    public class SkillCard : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private TextMeshProUGUI _chip;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _sale;
        [SerializeField] private Button _buyButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private SkillMaster _skillMaster;
        private Skill _skill;

        public void Initialize(Skill skill, int chip, Action<Skill, SkillCard> callBack)
        {
            var buyable = skill.Chip <= chip;
            _chip.text = skill.Chip.ToString();
            _chip.color = buyable ? Color.black : Color.red;
            _icon.sprite = _skillMaster.Get(skill);
            _sale.gameObject.SetActive(skill.IsSale);
            _description.text = skill.ToDescription();

            _buyButton.onClick.AddListener(() => callBack(skill, this));
            _buyButton.interactable = buyable;
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public async UniTask PresentAsync(float delay, CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
            _content.localPosition = Vector3.up * 50;
            var move = _content.DOLocalMoveY(0, 0.3f).ToUniTask(cancellationToken: token);
            var fade = _canvasGroup.DOFade(1, 0.3f).ToUniTask(cancellationToken: token);
            await UniTask.WhenAll(move, fade);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public async UniTask DismissAsync(float delay, CancellationToken token)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);

            var move = _content.DOLocalMoveY(50, 0.3f).ToUniTask(cancellationToken: token);
            var fade = _canvasGroup.DOFade(0, 0.3f).ToUniTask(cancellationToken: token);
            await UniTask.WhenAll(move, fade);
            Destroy(gameObject);
        }

        public async UniTask Pick(CancellationToken token)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            await _content.DOLocalMoveY(50, 0.3f).ToUniTask(cancellationToken: token);
        }
    }
}
