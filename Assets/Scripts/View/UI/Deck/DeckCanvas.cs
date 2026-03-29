using Laughter.Poker.View.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Laughter.Poker.View.UI.Deck
{
    public class DeckCanvas : ViewCommon
    {
        [SerializeField] private Button _closeButton;

        private void Start()
        {
            _closeButton.onClick.AddListener(Dismiss);
        }
    }
}
