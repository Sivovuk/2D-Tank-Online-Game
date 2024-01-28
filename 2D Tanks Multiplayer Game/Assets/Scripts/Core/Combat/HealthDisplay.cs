using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")] 
    [SerializeField] private Health _health;
    [SerializeField] private Image _healthBarImage;

    public override void OnNetworkSpawn()
    {
        if(!IsClient) return;
        
        _health.CurrentHealth.OnValueChanged += HandleHealthChange;
        HandleHealthChange(0, _health.CurrentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if(!IsClient) return;
        
        _health.CurrentHealth.OnValueChanged -= HandleHealthChange;
    }

    private void HandleHealthChange(int oldHealth, int newHealth)
    {
        Debug.Log("Health change");
        _healthBarImage.fillAmount = (float)newHealth / _health.MaxHealth;
    }
}
