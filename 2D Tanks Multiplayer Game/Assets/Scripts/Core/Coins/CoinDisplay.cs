using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CoinDisplay : NetworkBehaviour
{
    [Header("References")] 
    [SerializeField] private CoinWallet _coinWallet;
    [SerializeField] private TMP_Text _coinsTMP;

    public override void OnNetworkSpawn()
    {
        if(!IsClient) return;
        
        _coinWallet.TotalCoins.OnValueChanged += HandleHealthChange;
        HandleHealthChange(0, _coinWallet.TotalCoins.Value);
    }

    public override void OnNetworkDespawn()
    {
        if(!IsClient) return;
        
        _coinWallet.TotalCoins.OnValueChanged -= HandleHealthChange;
    }

    private void HandleHealthChange(int oldHealth, int newHealth)
    {
        _coinsTMP.text = _coinWallet.TotalCoins.Value.ToString();
    }
}
