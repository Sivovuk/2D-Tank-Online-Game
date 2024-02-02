using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>(1000);

    public void SpendCoins(int costToFire)
    {
        TotalCoins.Value -= costToFire;
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (!collider2D.TryGetComponent<Coin>(out Coin coin)) return;
        
        int coinValue = coin.Collect();

        if(!IsServer) return;
        
        TotalCoins.Value += coinValue;
    }
}
