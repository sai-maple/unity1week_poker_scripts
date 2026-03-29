using System;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Utility;
using Laughter.Poker.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Laughter.Poker.Domain.Model
{
    public class Skill
    {
        public SkillType SkillType { get; private set; }
        public Card CardFrom { get; private set; }
        public Card CardTo { get; private set; }
        public int Chip { get; private set; }
        public bool IsSale { get; }

        public Skill(SkillType skillType, int chip)
        {
            SkillType = skillType;
            Chip = chip;
            CardFrom = new Card(1, Suit.Spade);
            CardFrom = new Card(1, Suit.Spade);
            IsSale = Random.Range(0, 100) < StaticData.SaleRatio;
            if (!IsSale) return;
            Chip = Mathf.FloorToInt(chip * StaticData.SaleDiscount);
        }

        public Skill(SkillType skillType, Card cardFrom, Card cardTo, int chip)
        {
            SkillType = skillType;
            CardFrom = cardFrom;
            CardTo = cardTo;
            Chip = chip;
            IsSale = Random.Range(0, 100) < StaticData.SaleRatio;
            if (!IsSale) return;
            Chip = Mathf.FloorToInt(chip * StaticData.SaleDiscount);
        }

        public string ToDescription()
        {
            return SkillType switch
            {
                SkillType.AddHand => "手札の枚数を増やす",
                SkillType.AddExchange => "手札の交換回数を増やす",
                SkillType.ChangeSuit => $"{LabelExtensions.ToLargeBold(CardFrom.Number)}のスートを{CardTo.Suit.ToMark()}に変える",
                SkillType.ChangeNumber => $"{LabelExtensions.ToLargeBold(CardFrom.Number)}を{LabelExtensions.ToLargeBold(CardTo.Number)}に変える",
                SkillType.LowToMiddle => $"{CardFrom.Suit.ToMark()}の<b>2 ~ 5</b>のを<b>6 ~ 9</b>に変える",
                SkillType.HighToMiddle => $"{CardFrom.Suit.ToMark()}の<b>10 ~ K</b>を<b>6 ~ 9</b>に変える",
                SkillType.MiddleToHigh => $"{CardFrom.Suit.ToMark()}の<b>6 ~ 9</b>を<b>10 ~ K</b>に変える",
                _ => throw new ArgumentOutOfRangeException(nameof(SkillType), SkillType, null)
            };
        }
    }
}
