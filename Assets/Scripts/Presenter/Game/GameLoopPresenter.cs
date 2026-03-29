using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Service;
using Laughter.Poker.Domain.Utility;
using Laughter.Poker.Installer;
using Laughter.Poker.View.Finish;
using Laughter.Poker.View.UI.Common;
using Laughter.Poker.View.UI.Custom;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Laughter.Poker.Presenter.Game
{
    /// <summary>
    /// ゲームループを管理するPresenter
    /// </summary>
    [RequireComponent(typeof(LifetimeScope))]
    public class GameLoopPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerSelfInstaller _selfInstaller;
        [SerializeField] private PlayerOpponentInstaller _opponentInstaller;
        [SerializeField] private GameFacade _gameFacadePrefab;
        [SerializeField] private CustomCanvas _customCanvasPrefab;
        [SerializeField] private FinishCanvas _finishCanvasPrefab;

        private IObjectResolver _resolver;
        private PlayerStatusService _statusService;
        private ChipService _chipService;
        private RoundService _roundService;
        private DeckDatabaseService _deckDatabaseService;

        [Inject]
        private void Construct(
            IObjectResolver resolver,
            PlayerStatusService statusService,
            ChipService chipService,
            RoundService roundService,
            DeckDatabaseService deckDatabaseService)
        {
            _resolver = resolver;
            _statusService = statusService;
            _chipService = chipService;
            _roundService = roundService;
            _deckDatabaseService = deckDatabaseService;
        }

        private void Start()
        {
            GameLoopAsync().Forget();
        }

        private async UniTaskVoid GameLoopAsync()
        {
            var token = destroyCancellationToken;

            while (true)
            {
                var keepBoteHand = new KeepBoteHandService();
                // 各プレイヤーのLifetimeScopeを生成
                var self = _resolver.Instantiate(_selfInstaller);
                var opponent = _resolver.Instantiate(_opponentInstaller);
                // ラウンド設定
                self.Initialize(keepBoteHand);
                var selfDeckPower = DeckPowerCalculator.CalculatePower(
                    _statusService.DeckCard,
                    _statusService.Hand,
                    _statusService.ExchangeCount);
                opponent.Initialize(keepBoteHand, _deckDatabaseService.GetOpponentDeck(selfDeckPower));
                self.Build();
                opponent.Build();

                // 画面を含むコンポーネント軍を生成
                // カード配布 インスタンス生成時にやってくれる
                var game = Instantiate(_gameFacadePrefab);
                game.Initialize(Camera.main, _chipService.CurrentChip);

                Debug.Log("交換待機");
                // プレイヤーの交換待機
                var isCall = await game.StartRoundAsync(token);

                if (isCall)
                {
                    Debug.Log("相手の交換待機");
                    // 相手の交換待機
                    await keepBoteHand.RegisterAsync(token);
                }

                Debug.Log($"コール {isCall}");
                await (isCall ? game.CallAsync(keepBoteHand.OpponentHand, token) : game.FoldAsync(token));

                // 結果表示 チップ獲得
                var result = isCall ? keepBoteHand.GetResult() : GameResult.Fold;
                await game.ResultAsync(result, keepBoteHand.SelfRanks, _roundService.GerFoldChip(), token);

                // チップ増減
                switch (result)
                {
                    case GameResult.Win:
                        _chipService.Increase(keepBoteHand.SelfRanks.Sum(r => r.ToChip()));
                        // ラウンド増加
                        _roundService.Next();
                        break;
                    case GameResult.Fold:
                        _chipService.TryDecrease(_roundService.GerFoldChip());
                        _roundService.Fold();
                        break;
                    case GameResult.Lose:
                    case GameResult.Draw:
                    default:
                        break;
                }


                Debug.Log("カードのお片付け");
                // カードのお片付け
                await game.DestroyHandCardAsync(token);

                Destroy(self.gameObject);
                Destroy(opponent.gameObject);

                // 画面の破棄
                Destroy(game.gameObject);

                if (result == GameResult.Lose)
                {
                    // ゲームオーバー画面
                    var finish = Instantiate(_finishCanvasPrefab);
                    finish.SetCamera(Camera.main);
                    return;
                }

                // 強化画面生成
                var custom = Instantiate(_customCanvasPrefab);
                custom.SetCamera(Camera.main);
                custom.Present();
                // submit ボタンの押下をawait
                await custom.OnSubmitAsync(token);
                custom.Dismiss();
                _statusService.Export();
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
                Destroy(custom.gameObject);
                await UniTask.Yield(cancellationToken: token);
            }
        }
    }
}
