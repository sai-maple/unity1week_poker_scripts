using System;
using System.Collections.Generic;
using System.Linq;

namespace Laughter.Poker.Domain.Enum
{
    /// <summary>
    /// ポーカー役一覧
    /// </summary>
    public enum HandRank
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }

    public static class HandRankExtension
    {
        public static int ToChip(this HandRank rank)
        {
            return rank switch
            {
                HandRank.RoyalFlush => 300,
                HandRank.StraightFlush => 200,
                HandRank.FourOfAKind => 100,
                HandRank.FullHouse => 50,
                HandRank.Flush => 25,
                HandRank.Straight => 20,
                HandRank.ThreeOfAKind => 15,
                HandRank.TwoPair => 10,
                HandRank.OnePair => 5,
                HandRank.HighCard => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null)
            };
        }

        public static string ToName(this HandRank rank)
        {
            return rank switch
            {
                HandRank.RoyalFlush => "ROYAL FLASH",
                HandRank.StraightFlush => "STRAIGHT FLUSH",
                HandRank.FourOfAKind => "FOUR OF A KIND",
                HandRank.FullHouse => "FULL HOUSE",
                HandRank.Flush => "FLUSH",
                HandRank.Straight => "STRAIGHT",
                HandRank.ThreeOfAKind => "THREE OF A KIND",
                HandRank.TwoPair => "TWO PAIR",
                HandRank.OnePair => "ONE PAIR",
                HandRank.HighCard => "HIGH CARD",
                _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null)
            };
        }

        public static HandRank Highest(this List<HandRank> ranks)
        {
            return ranks.Max();
        }

        public static int Const(this HandRank rank, int current)
        {
            return rank switch
            {
                HandRank.HighCard => 5 + current * 5,
                HandRank.OnePair => 5 + current * 5,
                HandRank.TwoPair => 5 + current * 5,
                HandRank.ThreeOfAKind => 5 + current * 5,
                HandRank.Straight => 5 + current * 5,
                HandRank.Flush => 5 + current * 5,
                HandRank.FullHouse => 5 + current * 5,
                HandRank.FourOfAKind => 5 + current * 5,
                HandRank.StraightFlush => 5 + current * 5,
                HandRank.RoyalFlush => 5 + current * 5,
                _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null)
            };
        }
    }
}
