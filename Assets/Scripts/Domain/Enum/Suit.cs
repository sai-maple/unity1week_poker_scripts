using System;

namespace Laughter.Poker.Domain.Enum
{
    /// <summary>
    /// トランプのマーク
    /// </summary>
    public enum Suit
    {
        Spade,
        Heart,
        Club,
        Diamond
    }

    public static class SuitExtensions
    {
        public static string ToMark(this Suit self)
        {
            return self switch
            {
                Suit.Spade => @"<sprite name=""suit_0"">",
                Suit.Heart => @"<sprite name=""suit_1"">",
                Suit.Diamond => @"<sprite name=""suit_3"">",
                Suit.Club => @"<sprite name=""suit_2"">",
                _ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
            };
        }
    }
}
