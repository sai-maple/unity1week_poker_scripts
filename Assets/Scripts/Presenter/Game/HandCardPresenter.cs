using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.Domain.Service;
using Laughter.Poker.View.Game;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Laughter.Poker.Presenter.Game
{
    /// <summary>
    /// 手札のPresenter プレイヤーと親でそれぞれ別の親LifetimeScopeを持ちます
    /// </summary>
    [RequireComponent(typeof(HandCard), typeof(LifetimeScope))]
    public class HandCardPresenter : MonoBehaviour
    {
        [SerializeField] private HandCard _handCard;
        [SerializeField] private CardView _cardViewPrefab;
        [SerializeField] private Transform _spawner;
        [SerializeField] private Transform _trash;
        [SerializeField] private bool _isSelf = true;

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
            _handService.OnAddCardSubject()
                .Subscribe(DrawCards)
                .AddTo(this);

            _handService.OnRemoveCardSubject()
                .Subscribe(flag => ChangeHandCards(flag).Forget())
                .AddTo(this);

            Debug.Log("初期手札をドロー");
            // 初期手札をドロー
            var cards = _deckService.Draw(_statusService.Hand);
            _handService.Add(cards);
        }

        /// <summary>
        /// カードを生成して自分の手札に加えます
        /// </summary>
        private void DrawCards(List<Card> cards)
        {
            var cardViews = new List<CardView>();
            foreach (var card in cards)
            {
                var cardView = Instantiate(_cardViewPrefab, _spawner.position, Quaternion.identity);
                cardView.Initialize(card, _isSelf);
                cardViews.Add(cardView);
            }

            _handCard.Add(cardViews);
            if (_handService.IsLock) _handCard.LockAll();
        }

        /// <summary>
        /// フラグに対応した手札を捨ます
        /// </summary>
        private async UniTask ChangeHandCards(int removeFlag)
        {
            var token = destroyCancellationToken;
            var result = await _handCard.RemoveCardAsync(removeFlag, _trash.position, token);
            if (token.IsCancellationRequested) return;

            // 捨てた枚数分のドロー処理
            var cards = _deckService.Draw(result.Count);
            _handService.Add(cards);
        }
    }
}
