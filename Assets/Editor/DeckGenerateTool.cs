#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using Laughter.Poker.Domain.Enum;
using Laughter.Poker.Domain.Model;
using UnityEditor;
using UnityEngine;

namespace Laughter.Poker
{
    public class DeckGenerateTool : EditorWindow
    {
        [SerializeField, Range(5, 7)] private int _hand = 5;
        [SerializeField, Range(0, 2)] private int _changeCount = 1;
        [SerializeField] private Suit[] _suits = new Suit[DeckCardNum];
        [SerializeField] private int[] _number = new int[DeckCardNum];

        private const int DeckCardNum = 52;

        private Vector3 _scrollPosition;

        [MenuItem("Tools/DeckGenerateTool")]
        private static void ShowWindow()
        {
            GetWindow<DeckGenerateTool>("ToolWindow");
        }

        private void OnGUI()
        {
            _hand = EditorGUILayout.IntField("Hand", _hand);
            _changeCount = EditorGUILayout.IntField("Change Count", _changeCount);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            for (var i = 0; i < DeckCardNum; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _suits[i] = (Suit)EditorGUILayout.EnumPopup("Suit", _suits[i]);
                _number[i] = EditorGUILayout.IntField("Number", _number[i]);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Export"))
            {
                Export();
            }
        }

        private void Export()
        {
            var deckCard = new List<Card>();
            for (var i = 0; i < DeckCardNum; i++)
            {
                deckCard.Add(new Card(_number[i], _suits[i]));
            }

            var deck = new Deck(deckCard, _hand, _changeCount);
            var json = JsonUtility.ToJson(deck);
            var now = DateTime.Now;
            var deckName = $"Deck{now.Day}_{now.Hour}_{now.Minute}_{now.Second}.json";
            var path = $"/Users/hiroakigoto/unity1week_poker/Assets/Resources/Deck/{deckName}";
            File.WriteAllText(path, json);
        }
    }
}
#endif
