using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Master;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.View.UI.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Laughter.Poker.View.UI.Custom
{
    public class CustomCard : MonoBehaviour
    {
        [SerializeField] private CardMaster _cardMaster;
        [SerializeField] private Image _cardRenderer;

        public void Initialize(Card card, Vector3 position)
        {
            transform.position = position;
            _cardRenderer.DOFade(0, 0);
            _cardRenderer.sprite = _cardMaster.Get(card);
        }

        public UniTask PresentAsync(Vector3 target, float duration, CancellationToken token)
        {
            var fade = _cardRenderer.DOFade(1, duration).ToUniTask(cancellationToken: token);
            var move = transform.DOMove(target, duration).ToUniTask(cancellationToken: token);
            return UniTask.WhenAll(fade, move);
        }

        // カードを回転させながら、別のカードに変更します
        public async UniTask ChangeCardAsync(Card card, CancellationToken token = default)
        {
            // todo SE Flip
            AudioView.Instance.PlayOneShot(Sounds.Sort);
            await DOTween.Sequence()
                .Append(_cardRenderer.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.1f).SetEase(Ease.Linear))
                .AppendCallback(() =>
                {
                    _cardRenderer.sprite = _cardMaster.Get(card);
                    _cardRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));
                })
                .Append(_cardRenderer.transform.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.Linear))
                .ToUniTask(cancellationToken: token);
        }

        public UniTask AddDeckAsync(Vector3 position, CancellationToken token = default)
        {
            var move = transform.DOMove(position, 0.3f).ToUniTask(cancellationToken: token);
            var scale = transform.DOScale(0, 0.3f).ToUniTask(cancellationToken: token);
            return UniTask.WhenAll(move, scale);
        }
    }
}
