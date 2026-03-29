using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Master;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.View.UI.Audio;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Laughter.Poker.View.Game
{
    public class CardView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private CardMaster _cardMaster;
        [SerializeField] private SpriteRenderer _cardRenderer;
        [SerializeField] private SpriteRenderer _selectedFrame;
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private Transform _hintContainer;
        [SerializeField] private HintView _hintPrefab;

        private Card _card;
        private int _index;
        private bool _isSelected;
        private bool _isFront;
        private HintView _hintView;

        private readonly Subject<int> _selectSubject = new();

        public IObservable<int> OnSelectAsObservable() => _selectSubject;

        // ゲーム終了時とかに制御したい
        public bool IsSelectable
        {
            get => _boxCollider2D.enabled;
            set => _boxCollider2D.enabled = value;
        }

        public void Initialize(Card card, bool isFront)
        {
            _card = card;
            _isFront = isFront;
            _cardRenderer.sprite = isFront ? _cardMaster.Get(_card) : _cardMaster.Back();
            _selectedFrame.enabled = false;
        }

        public async UniTask FlipAsync(bool isFront, CancellationToken token = default)
        {
            if (_isFront == isFront)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: token);
                return;
            }

            // todo Flip
            AudioView.Instance.PlayOneShot(Sounds.DrawCard);
            await DOTween.Sequence()
                .Append(_cardRenderer.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.1f).SetEase(Ease.Linear))
                .AppendCallback(() =>
                {
                    _cardRenderer.sprite = isFront ? _cardMaster.Get(_card) : _cardMaster.Back();
                    _cardRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));
                    _isFront = isFront;
                })
                .Append(_cardRenderer.transform.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.Linear))
                .ToUniTask(cancellationToken: token);
        }

        public void Sort(Vector3 localPosition, Vector3 localRotation, int index, float duration, float delay)
        {
            _index = index;
            _cardRenderer.sortingOrder = index * 2 + 1;
            _selectedFrame.sortingOrder = index * 2;
            transform.DOLocalMove(localPosition, duration + delay * index);
            transform.DOLocalRotate(localRotation, duration + delay * index);
            // todo scale いじってもいいかも
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _selectSubject.OnNext(_index);
        }

        public void OnSelect()
        {
            AudioView.Instance.PlayOneShot(Sounds.CardSelect);
            _isSelected = !_isSelected;
            _selectedFrame.enabled = _isSelected;
            var endValue = _isSelected ? Vector3.up * 1f : Vector3.zero;
            _cardRenderer.transform.DOLocalMove(endValue, 0.2f);
        }

        /// <summary>
        /// カードをトラッシュの位置に移動します
        /// </summary>
        /// <param name="target"></param>
        /// <param name="duration"></param>
        /// <param name="token"></param>
        public async UniTask TrashAsync(Vector3 target, float duration, CancellationToken token)
        {
            IsSelectable = false;
            _selectedFrame.enabled = false;
            transform.DOLocalRotate(Vector3.zero, 0.1f);
            _cardRenderer.transform.DOLocalMove(Vector3.zero, 0.1f);
            await transform.DOMove(target, duration).ToUniTask(cancellationToken: token);
        }

        /// <summary>
        /// 手札を裏向きにして中央に集めます
        /// </summary>
        /// <param name="target"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async UniTask ToCenterAsync(Vector3 target, CancellationToken token)
        {
            IsSelectable = false;
            _hintView?.SetActive(false);
            _selectedFrame.enabled = false;
            transform.DOLocalRotate(Vector3.zero, 0.1f);
            _cardRenderer.transform.DOLocalMove(Vector3.zero, 0.1f);
            await FlipAsync(true, token);
            AudioView.Instance.PlayOneShot(Sounds.Sort);
            await transform.DOMove(target, 0.2f).ToUniTask(cancellationToken: token);
        }

        /// <summary>
        /// カードを左端から順に並べます
        /// </summary>
        public async UniTask ShowAsync(
            Vector3 startPosition,
            Vector3 targetPosition,
            float duration,
            CancellationToken token)
        {
            await transform.DOMove(startPosition, 0.3f).ToUniTask(cancellationToken: token);
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);
            await transform.DOMove(targetPosition, duration).ToUniTask(cancellationToken: token);
        }

        public void AddHint(List<CardHint> hints)
        {
            _hintView = Instantiate(_hintPrefab, _hintContainer);
            foreach (var hint in hints.Where(hint => hint.Card.Equals(_card)))
            {
                _hintPrefab.Initialize(hint);
            }
        }
    }
}
