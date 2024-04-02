using System;
using System.Collections;
using System.Collections.Generic;
using Core.Player;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private Projectile _projectile;
    [SerializeField] private int _damage = 5;

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if(collider2D.attachedRigidbody == null) return;

        if (_projectile.TeamIndex != -1)
        {
            if (collider2D.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
            {
                if (_projectile.TeamIndex == player.TeamIndex.Value)
                {
                    return;
                }
            }
        }

        if (collider2D.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(_damage);
        }
    }
}
