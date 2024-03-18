using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Image _healBar;

    [Header("Settings")] 
    [SerializeField] private int _maxHealValue = 30;
    [SerializeField] private float _healCooldown = 60f;
    [SerializeField] private float _healTickRate = 1f;
    [SerializeField] private int _coinsPerTick = 10;
    [SerializeField] private int _healthPerTick = 10;
    private float _remainingCooldown;
    private float _tickTimer;

    private List<TankPlayer> _playersInZone = new List<TankPlayer>();

    private NetworkVariable<int> HealPower = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged += HandleHealPowerChange;
            HandleHealPowerChange(0, HealPower.Value);
        }

        if (IsServer)
        {
            HealPower.Value = _maxHealValue;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged -= HandleHealPowerChange;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer)return;
        
        if(!other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) return;
        
        _playersInZone.Add(player);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsServer) return;
        
        if(!other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) return;
        
        _playersInZone.Remove(player);
    }

    private void Update()
    {
        if (_remainingCooldown > 0)
        {
            _healBar.fillAmount = 1 / _remainingCooldown;
        }
        
        if(!IsServer) return;

        if (_remainingCooldown > 0)
        {
            _remainingCooldown -= Time.deltaTime;
            
            if (_remainingCooldown <= 0)
            {
                HealPower.Value = _maxHealValue;
            }
            else
            {
                return;
            }
        }

        _tickTimer += Time.deltaTime;
        if (_tickTimer >= 1 / _healTickRate)
        {
            foreach (TankPlayer player in _playersInZone)
            {
                if (HealPower.Value == 0) return;
                
                if(player.Health.CurrentHealth.Value == player.Health.MaxHealth) continue;
                
                if(player.Wallet.TotalCoins.Value < _coinsPerTick) continue;
                
                player.Wallet.SpendCoins(_coinsPerTick);
                player.Health.RestoreHealth(_healthPerTick);

                HealPower.Value -= 1;

                if (HealPower.Value == 0)
                {
                    _remainingCooldown = _healCooldown;
                }
            }

            _tickTimer = _tickTimer % (1 / _healTickRate);
        }
    }

    private void HandleHealPowerChange(int oldHealPower, int newHealPower)
    {
        _healBar.fillAmount = (float)newHealPower / _maxHealValue;
    }
}
