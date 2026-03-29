using System.Collections.Generic;
using System.Linq;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Extensions;
using UnityEngine;

namespace Laughter.Poker.View.UI.Common
{
    public class HandRankView : ViewCommon
    {
        [SerializeField] private HandRankCell _rankPrefab;

        private readonly Dictionary<HandRank, HandRankCell> _cells = new();

        public void Initialize()
        {
            foreach (var handRank in EnumExtensions.GetValues<HandRank>().Reverse())
            {
                var cell = Instantiate(_rankPrefab, transform);
                cell.Initialize(handRank);
                _cells.Add(handRank, cell);
            }
        }

        /// <summary>
        /// 手札の最高ランクを更新
        /// </summary>
        public void SetHighRank(bool isSelf, HandRank handRank)
        {
            foreach (var (rank, cell) in _cells)
            {
                cell.HighRank(isSelf, handRank == rank);
            }
        }

        public void Rise(HandRank handRank)
        {
            _cells[handRank].Rise();
        }

        public void Fall(HandRank handRank)
        {
            _cells[handRank].Fall();
        }
    }
}
