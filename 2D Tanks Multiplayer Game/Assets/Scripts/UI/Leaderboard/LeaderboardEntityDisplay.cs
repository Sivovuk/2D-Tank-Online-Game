using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace UI.Leaderboard
{
    public class LeaderboardEntityDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _displayText;
        [SerializeField] private Color _myColor;
        
        private FixedString32Bytes _playerName;
        public ulong ClientID { get; private set; }
        public int Coins { get; private set; }

        public void Initialise(ulong clientId, FixedString32Bytes playerName, int coins)
        {
            ClientID = clientId;
            _playerName = playerName;
            Coins = coins;

            if (clientId == NetworkManager.Singleton.LocalClientId) _displayText.color = _myColor;
            
            UpdateCoins(Coins);
        }

        public void UpdateCoins(int coins)
        {
            Coins = coins;
            
            UpdateText();
        }

        public void UpdateText()
        {
            _displayText.text = $"{transform.GetSiblingIndex()+1} {_playerName} ({Coins})";
        }
    }
}