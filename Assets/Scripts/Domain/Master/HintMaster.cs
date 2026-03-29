using Laughter.Poker.Domain.Enum;
using Laughter.Poker.View.Game;
using UnityEngine;

namespace Laughter.Poker.Domain.Master
{
    [CreateAssetMenu(fileName = nameof(HintMaster), menuName = "Master/" + nameof(HintMaster))]
    public class HintMaster : ScriptableObject
    {
        [SerializeField] private Sprite[] _cardHint;
        [SerializeField] private HintView[] _handHint;

        public Sprite Get(CardHintType cardHint) => _cardHint[(int)cardHint];
        public HintView Get(TrendHintType hintType) => _handHint[(int)hintType];
    }
}
