using Core.Player;
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
        Color32 color = _colorLookup.GetTeamColor(newIndex);
        foreach (var part in _playerParts)
        {
            part.color = color;
        }
    }

    private void OnDestroy()
    {
        _player.TeamIndex.OnValueChanged -= HandlePlayerNameChange;
    }
}
