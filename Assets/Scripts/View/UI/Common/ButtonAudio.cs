using Laughter.Poker.Domain.Enum;
using Laughter.Poker.View.UI.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Laughter.Poker.View.UI.Common
{
    [RequireComponent(typeof(Button))]
    public class ButtonAudio : MonoBehaviour
    {
        [SerializeField] private Sounds _sound;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => AudioView.Instance.PlayOneShot(_sound));
        }
    }
}
