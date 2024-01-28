using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    [field: SerializeField] public int MaxHealth { get; private set; } = 100;

    [SerializeField] private bool isDead;

    public Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        Debug.Log(gameObject.name);
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }

    public void ModifyHealth(int value)
    {
        if (isDead) return;
        Debug.Log("1");

        int newHealth = CurrentHealth.Value + value;
        CurrentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

        Debug.Log("2");
        if (CurrentHealth.Value == 0)
        {
            Debug.Log("3");
            isDead = true;
            OnDie?.Invoke(this);
        }
    }
}
