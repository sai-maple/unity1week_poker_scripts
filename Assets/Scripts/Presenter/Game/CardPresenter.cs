using Laughter.Poker.Domain.Service;
using Laughter.Poker.View.Game;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Laughter.Poker.Presenter.Game
{
    [RequireComponent(typeof(CardView), typeof(LifetimeScope))]
    public class CardPresenter : MonoBehaviour
    {
        [SerializeField] private CardView _cardView;

        private DeckService _deckService;
        private HandService _handService;

        [Inject]
        private void Construct(DeckService deckService, HandService handService)
        {
            _deckService = deckService;
            _handService = handService;
        }

        private void Start()
        {
            _cardView.OnSelectAsObservable()
                .Subscribe(index =>
                {
                    if (_handService.TrySelect(index, _deckService.Limit))
                    {
                        _cardView.OnSelect();
                    }
                }).AddTo(this);
        }
    }
}
