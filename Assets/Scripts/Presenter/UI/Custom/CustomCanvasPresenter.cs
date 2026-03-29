using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.Domain.Service;
using Laughter.Poker.Domain.Utility;
using Laughter.Poker.View.UI.Audio;
using Laughter.Poker.View.UI.Custom;
using Laughter.Poker.View.UI.Deck;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Laughter.Poker.Presenter.UI.Custom
{
    [RequireComponent(typeof(CustomCanvas), typeof(LifetimeScope))]
    public class CustomCanvasPresenter : MonoBehaviour
    {
        [SerializeField] private CustomCanvas _customCanvas;
        [SerializeField] private DeckCanvas _deckCanvas;
        [SerializeField] private DeckListView _deckList;
        [SerializeField] private CustomCard _cardView;
        private RoundService _roundService;
        private ChipService _chipService;
        private PlayerStatusService _playerStatusService;

        private int _rerollChip;

        [Inject]
        private void Construct(
            RoundService roundService,
            ChipService chipService,
            PlayerStatusService playerStatusService)
        {
            _roundService = roundService;
            _chipService = chipService;
            _playerStatusService = playerStatusService;
        }

        private void Start()
        {
            _rerollChip = StaticData.RerollBaseConst + StaticData.RerollRoundConst * _roundService.Round;
            _customCanvas.Initialize(_roundService.GerFoldChip(), _chipService.CurrentChip);
            _customCanvas.OnReRollAsObservable()
                .Subscribe(_ =>
                {
                    _chipService.TryDecrease(_rerollChip);
                    _customCanvas.SetChip(_chipService.CurrentChip, _rerollChip);
                    _rerollChip += StaticData.RerollBaseConst;
                    CreateSkill();
                });
            _customCanvas.OnDeckAsObservable()
                .Subscribe(_ =>
                {
                    _deckList.Clear();
                    _deckList.Initialize(_playerStatusService.DeckCard);
                    _deckCanvas.Present();
                })
                .AddTo(this);

            CreateSkill();
        }

        private void CreateSkill()
        {
            var token = destroyCancellationToken;
            var skillTypes = SkillTypeExtensions.GetRandomRange(
                4,
                _playerStatusService.Hand,
                _playerStatusService.ExchangeCount,
                _playerStatusService.DeckCard);
            var generator = new SkillGenerator(_playerStatusService);
            var skills = skillTypes.Select(skillType => generator.GetSkill(skillType)).ToList();
            _customCanvas.SetSkillAsync(skills,
                    _chipService.CurrentChip,
                    (skill, card) => BuyAsync(skill, card).Forget(),
                    token)
                .Forget();
            _customCanvas.SetRerollChip(_rerollChip, _chipService.CurrentChip);
        }

        private async UniTaskVoid BuyAsync(Skill skill, SkillCard skillCard)
        {
            _customCanvas.IsLock = true;
            // todo pick se
            var pick = skillCard.Pick(destroyCancellationToken);
            var addCard = UniTask.CompletedTask;

            switch (skill.SkillType)
            {
                case SkillType.AddHand:
                    _playerStatusService.AddHand();
                    break;
                case SkillType.AddExchange:
                    _playerStatusService.AddExchange();
                    break;
                case SkillType.ChangeSuit:
                {
                    var (removeCards, addCards) = _playerStatusService.ChangeSuit(skill);
                    addCard = AddCardAsync(removeCards, addCards);
                    break;
                }
                case SkillType.ChangeNumber:
                {
                    var (removeCards, addCards) = _playerStatusService.ChangeNumber(skill);
                    addCard = AddCardAsync(removeCards, addCards);
                    break;
                }

                case SkillType.LowToMiddle:
                {
                    var (removeCards, addCards) = _playerStatusService.LowToMiddle(skill);
                    addCard = AddCardAsync(removeCards, addCards);
                    break;
                }
                case SkillType.HighToMiddle:
                {
                    var (removeCards, addCards) = _playerStatusService.HighToMiddle(skill);
                    addCard = AddCardAsync(removeCards, addCards);
                    break;
                }
                case SkillType.MiddleToHigh:
                {
                    var (removeCards, addCards) = _playerStatusService.MiddleToHigh(skill);
                    addCard = AddCardAsync(removeCards, addCards);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _chipService.TryDecrease(skill.Chip);
            _customCanvas.SetChip(_chipService.CurrentChip, skill.Chip);
            // カードが入れ替わる演出
            await UniTask.WhenAll(pick, addCard);
            CreateSkill();
            _customCanvas.IsLock = false;
        }

        private async UniTask AddCardAsync(List<Card> removeCards, List<Card> addCards)
        {
            if (removeCards.Count != addCards.Count)
            {
                Debug.LogError("変更枚数が同数ではないです。");
            }

            var token = destroyCancellationToken;
            var cards = new List<CustomCard>();
            var task = new List<UniTask>();
            var count = removeCards.Count;
            foreach (var (removeCard, index) in removeCards.Select((c, i) => (c, i)))
            {
                var pos = new Vector3((index - (count - 1) * 0.5f) * 2.1f, 0, 0);
                var card = Instantiate(_cardView, transform);
                card.Initialize(removeCard, _customCanvas.DeckButtonPosition);
                cards.Add(card);
                task.Add(card.PresentAsync(pos, 0.3f, token));
                AudioView.Instance.PlayOneShot(Sounds.DrawCard);
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: token);
            }

            await UniTask.WhenAll(task);
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);
            task.Clear();
            task.AddRange(Enumerable.Select(cards, (t, i) => t.ChangeCardAsync(addCards[i], token)));

            await UniTask.WhenAll(task);
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);
            task.Clear();
            task.AddRange(Enumerable.Select(cards, (t, i) => t.AddDeckAsync(_customCanvas.DeckButtonPosition, token)));
            await UniTask.WhenAll(task);
            AudioView.Instance.PlayOneShot(Sounds.Sort);
            task.Clear();

            foreach (var card in cards)
            {
                Destroy(card.gameObject);
            }
        }
    }
}
