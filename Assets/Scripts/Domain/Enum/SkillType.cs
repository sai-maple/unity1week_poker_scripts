using System;
using System.Collections.Generic;
using System.Linq;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.Domain.Utility;
using Laughter.Poker.Extensions;
using Random = UnityEngine.Random;

namespace Laughter.Poker.Domain.Enum
{
    // デッキが増えるのはNG おおよそ4枚ずつ変更されていく
    // 特定のカードのスートを別のスートにする
    // 特定の数字のカードを別の数字のカードに変更
    // 「2~5」をLow「6~9」をMiddle「10 ~ 13」をHighとしてスート単位でシフトする
    public enum SkillType
    {
        AddHand,
        AddExchange,
        ChangeSuit,
        ChangeNumber,
        LowToMiddle,
        HighToMiddle,
        MiddleToHigh,
    }

    public static class SkillTypeExtensions
    {
        public static List<SkillType> GetRandomRange(int num, int hand, int exchangeCount, List<Card> deck)
        {
            var skills = new List<SkillType>();
            while (skills.Count < num)
            {
                if (!TryGetRandomOne(hand, exchangeCount, deck, out var skill)) return skills;

                if (skill is SkillType.AddHand or SkillType.AddExchange)
                {
                    if (skills.Contains(skill))
                    {
                        continue;
                    }
                }

                skills.Add(skill);
            }

            return skills;
        }

        private static bool TryGetRandomOne(int hand, int exchangeCount, List<Card> deck, out SkillType type)
        {
            var list = new List<SkillType>();
            var total = EnumExtensions.GetValues<SkillType>().Count();
            type = default;
            while (list.Count < total)
            {
                type = GetRandomOne();
                if (!list.Contains(type)) list.Add(type);
                if (hand >= StaticData.MaxHand && type == SkillType.AddHand)
                {
                    continue;
                }

                if (exchangeCount >= StaticData.MaxExchangeCount && type == SkillType.AddExchange)
                {
                    continue;
                }

                if (type == SkillType.ChangeSuit && deck.GroupBy(c => c.Suit).Count() < 2)
                {
                    continue;
                }

                if (type == SkillType.ChangeNumber && deck.GroupBy(c => c.Number).Count() < 2)
                {
                    continue;
                }

                if (type == SkillType.LowToMiddle && deck.Count(c => c.IsLow) == 0)
                {
                    continue;
                }

                if (type == SkillType.HighToMiddle && deck.Count(c => c.IsHigh) == 0)
                {
                    continue;
                }

                if (type == SkillType.MiddleToHigh && deck.Count(c => c.IsMiddle) == 0)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private static SkillType GetRandomOne()
        {
            var random = Random.Range(0, 100);
            return random switch
            {
                < 8 => SkillType.AddHand,
                < 18 => SkillType.AddExchange,
                < 36 => SkillType.ChangeSuit,
                < 54 => SkillType.ChangeNumber,
                < 69 => SkillType.LowToMiddle,
                < 84 => SkillType.HighToMiddle,
                < 100 => SkillType.MiddleToHigh,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
