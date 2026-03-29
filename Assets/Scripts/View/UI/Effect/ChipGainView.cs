using UnityEngine;
using Random = UnityEngine.Random;

namespace Laughter.Poker.View.UI.Effect
{
    public class ChipGainView : MonoBehaviour
    {
        public static ChipGainView Instance;

        [SerializeField] private ParticleSystem _chipParticle;
        [SerializeField] private int _burstMultiplier = 1;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlayEffect(int chipGain)
        {
            if (_chipParticle == null)
                return;

            var emitParams = new ParticleSystem.EmitParams();

            for (var i = 0; i < chipGain; i++)
            {
                emitParams.position = new Vector3(Random.Range(-8f, 8f), Random.Range(8f, 10f), 0f);
                emitParams.velocity = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-6f, -15f), 0f);
                emitParams.rotation = Random.Range(0f, 360f);
                emitParams.startLifetime = Random.Range(1.5f, 2.5f);
                emitParams.startSize = Random.Range(0.3f, 1f);

                _chipParticle.Emit(emitParams, 1);
            }
        }
    }
}
