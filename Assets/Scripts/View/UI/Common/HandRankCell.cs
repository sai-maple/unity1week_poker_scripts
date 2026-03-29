using DG.Tweening;
using Laughter.Poker.Domain.Enum;
using TMPro;
using UnityEngine;

namespace Laughter.Poker.View.UI.Common
{
    /// <summary>
    /// 役一覧のセル
    /// </summary>
    public class HandRankCell : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _rank;
        [SerializeField] private TextMeshProUGUI _baseChip;
        [SerializeField] private TextMeshProUGUI _you;
        [SerializeField] private TextMeshProUGUI _opponent;
        [SerializeField] private RectTransform _content;

        // todo スキルで獲得チップを強化

        public void Initialize(HandRank handRank)
        {
            _rank.text = handRank.ToName();
            _baseChip.text = $"{handRank.ToChip()}";
            _you.enabled = false;
            _opponent.enabled = false;
        }

        /// <summary>
        /// 手札の役のうち最高ランクの役を強調
        /// </summary>
        /// <param name="isSelf"></param>
        /// <param name="enable"></param>
        public void HighRank(bool isSelf, bool enable)
        {
            if (isSelf)
            {
                _you.enabled = enable;
                var endValue = enable ? -30f : 0f;
                _content.DOAnchorPosX(endValue, 0.3f);
            }
            else
            {
                var endValue = enable ? -30f : _content.anchoredPosition.x;
                _content.DOAnchorPosX(endValue, 0.3f);
                _opponent.enabled = enable;
            }
        }

        /// <summary>
        /// チップを獲得する時、成立している役を強調表示する
        /// </summary>
        public void Rise()
        {
            _content.DOAnchorPosX(0, 0);
            _content.DOScale(1.2f, 0.3f);
        }
        
        public void Fall()
        {
            _content.DOScale(1f, 0.3f);
        }
    }
}
