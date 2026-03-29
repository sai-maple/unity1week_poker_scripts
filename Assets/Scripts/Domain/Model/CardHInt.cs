using System;
using System.Collections.Generic;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Extensions;

namespace Laughter.Poker.Domain.Model
{
    public class CardHint
    {
        public int Index;
        public Card Card;
        public CardHintType Type;

        public string GetLabel()
        {
            return Type switch
            {
                CardHintType.Number => $"数字は{LabelExtensions.ToLargeBold(Card.Number)}",
                CardHintType.Suit => $"スートは{Card.Suit.ToMark()}",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public class HintResult
    {
        public readonly List<CardHint> CardHints = new();
        public List<TrendHintType> TrendHints = new();
    }
}
