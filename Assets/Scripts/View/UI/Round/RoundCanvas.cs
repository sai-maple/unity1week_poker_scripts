using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Master;
using Laughter.Poker.View.UI.Common;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Laughter.Poker.View.UI.Round
{
    /// <summary>
    /// 手札交換中のView
    /// </summary>
    public class RoundCanvas : ViewCommon
    {
        [SerializeField] private Button _deckButton;
        [SerializeField] private Button _callButton;
        [SerializeField] private Button _changeButton;
        [SerializeField] private Button _foldButton;
        [SerializeField] private TextMeshProUGUI _changeCount;
        [SerializeField] private TextMeshProUGUI _foldChip;
        [SerializeField] private TextMeshProUGUI _round;
        [SerializeField] private Transform _hintContainer;
        [SerializeField] private HintMaster _hintMaster;

        public IObservable<Unit> OnDeckButtonClickedAsObservable() => _deckButton.OnClickAsObservable();
        public IObservable<Unit> OnChangeButtonClickedAsObservable() => _changeButton.OnClickAsObservable();

        public UniTask FoldAsync(CancellationToken token) => _foldButton.OnClickAsync(token);
        public UniTask CallAsync(CancellationToken token) => _callButton.OnClickAsync(token);

        public void Initialize(int round)
        {
            _round.text = $"ROUND {round:00}";
        }

        public void SetFold(int foldChip, bool available)
        {
            _foldChip.text = foldChip.ToString();
            _foldChip.color = available ? Color.gold : Color.softRed;
            _foldButton.interactable = available;
        }

        public void SetChangeCount(int changeCount, int total)
        {
            var isActive = changeCount > 0;
            _changeCount.text = $"{changeCount}/{total}";
            _changeCount.color = isActive ? Color.gold : Color.softRed;
            _changeButton.interactable = isActive;
        }

        public void AddHint(List<TrendHintType> trendHints)
        {
            foreach (var trendHint in trendHints)
            {
                var hint = Instantiate(_hintMaster.Get(trendHint), _hintContainer);
                hint.Initialize(trendHint);
            }
        }
    }
}
