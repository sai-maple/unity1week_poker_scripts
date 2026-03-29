using Laughter.Poker.Domain.Service;
using Laughter.Poker.View.UI.Deck;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Laughter.Poker.Presenter.UI.Deck
{
    /// <summary>
    /// ゲーム中の互いのデッキウィストを表示するPresenter LifeTimeScopeの親を別にすることで自分と相手のデッキを表示させます
    /// </summary>
    [RequireComponent(typeof(DeckListView), typeof(LifetimeScope))]
    public class DeckListPresenter : MonoBehaviour
    {
        [SerializeField] private DeckListView _deckList;

        private PlayerStatusService _playerStatusService;

        [Inject]
        private void Construct(PlayerStatusService playerStatusService)
        {
            _playerStatusService = playerStatusService;
        }

        private void Start()
        {
            _deckList.Initialize(_playerStatusService.DeckCard);
        }
    }
}
