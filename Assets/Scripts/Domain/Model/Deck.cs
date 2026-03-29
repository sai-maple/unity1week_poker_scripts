using System;
using System.Collections.Generic;
using UnityEngine;

namespace Laughter.Poker.Domain.Model
{
    [Serializable]
    public class Deck
    {
        [SerializeField] private List<Card> _cards;
        [SerializeField] private int _hand;
        [SerializeField] private int _exchangeCount;

        public int ExchangeCount => _exchangeCount;
        public int Hand => _hand;
        public List<Card> Cards => _cards;

        public Deck(List<Card> cards, int hand, int exchangeCount)
        {
            _cards = cards;
            _hand = hand;
            _exchangeCount = exchangeCount;
        }
    }
}
