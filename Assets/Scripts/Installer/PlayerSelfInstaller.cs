using Laughter.Poker.Domain.Service;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Laughter.Poker.Installer
{
    /// <summary>
    /// プレイヤー側のゲーム1サイクル分のLifetimeScope 親 : GameInstaller
    /// </summary>
    public class PlayerSelfInstaller : LifetimeScope
    {
        private KeepBoteHandService _keepBoteHandService;

        public void Initialize(KeepBoteHandService keepBoteHandService)
        {
            _keepBoteHandService = keepBoteHandService;
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<DeckService>(Lifetime.Singleton);
            builder.Register<HandService>(Lifetime.Singleton);
            builder.RegisterInstance(_keepBoteHandService);
            builder.RegisterBuildCallback(container =>
            {
                var statusService = container.Resolve<PlayerStatusService>();
                container.Resolve<DeckService>().RegisterAndShuffle(statusService.DeckCard);
                Debug.Log("デッキ登録 self");
            });
        }
    }
}
