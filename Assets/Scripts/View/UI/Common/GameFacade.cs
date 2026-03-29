using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.View.Game;
using Laughter.Poker.View.UI.Deck;
using Laughter.Poker.View.UI.Result;
using Laughter.Poker.View.UI.Round;
using UniRx;
using UnityEngine;

namespace Laughter.Poker.View.UI.Common
{
    /// <summary>
    /// ゲーム中のCanvas全体を管理するView
    /// </summary>
    public class GameFacade : MonoBehaviour
    {
        [SerializeField] private RoundCanvas _roundCanvas;
        [SerializeField] private DeckCanvas _deckCanvas;
        [SerializeField] private HandRankView _handRankView;
        [SerializeField] private HandCard _selfHandCard;
        [SerializeField] private HandCard _opponentHandCard;
        [SerializeField] private ResultCanvas _resultCanvas;

        public void Initialize(Camera mainCamera, int chip)
        {
            foreach (var canvas in GetComponentsInChildren<Canvas>())
            {
                canvas.worldCamera = mainCamera;
            }

            _resultCanvas.Initialize(chip);
        }

        public async UniTask<bool> StartRoundAsync(CancellationToken token)
        {
            _roundCanvas.Present();
            _handRankView.Present();
            var disposable = _roundCanvas.OnDeckButtonClickedAsObservable()
                .Subscribe(_ => _deckCanvas.Present());
            var result = await UniTask.WhenAny(_roundCanvas.CallAsync(token), _roundCanvas.FoldAsync(token));
            disposable.Dispose();
            if (token.IsCancellationRequested) return false;
            _roundCanvas.Dismiss();
            return result == 0;
        }

        public async UniTask CallAsync(HandRank opponentRank, CancellationToken token)
        {
            Debug.Log("手札公開");
            // 手札の公開
            var openSelf = _selfHandCard.OpenHandAsync(true, token);
            var openOpponent = _opponentHandCard.OpenHandAsync(false, token);
            await UniTask.WhenAll(openSelf, openOpponent);
            _handRankView.SetHighRank(false, opponentRank);
            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
        }

        public async UniTask FoldAsync(CancellationToken token)
        {
            Debug.Log("Fold");
            // Fold演出
            await _selfHandCard.FoldAsync(token);
            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
        }

        public async UniTask ResultAsync(
            GameResult result,
            List<HandRank> handRanks,
            int foldChip,
            CancellationToken token)
        {
            // 勝敗に応じた演出
            _resultCanvas.Present();
            var task = result switch
            {
                GameResult.Win => _resultCanvas.ChipIncreaseAsync(handRanks, token),
                GameResult.Lose => _resultCanvas.LoseAsync(token),
                GameResult.Draw => _resultCanvas.DrawAsync(token),
                GameResult.Fold => _resultCanvas.ChipDecreaseAsync(foldChip, token),
                _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
            };

            await task;
            _handRankView.Dismiss();
            _resultCanvas.Dismiss();
        }

        public UniTask DestroyHandCardAsync(CancellationToken token)
        {
            _handRankView.Dismiss();
            // カードの破棄
            var selfDestroy = _selfHandCard.DestroyHandCardAsync(Vector3.right * 10, token);
            var opponentDestroy = _opponentHandCard.DestroyHandCardAsync(Vector3.right * 10, token);
            return UniTask.WhenAll(selfDestroy, opponentDestroy);
        }
    }
}
