using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.View.UI.Audio;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Laughter.Poker.View.Game
{
    public class HandCard : MonoBehaviour
    {
        // todo _cardSpacingは枚数に応じて可変でいいかも
        [SerializeField] private float _cardSpacing = 1.5f;
        [SerializeField] private float _fanAngle = 3f;
        [SerializeField] private float _yOffset = 0.2f;
        [SerializeField] private float _duration = 0.1f;
        [SerializeField] private float _delay = 0.1f;
        private readonly List<CardView> _handCards = new();
        private readonly List<CardView> _trashCards = new();

        private int _trashCount;
        private bool _isActive = true;

        public void Add(List<CardView> handCards)
        {
            if (!_isActive) return;
            _handCards.AddRange(handCards);
            Sort();
        }

        public void Sort()
        {
            var count = _handCards.Count;
            foreach (var (card, index) in _handCards.Select((c, i) => (c, i)))
            {
                var xPos = (index - (count - 1) * 0.5f) * _cardSpacing;
                var angle = (index - (count - 1) * 0.5f) * -_fanAngle;
                var yPos = Mathf.Abs(index - (count - 1) * 0.5f) * -_yOffset;
                var targetRotation = new Vector3(0, 0, angle);
                var localPosition = new Vector3(xPos, yPos, 0) + transform.position;
                card.Sort(localPosition, targetRotation, index, _duration, _delay);
            }
            AudioView.Instance.PlayOneShot(Sounds.Sort);
        }

        /// <summary>
        /// ビットブラグで指定された位置のカードを破棄します
        /// </summary>
        public async UniTask<List<CardView>> RemoveCardAsync(int removeFlag, Vector3 target,
            CancellationToken token = default)
        {
            var removedCards = new List<CardView>();
            var task = new List<UniTask>();
            for (var i = 0; i < _handCards.Count; i++)
            {
                if (((removeFlag >> i) & 1) == 0) continue;
                removedCards.Add(_handCards[i]);
                task.Add(_handCards[i].FlipAsync(false, token));
            }

            await task;

            task.Clear();
            var hash = new HashSet<CardView>(removedCards);
            _handCards.RemoveAll(x => hash.Contains(x));
            foreach (var (card, index) in removedCards.Select((c, i) => (c, i)))
            {
                // todo duration はindexに対して収束する方向で大きくなっていきたい 
                target.x += index * 0.05f;
                target.y += _trashCount * -0.1f;
                task.Add(card.TrashAsync(target, _duration + _delay * index, token));
            }

            _trashCount++;
            _trashCards.AddRange(removedCards);
            await task;
            return removedCards;
        }

        /// <summary>
        /// 手札のカードを選択不可にします
        /// </summary>
        public void LockAll()
        {
            foreach (var card in _handCards)
            {
                card.IsSelectable = false;
            }
        }

        /// <summary>
        /// 手札を公開して見えるように並べます
        /// </summary>
        public async UniTask OpenHandAsync(bool isSelf, CancellationToken token)
        {
            // 手札を公開して左端に集める
            var count = _handCards.Count;
            var center = transform.position;
            var task = Enumerable.Select(_handCards, card => card.ToCenterAsync(center, token)).ToList();
            await task;
            task.Clear();
            const float spacing = 1f;
            var left = new Vector3(-(count - 1) * 0.5f * spacing, 1, 0) * (isSelf ? 1 : -1);
            var localLeft = left + transform.position;
            foreach (var (card, index) in _handCards.Select((c, i) => (c, i)))
            {
                var target = new Vector3((index - (count - 1) * 0.5f) * spacing, 1, 0) * (isSelf ? 1 : -1);
                var localTarget = target + transform.position;
                var duration = (_duration + _delay * index) * 1.5f;
                task.Add(card.ShowAsync(localLeft, localTarget, duration, token));
            }

            await task;
        }

        /// <summary>
        /// 手札を裏向きにしてバサッと捨てます
        /// </summary>
        /// <param name="token"></param>
        public async UniTask FoldAsync(CancellationToken token)
        {
            var task = Enumerable.Select(_handCards, card => card.FlipAsync(false, token)).ToList();
            await task;

            // 手札をバサッと捨てる
            var count = _handCards.Count;
            foreach (var (card, index) in _handCards.Select((c, i) => (c, i)))
            {
                var xPos = (index - (count - 1) * 0.5f) * _cardSpacing;
                var angle = Random.Range(-75, 75);
                var yPos = Random.Range(0.5f, 1.5f);
                var targetRotation = new Vector3(0, 0, angle);
                var localPosition = new Vector3(xPos, yPos, 0) + transform.position;

                card.Sort(localPosition, targetRotation, index, _duration, _delay);
            }
        }


        /// <summary>
        /// 手札とトラッシュ、全てのカードを破棄します
        /// </summary>
        /// <param name="target"></param>
        /// <param name="token"></param>
        public async UniTask DestroyHandCardAsync(Vector3 target, CancellationToken token)
        {
            _isActive = false;
            var task = new List<UniTask>();
            var cards = _handCards.Concat(_trashCards);
            foreach (var (card, index) in cards.Select((c, i) => (c, i)))
            {
                task.Add(ResetCard(target, card, _duration + _delay * index, token));
            }

            await task;
        }

        private static async UniTask ResetCard(Vector3 target, CardView card, float duration, CancellationToken token)
        {
            await card.FlipAsync(false, token);
            await card.TrashAsync(target, duration, token);
            Destroy(card.gameObject);
        }

        public void AddHint(List<CardHint> hints)
        {
            foreach (var hint in hints)
            {
                _handCards[hint.Index].AddHint(hints);
            }
        }

        private void OnDestroy()
        {
            _handCards.Clear();
            _trashCards.Clear();
        }
    }
}
