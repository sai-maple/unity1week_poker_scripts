using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.Domain.Service;
using Laughter.Poker.Domain.Utility;
using Laughter.Poker.View.Game;
using UniRx;
using UnityEngine;
using VContainer;

namespace Laughter.Poker.Presenter.Game
{
    /// <summary>
    /// CPUの手札交換交換ロジック
    /// </summary>
    public class OpponentBrain : MonoBehaviour
    {
        [SerializeField] private HandCard _handCard;
        private PlayerStatusService _playerStatusService;
        private KeepBoteHandService _keepBoteHandService;
        private DeckService _deckService;
        private HandService _handService;

        private int _changeCount;
        private IDisposable _disposable;

        [Inject]
        public void Construct(
            PlayerStatusService playerStatusService,
            KeepBoteHandService keepBoteHandService,
            DeckService deckService,
            HandService handService)
        {
            _playerStatusService = playerStatusService;
            _keepBoteHandService = keepBoteHandService;
            _deckService = deckService;
            _handService = handService;
            _changeCount = _playerStatusService.ExchangeCount;
            Debug.Log("CPU思考開始");
            _disposable = _handService.OnAddCardSubject()
                .Subscribe(_ => { Exchange().Forget(); })
                .AddTo(this);
        }

        private async UniTask Exchange()
        {
            var token = destroyCancellationToken;
            // カーロオブジェクト生成待機
            await UniTask.Yield();
            _handCard.LockAll();

            // アニメーションの待機
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

            if (_changeCount <= 0)
            {
                Debug.Log("CPU思考終了");
                var (ranks, highCard) = PokerHandEvaluator.EvaluateBest(_handService.Hand);
                _keepBoteHandService.RegisterOpponent(ranks, highCard);
                _disposable?.Dispose();
                CreateHints();
                return;
            }

            var removedFlag = await ChooseBestExchange(_handService.Hand, _deckService.Deck);
            Debug.Log($"CPU交換 {removedFlag}");
            _changeCount--;
            _handService.DiscordByBrain(removedFlag);
        }

        private async UniTask<int> ChooseBestExchange(IReadOnlyList<Card> hand, IReadOnlyList<Card> deck)
        {
            var handSize = hand.Count;

            var patterns = GenerateExchangePatterns(handSize);

            double bestScore = -1;
            var bestPattern = 0b0;

            var count = 0;
            foreach (var p in patterns)
            {
                // todo シミュレーション回数(CPUの強さ) と枝狩り
                var score = EvaluatePattern(hand, deck, p, 200);

                if (!(score > bestScore)) continue;
                bestScore = score;
                bestPattern = p;
                count++;
                if (count % 100 == 0)
                {
                    await UniTask.Yield();
                }
            }

            return bestPattern;
        }

        private static double EvaluatePattern(
            IReadOnlyList<Card> hand,
            IReadOnlyList<Card> deck,
            int pattern,
            int simulationCount)
        {
            var handSize = hand.Count;

            var replaceIndexes = new List<int>();

            for (var i = 0; i < handSize; i++)
            {
                if ((pattern & (1 << i)) != 0)
                    replaceIndexes.Add(i);
            }

            var replaceCount = replaceIndexes.Count;

            if (replaceCount == 0) return GetHandScore(hand);

            double totalScore = 0;

            for (var s = 0; s < simulationCount; s++)
            {
                var newHand = new List<Card>(hand);
                var tempDeck = new List<Card>(deck);

                // シャッフル
                tempDeck = tempDeck.OrderBy(_ => Guid.NewGuid()).ToList();

                // 疑似ドロー処理
                for (var r = 0; r < replaceCount; r++)
                {
                    newHand[replaceIndexes[r]] = tempDeck[r];
                }

                totalScore += GetHandScore(newHand);
            }

            // 期待値評価
            return totalScore / simulationCount;
        }


        /// <summary>
        /// 全交換パターンの列挙
        /// </summary>
        /// <param name="handSize"></param>
        /// <returns></returns>
        private List<int> GenerateExchangePatterns(int handSize)
        {
            var patterns = new List<int>();

            var max = 1 << handSize;

            for (var i = 0; i < max; i++)
            {
                var count = 0;
                // 残りデッキ枚数を超える選択を禁止する
                var temp = i;
                while (temp != 0)
                {
                    temp &= temp - 1;
                    count++;
                }

                // 交換枚数の上限を4枚にする
                if (count > 4) continue;
                patterns.Add(i);
            }

            return patterns;
        }

        /// <summary>
        /// 手札の役を返します
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        private static int GetHandScore(IReadOnlyList<Card> hand)
        {
            return (int)PokerHandEvaluator.EvaluateBest(hand).Ranks.Highest();
        }

        private void CreateHints()
        {
            var hint = HintGenerator.Generate(_handService.Hand);
            _handCard.AddHint(hint.CardHints);
            MessageBroker.Default.Publish(hint);
        }
    }
}
