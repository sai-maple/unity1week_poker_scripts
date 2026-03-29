using Laughter.Poker.Domain.Service;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Laughter.Poker.Installer
{
    public class PlayerOpponentInstaller : LifetimeScope
    {
        private KeepBoteHandService _keepBoteHandService;
        private string _deck;

        public void Initialize(KeepBoteHandService keepBoteHandService, string deck)
        {
            _keepBoteHandService = keepBoteHandService;
            _deck = deck;
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<DeckService>(Lifetime.Singleton);
            builder.Register<HandService>(Lifetime.Singleton);
            builder.Register<PlayerStatusService>(Lifetime.Singleton)
                .WithParameter(_deck);
            builder.RegisterInstance(_keepBoteHandService);
            builder.RegisterBuildCallback(container =>
            {
                var statusService = container.Resolve<PlayerStatusService>();
                container.Resolve<DeckService>().RegisterAndShuffle(statusService.DeckCard);
                Debug.Log("デッキ登録 Opponent");
            });
        }
    }
}
