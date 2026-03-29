using System;
using System.Collections.Generic;
using System.Linq;
using Laughter.Poker.Domain.Model;

namespace Laughter.Poker.Domain.Service
{
    /// <summary>
    /// 使用中のデッキを管理するService
    /// </summary>
    public class DeckService
    {
        private List<Card> _deck;

        public IReadOnlyList<Card> Deck => _deck;
        
        /// <summary>
        /// 交換枚数上限
        /// </summary>
        public int Limit => _deck.Count;

        public void RegisterAndShuffle(List<Card> original)
        {
            _deck = original.OrderBy(_ => Guid.NewGuid()).ToList();
        }

        public List<Card> Draw(int count)
        {
            // 先頭n個を獲得
            var result = _deck.Take(count).ToList();
            _deck.RemoveRange(0, count);

            return result;
        }
    }
}
