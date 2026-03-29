using System;
using System.Collections.Generic;
using System.Linq;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.Domain.Service;
using Laughter.Poker.Extensions;
using UnityEngine;

namespace Laughter.Poker.Domain.Utility
{
    public class SkillGenerator
    {
        private readonly int _hand;
        private readonly int _exchangeCount;
        private readonly List<Card> _deck;

        public SkillGenerator(PlayerStatusService statusService)
        {
            _hand = statusService.Hand;
            _exchangeCount = statusService.ExchangeCount;
            _deck = statusService.DeckCard;
        }

        public Skill GetSkill(SkillType skillType)
        {
            switch (skillType)
            {
                case SkillType.AddHand:
                    return new Skill(skillType, 30 * (_hand - 4));
                case SkillType.AddExchange:
                    return new Skill(skillType, 25 * _exchangeCount);
                case SkillType.ChangeSuit:
                    return CalculateChangeSuit(skillType);
                case SkillType.ChangeNumber:
                    return CalculateChangeNumber(skillType);
                case SkillType.LowToMiddle:
                    return CalculateLowToMiddle(skillType);
                case SkillType.HighToMiddle:
                    return CalculateHighToMiddle(skillType);
                case SkillType.MiddleToHigh:
                    return CalculateMiddleToHigh(skillType);
                default:
                    throw new ArgumentOutOfRangeException(nameof(skillType), skillType, null);
            }
        }

        private Skill CalculateChangeSuit(SkillType skillType)
        {
            var card = _deck.GroupBy(c => c.Number).RandomOne().RandomOne();
            var targetSuit = EnumExtensions.GetValues<Suit>().Where(s => s != card.Suit).RandomOne();
            var targetNum = _deck.Count(c => c.Suit == targetSuit);

            var influence = ConvertAsymmetric(targetNum, 0f, 3f);

            // コスト算出（6〜39）おおよそ9前後
            var cost = 9f + influence * 15f;
            var chip = Mathf.RoundToInt(cost);

            return new Skill(skillType, card, new Card(card.Number, targetSuit), chip);
        }

        private Skill CalculateChangeNumber(SkillType skillType)
        {
            var card = _deck.GroupBy(c => c.Number).RandomOne().RandomOne();
            var targetNumber = _deck.Where(s => s.Number != card.Number).RandomOne();
            var targetNum = _deck.Count(c => c.Number == targetNumber.Number);

            var influence = ConvertAsymmetric(targetNum, -0.2f, 2f);

            // コスト算出（6〜39）おおよそ9前後
            var cost = 9f + influence * 15f;
            var chip = Mathf.RoundToInt(cost);

            return new Skill(skillType, card, new Card(targetNumber.Number, card.Suit), chip);
        }

        private Skill CalculateLowToMiddle(SkillType skillType)
        {
            // where入れると
            var card = _deck.RandomOne();
            var targetNum = _deck.Count(c => c.IsMiddle);

            var influence = ConvertAsymmetric(targetNum, -0.2f, 1.5f);

            // コスト算出（6〜23）おおよそ8前後
            var cost = 8f + influence * 10;
            var chip = Mathf.RoundToInt(cost);

            return new Skill(skillType, new Card(2, card.Suit), new Card(6, card.Suit), chip);
        }

        private Skill CalculateHighToMiddle(SkillType skillType)
        {
            // where入れると
            var card = _deck.RandomOne();
            var targetNum = _deck.Count(c => c.IsMiddle);

            var influence = ConvertAsymmetric(targetNum, -0.2f, 1.5f);

            // コスト算出（6〜23）おおよそ8前後
            var cost = 8f + influence * 10;
            var chip = Mathf.RoundToInt(cost);

            return new Skill(skillType, new Card(10, card.Suit), new Card(6, card.Suit), chip);
        }

        private Skill CalculateMiddleToHigh(SkillType skillType)
        {
            // where入れると
            var card = _deck.RandomOne();
            var targetNum = _deck.Count(c => c.IsHigh);

            var influence = ConvertAsymmetric(targetNum, -0.2f, 1.5f);

            // コスト算出（6〜23）おおよそ8前後
            var cost = 8f + influence * 10;
            var chip = Mathf.RoundToInt(cost);

            return new Skill(skillType, new Card(6, card.Suit), new Card(10, card.Suit), chip);
        }

        private float ConvertAsymmetric(int targetNum, float min, float max)
        {
            if (targetNum <= 13f)
            {
                return min - min * (targetNum / 13f);
            }

            return max * Mathf.Clamp((targetNum - 13) / 13f, 0, 2);
        }
    }
}
