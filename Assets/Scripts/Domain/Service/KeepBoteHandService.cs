using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Model;
using UnityEngine;

namespace Laughter.Poker.Domain.Service
{
    public class KeepBoteHandService
    {
        public List<HandRank> SelfRanks { get; private set; }
        private Card _selfHighCard;
        private bool _isRegisterSelf;

        private List<HandRank> _opponentRanks;
        private Card _opponentHighCard;
        private bool _isRegisterOpponent;

        public HandRank OpponentHand => _opponentRanks.Highest();

        public UniTask RegisterAsync(CancellationToken token) =>
            UniTask.WaitUntil(() => _isRegisterOpponent & _isRegisterSelf, cancellationToken: token);

        public void RegisterSelf(List<HandRank> selfRanks, Card selfHighCard)
        {
            SelfRanks = selfRanks;
            _selfHighCard = selfHighCard;
            _isRegisterSelf = true;
        }

        public void RegisterOpponent(List<HandRank> opponentRanks, Card opponentHighCard)
        {
            _opponentRanks = opponentRanks;
            _opponentHighCard = opponentHighCard;
            _isRegisterOpponent = true;
        }

        public GameResult GetResult()
        {
            var selfRank = SelfRanks.Highest();
            var opponentRank = _opponentRanks.Highest();
            if (selfRank > opponentRank) return GameResult.Win;
            if (selfRank == opponentRank)
            {
                var result = CompareCards(_selfHighCard, _opponentHighCard);
                Debug.Log($"self : {_selfHighCard.Suit}の{_selfHighCard.Number}, opponent : {_opponentHighCard.Suit}の{_opponentHighCard.Number}で勝負します。\n{result}");

                return result;
            }

            return GameResult.Lose;
        }

        private static GameResult CompareCards(Card selfCard, Card opponentCard)
        {
            if (selfCard.Number == 1 && opponentCard.Number != 1) return GameResult.Win;
            if (opponentCard.Number == 1 && selfCard.Number != 1) return GameResult.Lose;
            if (selfCard.Number > opponentCard.Number) return GameResult.Win;
            if (selfCard.Number < opponentCard.Number) return GameResult.Lose;
            if (selfCard.Suit < opponentCard.Suit) return GameResult.Win;
            if (selfCard.Suit > opponentCard.Suit) return GameResult.Lose;
            return GameResult.Draw;
        }
    }
}
