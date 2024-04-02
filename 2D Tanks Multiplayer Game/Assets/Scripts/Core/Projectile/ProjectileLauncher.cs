using Core.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProjectileLauncher : NetworkBehaviour
{

    [Header("References")] 
    [SerializeField] private TankPlayer _player;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private GameObject _serverProjectileAntyPrefab;
    [SerializeField] private GameObject _clientProjectileAntyPrefab;
    [SerializeField] private GameObject _serverProjectilePeircingPrefab;
    [SerializeField] private GameObject _clientProjectilePeircingPrefab;
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private Collider2D _playerCollider;
    [SerializeField] private CoinWallet _coinWallet;
    [SerializeField] private ShellSelection _shellSelection;

    [Header("Settings")]
    [SerializeField] private float _projectileSpeed = 5;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _muzzleFlashDuration;

    private bool _isPointerOverUI;
    private bool _shouldFire;
    private float _timer;
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
        if (_isPointerOverUI) return;
        
        _shouldFire = shouldFire;
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

        _isPointerOverUI = EventSystem.current.IsPointerOverGameObject();

        if(_timer > 0)
            _timer -= Time.deltaTime;
        
        if(!_shouldFire) return;
        
        if (_timer > 0) { return; }
        
        if(_coinWallet.TotalCoins.Value < _shellSelection.GetActiveShellCost()) return;

        PrimaryFireServerRPC(_projectileSpawnPoint.position, _projectileSpawnPoint.up);

        SpawnDummyProjectile(_projectileSpawnPoint.position, _projectileSpawnPoint.up, _player.TeamIndex.Value);
        
        _timer = 1 / _fireRate;

    }

    [ServerRpc]
    private void PrimaryFireServerRPC(Vector3 spawnPosition, Vector3 direction)
    {
        if (_coinWallet.TotalCoins.Value < _shellSelection.GetActiveShellCost()) return;
        
        _coinWallet.SpendCoins(_shellSelection.GetActiveShellCost());
        
        GameObject projectileSpawn = Instantiate(GetShellTypeServer(), spawnPosition, Quaternion.identity);

        projectileSpawn.transform.up = direction;

        Physics2D.IgnoreCollision(_playerCollider, projectileSpawn.GetComponent<Collider2D>());

        if (projectileSpawn.TryGetComponent<Projectile>(out Projectile projectile))
        {
            projectile.Initialise(_player.TeamIndex.Value);
        }
        
        if(projectileSpawn.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * _projectileSpeed;
        }

        SpawnDummyProjectileClientRPC(spawnPosition, direction, _player.TeamIndex.Value);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRPC(Vector3 spawnPosition, Vector3 direction, int teamIndex)
    {
        if(IsOwner) return;

        SpawnDummyProjectile(spawnPosition, direction, teamIndex);
    }
    
    
    private void SpawnDummyProjectile(Vector3 spawnPosition, Vector3 direction, int teamIndex)
    {
        _muzzleFlash.SetActive(true);
        _muzzleFlashTimer = _muzzleFlashDuration;

        GameObject projectileSpawn = Instantiate(GetShellTypeClient(), spawnPosition, Quaternion.identity);

        projectileSpawn.transform.up = direction;
        
        Physics2D.IgnoreCollision(_playerCollider, projectileSpawn.GetComponent<Collider2D>());

        if(projectileSpawn.TryGetComponent<Projectile>(out Projectile projectile))
        {
            projectile.Initialise(teamIndex);
        }
        
        if(projectileSpawn.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * _projectileSpeed;
        }
    }

    private GameObject GetShellTypeClient()
    {
        if (_shellSelection.GetActiveShell() == ShellSelection.TankShells.AntyTankShell)
        {
            return _clientProjectileAntyPrefab;
        }
        else
        {
            return _clientProjectilePeircingPrefab;
        }
    }
    
    private GameObject GetShellTypeServer()
    {
        if (_shellSelection.GetActiveShell() == ShellSelection.TankShells.AntyTankShell)
        {
            return _serverProjectileAntyPrefab;
        }
        else
        {
            return _serverProjectilePeircingPrefab;
        }
    }
}
