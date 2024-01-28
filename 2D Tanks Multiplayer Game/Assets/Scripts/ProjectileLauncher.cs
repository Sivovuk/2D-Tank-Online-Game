using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour {
   
    [Header("References")] 
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private GameObject _serverProjectilePrefab;
    [SerializeField] private GameObject _clientProjectilePrefab;
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private Collider2D _playerCollider;

    [Header("Settings")]
    [SerializeField] private float _projectileSpeed = 5;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _muzzleFlashDuration;

    private bool _shouldFire;
    private float _previousFireTime;
    private float _muzzleFlashTimer;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;

        _inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) return;
        
        _inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        this._shouldFire = shouldFire;
    }

    void Update()
    {
        if(_muzzleFlashTimer > 0)
        {
            _muzzleFlashTimer -= Time.deltaTime;

            if(_muzzleFlashTimer <= 0f)
            {
                _muzzleFlash.SetActive(false);
            }
        }

        if(!IsOwner) return;

        if(!_shouldFire) return;
        
        if (Time.time < (1 / _fireRate) + _previousFireTime) { return; }

        PrimaryFireServerRPC(_projectileSpawnPoint.position, _projectileSpawnPoint.up);

        SpawnDummyProjectile(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
        
        _previousFireTime = Time.time;

    }

    [ServerRpc]
    private void PrimaryFireServerRPC(Vector3 spawnPosition, Vector3 direction)
    {
        GameObject projectileSpawn = Instantiate(_serverProjectilePrefab, spawnPosition, Quaternion.identity);

        projectileSpawn.transform.up = direction;

        Physics2D.IgnoreCollision(_playerCollider, projectileSpawn.GetComponent<Collider2D>());

        if (projectileSpawn.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }

        if(projectileSpawn.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * _projectileSpeed;
        }

        SpawnDummyProjectileClientRPC(spawnPosition, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRPC(Vector3 spawnPosition, Vector3 direction)
    {
        if(IsOwner) return;

        SpawnDummyProjectile(spawnPosition, direction);
    }
    
    
    private void SpawnDummyProjectile(Vector3 spawnPosition, Vector3 direction)
    {
        _muzzleFlash.SetActive(true);
        _muzzleFlashTimer = _muzzleFlashDuration;

        GameObject projectileSpawn = Instantiate(_clientProjectilePrefab, spawnPosition, Quaternion.identity);

        projectileSpawn.transform.up = direction;
        
        Physics2D.IgnoreCollision(_playerCollider, projectileSpawn.GetComponent<Collider2D>());

        if(projectileSpawn.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * _projectileSpeed;
        }
    }

}
