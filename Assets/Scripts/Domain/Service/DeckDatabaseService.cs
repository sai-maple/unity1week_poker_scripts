using System.Collections.Generic;
using System.Linq;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.Extensions;
using UnityEngine;

namespace Laughter.Poker.Domain.Service
{
    public class DeckDatabaseService
    {
        private readonly List<DeckPowerData> _decks;

        public DeckDatabaseService()
        {
            var json = (TextAsset)Resources.Load("Database");
            var database = JsonUtility.FromJson<DeckPowerDatabase>(json.text);
            _decks = database.Database;
        }

        /// <summary>
        /// 自身のデッキの強さに近いデッキを返す
        /// </summary>
        /// <param name="selfDeckPower"></param>
        /// <returns></returns>
        public string GetOpponentDeck(float selfDeckPower)
        {
            var opponentDeck = _decks.OrderBy(d => Mathf.Abs(d.Power - selfDeckPower)).Take(10).RandomOne();

            var deck = (TextAsset)Resources.Load($"Deck/{opponentDeck.JsonName}");

            return deck.text;
        }
    }
}
