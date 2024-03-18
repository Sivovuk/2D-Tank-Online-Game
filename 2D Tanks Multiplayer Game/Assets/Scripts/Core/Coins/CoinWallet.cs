using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private BountyCoin _coinPrefab;
    [SerializeField] private Health _health;

    [Header("Settings")] 
    [SerializeField] private float _coinSpread = 3f;
    [SerializeField] private float _bountyPercentage = 3f;
    [SerializeField] private int _bountyCoinCount = 10;
    [SerializeField] private int _minBountyCoinValue = 5;
    [SerializeField] private LayerMask _layerMask;
    
    
    private float _coinRadius;
    private Collider2D[] coinBuffer = new Collider2D[1];
    
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }
        
        _coinRadius = _coinPrefab.GetComponent<CircleCollider2D>().radius;

        _health.OnDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
        {
            return;
        }
        _health.OnDie -= HandleDie;
    }

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
    
    private void HandleDie(Health health)
    {
        int bountyValue = (int)(TotalCoins.Value * (_bountyPercentage / 100f));
        int bountyCoinValue = bountyValue / _bountyCoinCount;

        if (bountyCoinValue < _minBountyCoinValue)
        {
            return;
        }

        for (int i = 0; i < _bountyCoinCount; i++)
        {
            BountyCoin coin = Instantiate(_coinPrefab, GetSpawnPoint(), Quaternion.identity);
            coin.SetValue(bountyValue);
            coin.NetworkObject.Spawn();
        }
    }
    
    private Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * _coinSpread;
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, _coinRadius, coinBuffer, _layerMask);

            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }

}
