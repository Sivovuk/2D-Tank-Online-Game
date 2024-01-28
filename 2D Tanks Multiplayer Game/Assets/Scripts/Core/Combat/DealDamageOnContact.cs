using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : NetworkBehaviour
{
    [SerializeField] private int _damage = 5;

    private ulong ownerClientID;
    
    public void SetOwner(ulong ownerClientID)
    {
        this.ownerClientID = ownerClientID;
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        Debug.LogError(gameObject.name);
        if(collider2D.attachedRigidbody == null) return;

        if (collider2D.attachedRigidbody.TryGetComponent<NetworkObject>(out var netObj))
        {
            if (ownerClientID == netObj.OwnerClientId)
            {
                Debug.Log("Isti objekat");
                return;
            }
        }

        if (collider2D.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            Debug.Log("Pogodjen drugi objekat");
            health.TakeDamage(_damage);
        }
    }
}
