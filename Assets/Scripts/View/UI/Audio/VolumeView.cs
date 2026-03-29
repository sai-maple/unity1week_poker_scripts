using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Laughter.Poker.View.UI.Audio
{
    public class VolumeView : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private AudioView _audioView;
        [SerializeField, Range(0, 1)] private float _volume = 0.5f;

        private void Awake()
        {
            _audioView.SetVolume(_volume);
            _slider.value = _volume;
            _slider.OnValueChangedAsObservable()
                .Subscribe(_audioView.SetVolume)
                .AddTo(this);
        }
    }
}
