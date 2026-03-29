using System;
using System.Collections.Generic;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.View.UI.Common;
using Laughter.Poker.View.UI.Deck;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Laughter.Poker.View.Finish
{
    public class FinishCanvas : ViewCommon
    {
        [SerializeField] private DeckListView _deckListView;
        [SerializeField] private TextMeshProUGUI _roundText;
        [SerializeField] private TextMeshProUGUI _chip;
        [SerializeField] private Button _returnButton;
        [SerializeField] private Button _postButton;

        public IObservable<Unit> OnReturnAsObservable() => _returnButton.OnClickAsObservable();
        public IObservable<Unit> OnPostAsObservable() => _postButton.OnClickAsObservable();

        public void SetCamera(Camera mainCamera)
        {
            GetComponent<Canvas>().worldCamera = mainCamera;
        }
        
        public void Initialize(List<Card> deck, int round, int totalChip)
        {
            _deckListView.Initialize(deck);
            _roundText.text = $"到達ROUND : {round}";
            _chip.text = $"総獲得チップ : {totalChip}";
        }
    }
}
