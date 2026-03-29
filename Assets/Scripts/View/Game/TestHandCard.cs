using Laughter.Poker.Domain.Service;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Laughter.Poker.View.Game
{
    public class TestHandCard : MonoBehaviour
    {
        [SerializeField] private Button _discordButton;

        private PlayerStatusService _statusService;
        private DeckService _deckService;
        private HandService _handService;

        [Inject]
        private void Construct(PlayerStatusService statusService, DeckService deckService, HandService handService)
        {
            _statusService = statusService;
            _deckService = deckService;
            _handService = handService;
        }

        private void Start()
        {
            var cards = _deckService.Draw(_statusService.Hand);
            _handService.Add(cards);

            _discordButton.onClick.AddListener(RemoveCard);
        }

        private void RemoveCard()
        {
            _handService.Discard();
        }
    }
}
