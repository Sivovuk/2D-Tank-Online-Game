using System;
using System.Collections;
using System.Collections.Generic;
using Core.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private TankPlayer _player;
    [SerializeField] private Image _healthBarImage;

    private void Start()
    {
        
        _player.Health.CurrentHealth.OnValueChanged += HandleHealthChange;
        HandleHealthChange(0, _player.Health.CurrentHealth.Value);
    }

    private void OnDestroy()
    {
        _player.Health.CurrentHealth.OnValueChanged -= HandleHealthChange;
    }

    private void HandleHealthChange(int oldHealth, int newHealth)
    {
        _healthBarImage.fillAmount = (float)_player.Health.CurrentHealth.Value / _player.Health.MaxHealth;
    }
}
