using System;
using TMPro;
using Unity.Collections;
using UnityEngine;

namespace Core.Player
{
    public class PlayerNameDisplay : MonoBehaviour
    {
        [SerializeField] private TankPlayer _player;
        [SerializeField] private TMP_Text _playerNameTMP;
        
        private void Start()
        {
            HandlePlayerNameChange(string.Empty, _player.PlayerName.Value);
            
            _player.PlayerName.OnValueChanged += HandlePlayerNameChange;
        }

        private void HandlePlayerNameChange(FixedString32Bytes oldName, FixedString32Bytes newName)
        {
            _playerNameTMP.text = newName.ToString();
        }

        private void OnDestroy()
        {
            
            _player.PlayerName.OnValueChanged -= HandlePlayerNameChange;
        }
    }
}