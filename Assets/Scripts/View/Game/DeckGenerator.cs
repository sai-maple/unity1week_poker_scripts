using System.Collections.Generic;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Laughter.Poker.View.Game
{
    public class DeckGenerator : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private void Start()
        {
            _button.onClick.AddListener(Generate);
        }

        private static void Generate()
        {
            var deckCard = new List<Card>();
            foreach (var suit in EnumExtensions.GetValues<Suit>())
            {
                for (var i = 1; i <= 13; i++)
                {
                    var card = new Card(i, suit);
                    deckCard.Add(card);
                }
            }

            var deck = new Deck(deckCard, 5, 1);
            var json = JsonUtility.ToJson(deck);
            GUIUtility.systemCopyBuffer = json;
            Debug.Log("クリップボードにコピーしました。");
        }
    }
}
