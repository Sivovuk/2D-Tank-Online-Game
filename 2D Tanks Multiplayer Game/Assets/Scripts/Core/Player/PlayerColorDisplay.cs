using System.Collections;
using System.Collections.Generic;
using Core.Player;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerColorDisplay : MonoBehaviour
{
    [SerializeField] private TankPlayer _player;
    [SerializeField] private SpriteRenderer[] _playerParts;
    [SerializeField] private TeamColorLookup _colorLookup;
    
    private void Start()
    {
        HandlePlayerNameChange(-1, _player.TeamIndex.Value);
            
        _player.TeamIndex.OnValueChanged += HandlePlayerNameChange;
    }

    private void HandlePlayerNameChange(int oldIndex, int newIndex)
    {
        foreach (var part in _playerParts)
        {
            part.color = _colorLookup.GetTeamColor(newIndex);
        }
    }

    private void OnDestroy()
    {
        _player.TeamIndex.OnValueChanged -= HandlePlayerNameChange;
    }
}
