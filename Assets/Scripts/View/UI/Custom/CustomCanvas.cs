using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.View.UI.Common;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Laughter.Poker.View.UI.Custom
{
    public class CustomCanvas : ViewCommon
    {
        [SerializeField] private Button _rerollButton;
        [SerializeField] private TextMeshProUGUI _rerollCost;
        [SerializeField] private Button _submitButton;
        [SerializeField] private Button _deckButton;
        [SerializeField] private SkillCard _skillPrefab;
        [SerializeField] private Transform _skillCardContent;
        [SerializeField] private TextMeshProUGUI _chip;
        [SerializeField] private TextMeshProUGUI _decreaseChip;
        [SerializeField] private PlayableDirector _decrease;
        [SerializeField] private TextMeshProUGUI _nextFoldChip;

        private readonly List<SkillCard> _skillCards = new();

        public Vector3 DeckButtonPosition => _deckButton.transform.position;

        public IObservable<Unit> OnReRollAsObservable() => _rerollButton.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(500));

        public IObservable<Unit> OnDeckAsObservable() => _deckButton.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(500));

        public UniTask OnSubmitAsync(CancellationToken token) => _submitButton.OnClickAsync(token);

        public void SetCamera(Camera mainCamera)
        {
            GetComponent<Canvas>().worldCamera = mainCamera;
        }

        public void Initialize(int nextFoldChip, int chip)
        {
            _nextFoldChip.text = nextFoldChip.ToString();
            _nextFoldChip.color = nextFoldChip <= chip ? Color.gold : Color.softRed;
            _chip.text = chip.ToString();
        }

        public async UniTask SetSkillAsync(
            List<Skill> skills,
            int chip,
            Action<Skill, SkillCard> callBack,
            CancellationToken token)
        {
            const float delay = 0.1f;
            _rerollButton.interactable = false;
            var task = Enumerable.Select(_skillCards, skillCard => skillCard.DismissAsync(delay, token)).ToList();
            await task;
            task.Clear();
            _skillCards.Clear();

            foreach (var skill in skills)
            {
                var skillCard = Instantiate(_skillPrefab, _skillCardContent);
                skillCard.Initialize(skill, chip, callBack);
                _skillCards.Add(skillCard);
                task.Add(skillCard.PresentAsync(delay, token));
            }

            await UniTask.WhenAll(task);
        }

        public void SetChip(int chip, int decreaseChip)
        {
            _chip.text = chip.ToString();
            _decreaseChip.text = decreaseChip.ToString();
            _decrease.Play();
        }

        public void SetRerollChip(int rerollChip, int chip)
        {
            var isActive = rerollChip <= chip;
            _rerollButton.interactable = isActive;
            _rerollCost.text = $"{rerollChip}";
            _rerollCost.color = isActive ? Color.gray2 : Color.softRed;
        }
    }
}
