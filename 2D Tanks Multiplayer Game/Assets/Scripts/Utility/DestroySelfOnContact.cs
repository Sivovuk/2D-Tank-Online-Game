using System.Collections;
using System.Collections.Generic;
using Core.Player;
using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
    [SerializeField] private Projectile _projectile;
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (_projectile.TeamIndex != -1)
        {
            if (other.attachedRigidbody != null)
            {

                if (other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
                {
                    if (player.TeamIndex.Value == _projectile.TeamIndex)
                    {
                        return;
                    }
                }
            }
        }

        Destroy(gameObject);
    }
}
