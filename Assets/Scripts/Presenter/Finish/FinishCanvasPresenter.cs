using Laughter.Poker.Domain.Service;
using Laughter.Poker.View.Finish;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using unityroom.Api;
using VContainer;
using VContainer.Unity;

namespace Laughter.Poker.Presenter.Finish
{
    [RequireComponent(typeof(FinishCanvas), typeof(LifetimeScope))]
    public class FinishCanvasPresenter : MonoBehaviour
    {
        [SerializeField] private FinishCanvas _canvas;
        private RoundService _roundService;
        private PlayerStatusService _statusService;
        private ChipService _chipService;


        [Inject]
        private void Construct(
            RoundService roundService,
            PlayerStatusService playerStatusService,
            ChipService chipService)
        {
            _roundService = roundService;
            _statusService = playerStatusService;
            _chipService = chipService;
        }

        private void Start()
        {
            _canvas.Initialize(
                _statusService.DeckCard,
                _roundService.Round,
                _chipService.TotalChip);

            _canvas.OnReturnAsObservable()
                .Subscribe(_ => SceneManager.LoadScene("MainScene"))
                .AddTo(this);

            _canvas.OnPostAsObservable()
                .Subscribe(_ => naichilab.UnityRoomTweet.Tweet("inflation-poker",
                    $"Inflation PokerでROUND{_roundService.Round}まで到達しました。\n総獲得チップ : {_chipService.TotalChip}",
                    "unity1week"))
                .AddTo(this);

            _canvas.Present();
            UnityroomApiClient.Instance.SendScore(1, _roundService.Round, ScoreboardWriteMode.Always);
        }

        private void Reset()
        {
            _canvas = GetComponent<FinishCanvas>();
        }
    }
}
