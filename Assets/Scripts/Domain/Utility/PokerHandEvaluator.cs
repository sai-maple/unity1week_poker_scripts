using System;
using System.Collections.Generic;
using System.Linq;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Model;

namespace Laughter.Poker.Domain.Utility
{
    public static class PokerHandEvaluator
    {
        /// <summary>
        /// 手札の中の成立したすべての役と、成立した役の配列を返します
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static (List<HandRank> Ranks, Card HighCard) EvaluateBest(IReadOnlyList<Card> cards)
        {
            if (cards == null || cards.Count < 5)
            {
                throw new ArgumentException("最低5枚必要です");
            }

            var result = new List<HandRank> { HandRank.HighCard };
            var highestRank = HandRank.HighCard;
            var highCard = HighCard(cards.ToList());

            foreach (var combination in GetCombinations(cards, 5))
            {
                var handRanks = Evaluate5(combination);
                foreach (var (hand, rankCards) in handRanks)
                {
                    if (!result.Contains(hand)) result.Add(hand);
                    if (hand > highestRank)
                    {
                        highestRank = hand;
                        highCard = HighCard(rankCards);
                    }
                    else if (hand == highestRank)
                    {
                        rankCards.Add(highCard);
                        highCard = HighCard(rankCards);
                    }
                }
            }

            result = result.OrderBy(handRank => handRank).ToList();
            return (result, highCard);
        }

        /// <summary>
        /// 手札の内5枚の組み合わせで成立している役を返します
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private static List<(HandRank, List<Card>)> Evaluate5(List<Card> cards)
        {
            var result = new List<(HandRank, List<Card>)>();
            var numbers = cards.Select(c => c.Number).OrderBy(n => n).ToList();

            var groups = cards
                .GroupBy(c => c.Number)
                .Select(g => (Count: g.Count(), Cards: g.ToList()))
                .OrderByDescending(c => c.Count)
                .ToList();

            var isFlush = cards.All(c => c.Suit == cards[0].Suit);
            var isStraight = IsStraight(numbers);

            var isRoyal = numbers.SequenceEqual(new List<int> { 10, 11, 12, 13, 1 });

            if (isStraight && isFlush && isRoyal) result.Add((HandRank.RoyalFlush, cards));

            if (isStraight && isFlush) result.Add((HandRank.StraightFlush, cards));

            if (groups[0].Count >= 4) result.Add((HandRank.FourOfAKind, groups[0].Cards));

            if (groups.Count >= 2 && groups[0].Count >= 3 && groups[1].Count >= 2)
                result.Add((HandRank.FullHouse, groups[0].Cards));

            if (isFlush) result.Add((HandRank.Flush, cards));

            if (isStraight) result.Add((HandRank.Straight, cards));

            if (groups[0].Count >= 3) result.Add((HandRank.ThreeOfAKind, groups[0].Cards));

            if (groups.Count >= 2 && groups[0].Count >= 2 && groups[1].Count >= 2)
                result.Add((HandRank.TwoPair, groups[0].Cards));

            if (groups[0].Count >= 2) result.Add((HandRank.OnePair, groups[0].Cards));

            return result;
        }

        private static bool IsStraight(List<int> numbers)
        {
            var distinct = numbers.Distinct().ToList();
            if (distinct.Count != 5) return false;

            // 通常
            if (IsSequential(distinct)) return true;

            // Aを14扱い
            var highAce = distinct.Select(n => n == 1 ? 14 : n).OrderBy(n => n).ToList();
            return IsSequential(highAce);
        }

        /// <summary>
        /// ストレート判定 手札の数値が5回連続するかを判定します
        /// </summary>
        /// <param name="nums">ソート済みの手札の数値</param>
        /// <returns></returns>
        private static bool IsSequential(List<int> nums)
        {
            for (var i = 0; i < nums.Count - 1; i++)
            {
                if (nums[i] + 1 != nums[i + 1])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 手札全てのカートからcount枚の組み合わせ全てを列挙します
        /// </summary>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static IEnumerable<List<Card>> GetCombinations(IReadOnlyList<Card> list, int count)
        {
            var n = list.Count;
            var indices = Enumerable.Range(0, count).ToArray();

            while (true)
            {
                yield return indices.Select(i => list[i]).ToList();

                int i;
                for (i = count - 1; i >= 0; i--)
                {
                    if (indices[i] != i + n - count)
                        break;
                }

                if (i < 0)
                    yield break;

                indices[i]++;

                for (var j = i + 1; j < count; j++)
                {
                    indices[j] = indices[j - 1] + 1;
                }
            }
        }

        private static Card HighCard(List<Card> cards)
        {
            var max = cards.Any(c => c.Number == 1) ? 1 : cards.Max(c => c.Number);
            var highCards = cards.Where(m => m.Number == max);
            return highCards.OrderBy(c => c.Suit).First();
        }
    }
}
