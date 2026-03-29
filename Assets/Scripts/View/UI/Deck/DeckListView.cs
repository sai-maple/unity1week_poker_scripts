using System.Collections.Generic;
using System.Linq;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Laughter.Poker.View.UI.Deck
{
    /// <summary>
    /// デッキの内容を表示するView
    /// </summary>
    public class DeckListView : MonoBehaviour
    {
        [SerializeField] private ScrollRect _deck;
        [SerializeField] private DeckCell _deckCellPrefab;
        [SerializeField] private TextMeshProUGUI[] _suitCount;

        public void Initialize(List<Card> deck)
        {
            foreach (var suit in EnumExtensions.GetValues<Suit>())
            {
                _suitCount[(int)suit].text = deck.Count(c => c.Suit == suit).ToString();
            }

            for (var number = 1; number <= 13; number++)
            {
                var cell = Instantiate(_deckCellPrefab, _deck.content);
                var percentage = (float)deck.Count(c => c.Number == number) / deck.Count;
                var spade = deck.Count(c => c.Number == number && c.Suit == Suit.Spade);
                var heard = deck.Count(c => c.Number == number && c.Suit == Suit.Heart);
                var club = deck.Count(c => c.Number == number && c.Suit == Suit.Club);
                var diamond = deck.Count(c => c.Number == number && c.Suit == Suit.Diamond);
                cell.CardNum(number, spade, heard, club, diamond, percentage);
            }
        }

        public void Clear()
        {
            foreach (Transform child in _deck.content)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
