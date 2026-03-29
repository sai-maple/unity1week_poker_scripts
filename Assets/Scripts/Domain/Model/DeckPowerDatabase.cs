using System;
using System.Collections.Generic;

namespace Laughter.Poker.Domain.Model
{
    [Serializable]
    public class DeckPowerDatabase
    {
        public List<DeckPowerData> Database =  new();
    }

    [Serializable]
    public class DeckPowerData
    {
        public string JsonName;
        public float Power;

        public DeckPowerData(string jsonName, float power)
        {
            JsonName = jsonName;
            Power = power;
        }
    }
}
