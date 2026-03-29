using System;
using System.Collections.Generic;
using System.Linq;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Model;
using UnityEngine;
using Random = System.Random;

namespace Laughter.Poker.Domain.Utility
{
    public static class HintGenerator
    {
        private static readonly Random Random = new();

        public static HintResult Generate(IReadOnlyList<Card> hand)
        {
            var result = new HintResult();

            // --- カードヒント ---
            var selectedCards = hand.Select((c, i) => (c, i))
                .OrderBy(_ => Random.Next())
                .Take(Mathf.CeilToInt(hand.Count / 2f))
                .ToList();

            foreach (var card in selectedCards)
            {
                var type = (CardHintType)Random.Next(0, 2);

                result.CardHints.Add(new CardHint
                {
                    Index = card.i,
                    Card = card.c,
                    Type = type
                });
            }

            // --- 傾向ヒント候補生成 ---
            var candidates = GetTrendCandidates(hand);

            // 最大2つ選択
            result.TrendHints = candidates
                .OrderBy(_ => Random.Next())
                .Take(Math.Min(2, candidates.Count))
                .ToList();

            return result;
        }

        private static List<TrendHintType> GetTrendCandidates(IReadOnlyList<Card> hand)
        {
            var result = new List<TrendHintType>();

            // --- スート集計 ---
            var suitGroups = hand.GroupBy(c => c.Suit).ToList();
            var maxSuit = suitGroups.Max(g => g.Count());

            if (maxSuit >= 3)
                result.Add(TrendHintType.SameSuitMany);

            if (suitGroups.Count >= 4)
                result.Add(TrendHintType.SuitDiverse);

            if (maxSuit < 5)
                result.Add(TrendHintType.NotFlush);

            // --- 数字集計 ---
            var numberGroups = hand.GroupBy(c => c.Number).ToList();
            var maxNumber = numberGroups.Max(g => g.Count());

            if (maxNumber >= 2)
                result.Add(TrendHintType.SameNumberMany);

            if (numberGroups.Count >= hand.Count - 1)
                result.Add(TrendHintType.NumberDiverse);

            // --- 色 ---
            var red = hand.Count(c => c.Suit is Suit.Heart or Suit.Diamond);
            var black = hand.Count - red;

            if (red >= hand.Count / 2 + 1)
                result.Add(TrendHintType.RedMany);

            if (black >= hand.Count / 2 + 1)
                result.Add(TrendHintType.BlackMany);

            // --- 範囲 ---
            var low = hand.Count(c => c.Number is >= 2 and <= 5);
            var mid = hand.Count(c => c.Number is >= 6 and <= 9);
            var high = hand.Count(c => c.Number is >= 10 and <= 13);

            var threshold = hand.Count / 2 + 1;

            if (low >= threshold)
                result.Add(TrendHintType.LowMany);

            if (mid >= threshold)
                result.Add(TrendHintType.MiddleMany);

            if (high >= threshold)
                result.Add(TrendHintType.HighMany);

            return result;
        }
    }
}
