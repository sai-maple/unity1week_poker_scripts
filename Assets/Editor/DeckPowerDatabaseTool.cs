#if UNITY_EDITOR
using System.IO;
using System.Linq;
using Laughter.Poker.Domain.Model;
using Laughter.Poker.Domain.Utility;
using UnityEditor;
using UnityEngine;

namespace Laughter.Poker
{
    public static class DeckPowerDatabaseTool
    {
        private const string DeckDirectory = "/Users/hiroakigoto/unity1week_poker/Assets/Resources/Deck";

        [MenuItem("Tools/DeckPowerDatabaseTool")]
        private static void Save()
        {
            var list = new DeckPowerDatabase();
            var files = Directory.GetFiles(DeckDirectory, "*.json", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var deck = JsonUtility.FromJson<Deck>(json);
                var power = DeckPowerCalculator.CalculatePower(deck.Cards, deck.Hand, deck.ExchangeCount);
                var fileName = file.Split('/').Last().Replace(".json", "");
                var data = new DeckPowerData(fileName, power);
                list.Database.Add(data);
            }

            const string path = "/Users/hiroakigoto/unity1week_poker/Assets/Resources/Database.json";
            var content = JsonUtility.ToJson(list);
            File.WriteAllText(path, content);
        }
    }
}

#endif
