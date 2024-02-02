using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningCoin : Coin
{
    public event Action<RespawningCoin> OnCollected;

    private Vector3 previousPosition;

    private void Update()
    {
        if (previousPosition != transform.position)
        {
            Show(true);
        }

        previousPosition = transform.position;
    }

    public override int Collect()
    {
        if (!IsServer)
        {
            Show(false);
            return 0;
        }

        if (_alreadyCollected) return 0;
        
        _alreadyCollected = true;
        OnCollected?.Invoke(this);
        
        return _coinValue;
        
    }

    public void Reset()
    {
        _alreadyCollected = false;
    }
}
