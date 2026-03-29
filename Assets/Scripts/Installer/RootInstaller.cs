using Laughter.Poker.Domain.Service;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Laughter.Poker.Installer
{
    /// <summary>
    /// 自身と相手で共有するDomainのLifetimeScope
    /// </summary>
    public class RootInstaller: LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            var deck = (TextAsset)Resources.Load("Deck/BaseDeck");
            
            builder.Register<RoundService>(Lifetime.Singleton);
            builder.Register<DeckDatabaseService>(Lifetime.Singleton);
            builder.Register<ChipService>(Lifetime.Singleton);
            builder.Register<PlayerStatusService>(Lifetime.Singleton)
                .WithParameter(deck.text);
        }
    }
}
