using Laughter.Poker.Domain.Model;
using UnityEngine;

namespace Laughter.Poker.Domain.Master
{
    [CreateAssetMenu(fileName = nameof(SkillMaster), menuName = "Master/" + nameof(SkillMaster))]
    public class SkillMaster : ScriptableObject
    {
        [SerializeField] private Sprite[] _skillIcon;
        
        public Sprite Get(Skill skill) => _skillIcon[(int)skill.SkillType];
    }
}
