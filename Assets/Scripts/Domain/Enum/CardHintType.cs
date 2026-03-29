using System;
using Laughter.Poker.Extensions;

namespace Laughter.Poker.Domain.Enum
{
    /// <summary>
    /// 手札の具体的なヒント
    /// </summary>
    public enum CardHintType
    {
        Number,
        Suit
    }

    /// <summary>
    /// 手札全体の傾向を示すヒント
    /// </summary>
    public enum TrendHintType
    {
        SameSuitMany,
        SameNumberMany,
        SuitDiverse,
        NumberDiverse,
        RedMany,
        BlackMany,
        LowMany,
        MiddleMany,
        HighMany,
        NotFlush
    }

    public static class TrendHintTypeExtensions
    {
        public static string ToLabel(this TrendHintType self)
        {
            return self switch
            {
                TrendHintType.SameSuitMany => "同じスートが多そうだ",
                TrendHintType.SameNumberMany => "同じ数字のカードが多そうだ",
                TrendHintType.SuitDiverse => "スートがバラバラに見える",
                TrendHintType.NumberDiverse => "数字がバラバラに見える",
                TrendHintType.RedMany => "赤いカードが多そうだ",
                TrendHintType.BlackMany => "黒いカードが多そうだ",
                TrendHintType.LowMany => $"{LabelExtensions.ToLargeBold("2 ~ 5")}が多そうだ",
                TrendHintType.MiddleMany => $"{LabelExtensions.ToLargeBold("6 ~ 9")}が多そうだ",
                TrendHintType.HighMany => $"{LabelExtensions.ToLargeBold("10 ~ K")}が多そうだ",
                TrendHintType.NotFlush => "フラッシュではないだろう",
                _ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
            };
        }
    }
}
