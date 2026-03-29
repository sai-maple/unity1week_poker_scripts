using System;
using System.Collections.Generic;
using System.Linq;
using Laughter.Poker.Domain.Model;
using UniRx;
using UnityEngine;

namespace Laughter.Poker.Domain.Service
{
    public class HandService : IDisposable
    {
        // カードは左端を0とする
        private readonly List<Card> _hand = new();
        private readonly Subject<List<Card>> _addSubject = new();
        private readonly Subject<int> _removeSubject = new();

        private int _removeFlag;

        public IReadOnlyList<Card> Hand => _hand;
        public IObservable<List<Card>> OnAddCardSubject() => _addSubject;
        public IObservable<int> OnRemoveCardSubject() => _removeSubject;
        public bool IsLock { get; private set; } = false;

        public void Add(List<Card> addCard)
        {
            _hand.AddRange(addCard);
            _addSubject.OnNext(addCard);
            _removeFlag = 0b0;
        }

        public bool TrySelect(int index, int limit)
        {
            var count = 0;
            // 残りデッキ枚数を超える選択を禁止する
            var temp = _removeFlag;
            while (temp != 0)
            {
                temp &= temp - 1;
                count++;
            }

            if (limit < count + 1) return false;
            _removeFlag ^= 1 << index;
            Debug.Log($"{index}枚目, {Convert.ToString(_removeFlag, 2)}, {count}");
            return true;
        }

        public void DiscordByBrain(int removeFlag)
        {
            _removeFlag = removeFlag;
            Discard();
        }

        public List<Card> Discard()
        {
            var removedCards = _hand.Where((_, i) => ((_removeFlag >> i) & 1) != 0).ToList();

            var hash = new HashSet<Card>(removedCards);
            _hand.RemoveAll(x => hash.Contains(x));

            _removeSubject.OnNext(_removeFlag);
            return removedCards;
        }

        public void Lock()
        {
            Debug.Log("交換仕切ったのでカードをロック");
            IsLock = true;
        }

        public void Dispose()
        {
            _addSubject?.OnCompleted();
            _addSubject?.Dispose();
            _removeSubject?.OnCompleted();
            _removeSubject?.Dispose();
        }
    }
}
