namespace Laughter.Poker.Domain.Service
{
    /// <summary>
    /// 何回戦か管理するサービス
    /// ラウンドごとの相手のデッキを返します
    /// </summary>
    public class RoundService
    {
        public int Round { get; private set; } = 1;
        private int _foldCount;

        public void Fold()
        {
            _foldCount++;
        }

        public int GerFoldChip()
        {
            return 5 + _foldCount * (_foldCount + 1) / 2;
        }

        public void Next()
        {
            Round++;
        }
    }
}
