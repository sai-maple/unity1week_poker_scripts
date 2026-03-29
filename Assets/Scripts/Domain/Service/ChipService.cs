using System;
using Laughter.Poker.Domain.Utility;
using UniRx;

namespace Laughter.Poker.Domain.Service
{
    /// <summary>
    /// 残りチップ枚数を管理するService
    /// </summary>
    public class ChipService : IDisposable
    {
        private readonly ReactiveProperty<int> _chip = new(StaticData.StartChip);
        public int TotalChip { get; private set; }

        public IObservable<int> OnChipChangedAsObservable() => _chip;
        public int CurrentChip => _chip.Value;

        public void Increase(int chip)
        {
            _chip.Value += chip;
            TotalChip += chip;
        }

        public bool TryDecrease(int chip)
        {
            if (_chip.Value < chip)
            {
                return false;
            }

            _chip.Value -= chip;
            return true;
        }

        public void Dispose()
        {
            _chip?.Dispose();
        }
    }
}
