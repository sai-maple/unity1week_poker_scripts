using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Laughter.Poker.View.UI.Deck
{
    /// <summary>
    /// お互いのデッキ画面のCell
    /// </summary>
    public class DeckCell : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _number;
        [SerializeField] private TextMeshProUGUI _spadeCount;
        [SerializeField] private TextMeshProUGUI _heartCount;
        [SerializeField] private TextMeshProUGUI _clubCount;
        [SerializeField] private TextMeshProUGUI _diamondCount;
        [SerializeField] private Slider _percentage;

        public void CardNum(
            int number,
            int spadeCount,
            int heartCount,
            int clubCount,
            int diamondCount,
            float percentage)
        {
            _number.text = $"{NumberToText(number)}";
            _spadeCount.text = $"{spadeCount}";
            _heartCount.text = $"{heartCount}";
            _clubCount.text = $"{clubCount}";
            _diamondCount.text = $"{diamondCount}";
            _percentage.value = percentage;
        }

        private static string NumberToText(int number)
        {
            return number switch
            {
                1 => "A",
                <= 10 => number.ToString(),
                11 => "J",
                12 => "Q",
                13 => "K",
                _ => throw new ArgumentOutOfRangeException(nameof(number), number, null)
            };
        }
    }
}
