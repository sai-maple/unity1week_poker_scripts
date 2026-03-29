using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Laughter.Poker.Domain.Model;
using UnityEngine;

namespace Laughter.Poker.Domain.Service
{
    /// <summary>
    /// プレイヤーが所持するカードとスキルを管理するService
    /// </summary>
    public class PlayerStatusService
    {
        public int Hand { get; private set; }
        public int ExchangeCount { get; private set; }

        public List<Card> DeckCard { get; }

        public PlayerStatusService(string json)
        {
            var deck = JsonUtility.FromJson<Deck>(json);
            DeckCard = deck.Cards;
            Hand = deck.Hand;
            ExchangeCount = deck.ExchangeCount;
        }

        /// <summary>
        /// 手札枚数の増加
        /// </summary>
        public void AddHand()
        {
            Hand++;
        }

        /// <summary>
        /// 交換回数の増加
        /// </summary>
        public void AddExchange()
        {
            ExchangeCount++;
        }

        public (List<Card>, List<Card>) ChangeSuit(Skill skill)
        {
            var remove = DeckCard.Where(c => c.Number == skill.CardFrom.Number && c.Suit != skill.CardTo.Suit)
                .ToList();
            var add = new List<Card>();
            foreach (var card in remove)
            {
                DeckCard.Remove(card);
                var newCard = new Card(card.Number, skill.CardTo.Suit);
                DeckCard.Add(newCard);
                add.Add(newCard);
            }

            return (remove, add);
        }

        public (List<Card>, List<Card>) ChangeNumber(Skill skill)
        {
            var remove = DeckCard.Where(c => c.Number == skill.CardFrom.Number).ToList();
            var add = new List<Card>();
            foreach (var card in remove)
            {
                DeckCard.Remove(card);
                var newCard = new Card(skill.CardTo.Number, card.Suit);
                DeckCard.Add(newCard);
                add.Add(newCard);
            }

            return (remove, add);
        }

        public (List<Card>, List<Card>) LowToMiddle(Skill skill)
        {
            var remove = DeckCard.Where(c => c.Suit == skill.CardFrom.Suit && c.IsLow).ToList();
            var add = new List<Card>();

            foreach (var card in remove)
            {
                DeckCard.Remove(card);
                var newCard = new Card(card.Number + 4, card.Suit);
                DeckCard.Add(newCard);
                add.Add(newCard);
            }

            return (remove, add);
        }

        public (List<Card>, List<Card>) HighToMiddle(Skill skill)
        {
            var remove = DeckCard.Where(c => c.Suit == skill.CardFrom.Suit && c.IsHigh).ToList();
            var add = new List<Card>();

            foreach (var card in remove)
            {
                DeckCard.Remove(card);
                var newCard = new Card(card.Number - 4, card.Suit);
                DeckCard.Add(newCard);
                add.Add(newCard);
            }

            return (remove, add);
        }

        public (List<Card>, List<Card>) MiddleToHigh(Skill skill)
        {
            var remove = DeckCard.Where(c => c.Suit == skill.CardFrom.Suit && c.IsMiddle).ToList();
            var add = new List<Card>();

            foreach (var card in remove)
            {
                DeckCard.Remove(card);
                var newCard = new Card(card.Number + 4, card.Suit);
                DeckCard.Add(newCard);
                add.Add(newCard);
            }

            return (remove, add);
        }

        public void Export()
        {
#if UNITY_EDITOR
            var deck = new Deck(DeckCard, Hand, ExchangeCount);
            var json = JsonUtility.ToJson(deck);
            var now = DateTime.Now;
            var deckName = $"Deck{now.Day}_{now.Hour}_{now.Minute}_{now.Second}.json";
            var path = $"/Users/hiroakigoto/unity1week_poker/Assets/Resources/Deck/{deckName}";
            File.WriteAllText(path, json);
#endif
        }
    }
}
