using System;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Model;
using UnityEngine;

namespace Laughter.Poker.Domain.Master
{
    /// <summary>
    /// Cardに対応したSpriteを返します
    /// </summary>
    [CreateAssetMenu(fileName = nameof(CardMaster), menuName = "Master/" + nameof(CardMaster))]
    public class CardMaster : ScriptableObject
    {
        [SerializeField] private Sprite[] _cardSpades;
        [SerializeField] private Sprite[] _cardHearts;
        [SerializeField] private Sprite[] _cardDiamonds;
        [SerializeField] private Sprite[] _cardClubs;
        [SerializeField] private Sprite _cardBack;

        public Sprite Get(Card card)
        {
            return Get(card.Number, card.Suit);
        }

        public Sprite Get(int number, Suit suit)
        {
            return suit switch
            {
                Suit.Spade => _cardSpades[number],
                Suit.Heart => _cardHearts[number],
                Suit.Diamond => _cardDiamonds[number],
                Suit.Club => _cardClubs[number],
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public Sprite Back() => _cardBack;
    }
}
