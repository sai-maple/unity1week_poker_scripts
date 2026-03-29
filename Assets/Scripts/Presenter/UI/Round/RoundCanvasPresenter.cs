using System;
using Cysharp.Threading.Tasks;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.Domain.Service;
using Laughter.Poker.Domain.Utility;
using Laughter.Poker.View.UI.Round;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Laughter.Poker.Presenter.UI.Round
{
    /// <summary>
    /// 手札交換画面のPresenter
    /// </summary>
    [RequireComponent(typeof(RoundCanvas), typeof(LifetimeScope))]
    public class RoundCanvasPresenter : MonoBehaviour
    {
        [SerializeField] private RoundCanvas _roundCanvas;

        private HandService _handService;
        private PlayerStatusService _playerStatusService;
        private ChipService _chipService;
        private RoundService _roundService;
        private KeepBoteHandService _keepBoteHandService;

        private int _changeCount;

        // todo 敵の特徴は表示したい
        // ペア特化、フラッシュ、デッキの強さが自分よりかなり強いボス をアイコン

        [Inject]
        private void Construct(
            HandService handService,
            PlayerStatusService playerStatusService,
            ChipService chipService,
            RoundService roundService,
            KeepBoteHandService keepBoteHandService)
        {
            _handService = handService;
            _playerStatusService = playerStatusService;
            _chipService = chipService;
            _roundService = roundService;
            _keepBoteHandService = keepBoteHandService;
            _changeCount = _playerStatusService.ExchangeCount;
        }

        private void Start()
        {
            _roundCanvas.OnChangeButtonClickedAsObservable()
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => ChangeCard())
                .AddTo(this);

            var foldChip = _roundService.GerFoldChip();
            _roundCanvas.Initialize(_roundService.Round);
            _roundCanvas.SetFold(foldChip, foldChip <= _chipService.CurrentChip);
            _roundCanvas.SetChangeCount(_changeCount, _playerStatusService.ExchangeCount);
            CallAsync().Forget();

            MessageBroker.Default.Receive<HintResult>()
                .Subscribe(hint => _roundCanvas.AddHint(hint.TrendHints))
                .AddTo(this);
        }

        private async UniTaskVoid CallAsync()
        {
            var token = destroyCancellationToken;
            await _roundCanvas.CallAsync(token);
            var (ranks, highCard) = PokerHandEvaluator.EvaluateBest(_handService.Hand);
            _keepBoteHandService.RegisterSelf(ranks, highCard);
        }

        private void ChangeCard()
        {
            // todo SE
            var removedCard = _handService.Discard();
            if (removedCard.Count == 0) return;
            _changeCount--;
            if (_changeCount <= 0) _handService.Lock();
            _roundCanvas.SetChangeCount(_changeCount, _playerStatusService.ExchangeCount);
        }

        private void Reset()
        {
            _roundCanvas = GetComponent<RoundCanvas>();
        }
    }
}
