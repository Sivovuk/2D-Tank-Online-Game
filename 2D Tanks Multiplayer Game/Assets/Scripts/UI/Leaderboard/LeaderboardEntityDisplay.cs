﻿using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace UI.Leaderboard
{
    public class LeaderboardEntityDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _displayText;
        
        private FixedString32Bytes _displayName;
        public int TeamIndex { get; private set; }
        public ulong ClientID { get; private set; }
        public int Coins { get; private set; }

        public void Initialise(ulong clientId, FixedString32Bytes displayName, int coins)
        {
            ClientID = clientId;
            _displayName = displayName;
            Coins = coins;

            UpdateCoins(Coins);
        }
        
        public void Initialise(int teamIndex, FixedString32Bytes displayName, int coins)
        {
            TeamIndex = teamIndex;
            _displayName = displayName;
            Coins = coins;

            UpdateCoins(Coins);
        }

        public void SetColor(Color color)
        {
            _displayText.color = color;
        }

        public void UpdateCoins(int coins)
        {
            Coins = coins;
            
            UpdateText();
        }

        public void UpdateText()
        {
            _displayText.text = $"{transform.GetSiblingIndex()+1} {_displayName} ({Coins})";
        }
    }
}