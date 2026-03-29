using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Service;
using Laughter.Poker.Domain.Utility;
using Laughter.Poker.View.UI.Common;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Laughter.Poker.Presenter.UI.Common
{
    [RequireComponent(typeof(HandRankView), typeof(LifetimeScope))]
    public class HandRankPresenter : MonoBehaviour
    {
        [SerializeField] private HandRankView _handRankView;

        private HandService _handService;

        [Inject]
        private void Construct(HandService handService)
        {
            _handService = handService;
        }

        private void Start()
        {
            _handRankView.Initialize();

            _handService.OnAddCardSubject()
                .Subscribe(_ =>
                {
                    var rank = PokerHandEvaluator.EvaluateBest(_handService.Hand);
                    _handRankView.SetHighRank(true, rank.Ranks.Highest());
                }).AddTo(this);
        }

        private void Reset()
        {
            _handRankView = GetComponent<HandRankView>();
        }
    }
}
