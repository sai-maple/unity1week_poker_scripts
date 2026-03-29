using System;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Laughter.Poker.Domain.Model
{
    /// <summary>
    /// トランプのModel
    /// </summary>
    [Serializable]
    public class Card : IEquatable<Card>
    {
        [SerializeField] private int _number;
        [SerializeField] private Suit _suit;

        public int Number => _number;
        public Suit Suit => _suit;

        public Card(int number, Suit suit)
        {
            if (number is < 0 or > 13)
            {
                Debug.LogError("不正な数字が設定されています");
            }

            _number = number;
            _suit = suit;
        }

        public static Card RandomCard() => new(Random.Range(1, 14), EnumExtensions.GetValues<Suit>().RandomOne());

        public bool IsLow => _number is 2 or 3 or 4 or 5;
        public bool IsMiddle => _number is 6 or 7 or 8 or 9;
        public bool IsHigh => _number is 10 or 11 or 12 or 13;

        public bool Equals(Card other)
        {
            if (other is null) return false;
            return _number == other._number && _suit == other._suit;
        }
    }
}
