using System.Collections.Generic;
using System.Linq;
using Laughter.Poker.Domain.Model;
using UnityEngine;

namespace Laughter.Poker.Domain.Utility
{
    /// <summary>
    /// プレイヤーの強さを評価するクラス
    /// 初期（5枚/1回）15〜25
    /// 中盤	30〜50
    /// 強ビルド	60〜80
    /// 壊れ	90以上
    /// </summary>
    public static class DeckPowerCalculator
    {
        public static float CalculatePower(List<Card> deck, int handSize, int exchangeCount)
        {
            if (deck == null || deck.Count == 0)
                return 0f;

            var numberBias = CalculateNumberBias(deck);
            var suitBias = CalculateSuitBias(deck);

            // --- 各スコア ---
            
            // 手札枚数の影響
            var handScore = handSize * 2.0f;
            // 交換回数の影響
            var exchangeScore = Mathf.Pow(exchangeCount, 1.3f) * 2.5f;
            // 数字とスートの影響
            var numberScore = numberBias * 10.0f;
            var suitScore = suitBias * 8.0f;

            return handScore + exchangeScore + numberScore + suitScore;
        }

        private static float CalculateNumberBias(List<Card> deck)
        {
            var groups = deck.GroupBy(c => c.Number);
            var maxCount = groups.Max(g => g.Count());

            return (float)maxCount / deck.Count;
        }

        private static float CalculateSuitBias(List<Card> deck)
        {
            var groups = deck.GroupBy(c => c.Suit);
            var maxCount = groups.Max(g => g.Count());

            return (float)maxCount / deck.Count;
        }
    }
}
