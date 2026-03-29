using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.View.UI.Audio;
using Laughter.Poker.View.UI.Common;
using Laughter.Poker.View.UI.Effect;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Laughter.Poker.View.UI.Result
{
    public class ResultCanvas : ViewCommon
    {
        [SerializeField] private HandRankView _handRankView;
        [SerializeField] private PlayableDirector _chipIncrease;
        [SerializeField] private PlayableDirector _chipDecrease;
        [SerializeField] private PlayableDirector _lose;
        [SerializeField] private PlayableDirector _drow;
        [SerializeField] private TextMeshProUGUI _chipCount;
        [SerializeField] private TextMeshProUGUI _increaseChip;
        [SerializeField] private TextMeshProUGUI _foldDescription;
        [SerializeField] private Button _nextButton;

        private int _chip;

        public void Initialize(int chip)
        {
            _chip = chip;
            _chipCount.text = chip.ToString();
            _nextButton.interactable = false;
        }

        /// <summary>
        ///  チップ増加演出
        /// </summary>
        public async UniTask ChipIncreaseAsync(List<HandRank> selfRanks, CancellationToken token)
        {
            _increaseChip.color = Color.chocolate;
            foreach (var rank in selfRanks)
            {
                Debug.Log($"役成立報酬 {rank}");
                // todo 成立している役のカードを持ち上げたい
                _handRankView.Rise(rank);
                _increaseChip.text = $"+{rank.ToChip()}";
                _chipIncrease.Play();
                _chip += rank.ToChip();
                var from = int.Parse(_chipCount.text);
                _chipCount.DOCounter(from, _chip, 1f).SetEase(Ease.Linear);
                ChipGainView.Instance.PlayEffect(rank.ToChip());
                AudioView.Instance.PlayOneShot(Sounds.ChipGain);
                AudioView.Instance.PlayOneShot(Sounds.Win);
                await UniTask.Delay(TimeSpan.FromSeconds(_chipIncrease.duration), cancellationToken: token);
                _handRankView.Fall(rank);
            }

            _nextButton.interactable = true;
            await _nextButton.OnClickAsync(token);
        }

        /// <summary>
        /// チップ現象演出
        /// </summary>
        public async UniTask ChipDecreaseAsync(int foldChip, CancellationToken token)
        {
            _foldDescription.text = $"チップ<color=red><size=50>{foldChip}</size></color>を支払って勝負から降りました";
            _increaseChip.text = $"-{foldChip}";
            _increaseChip.color = Color.softRed;
            _chipDecrease.Play();
            _chip -= foldChip;
            var from = int.Parse(_chipCount.text);
            _chipCount.DOCounter(from, _chip, 1f).SetEase(Ease.Linear);
            // todo SE ChipDecrease
            AudioView.Instance.PlayOneShot(Sounds.ChipGain);
            await UniTask.Delay(TimeSpan.FromSeconds(_chipDecrease.duration), cancellationToken: token);
            _nextButton.interactable = true;
            await _nextButton.OnClickAsync(token);
        }

        /// <summary>
        /// 敗北演出
        /// </summary>
        /// <param name="token"></param>
        public async UniTask LoseAsync(CancellationToken token)
        {
            AudioView.Instance.PlayOneShot(Sounds.Defeat);
            _lose.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(_lose.duration), cancellationToken: token);
        }

        public async UniTask DrawAsync(CancellationToken token)
        {
            AudioView.Instance.PlayOneShot(Sounds.DrawResult);
            _drow.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(_drow.duration), cancellationToken: token);
        }
    }
}
