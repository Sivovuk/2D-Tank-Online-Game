using System.Collections;
using Core.Player;
using Unity.Netcode;
using UnityEngine;

namespace DefaultNamespace.Core.Combat
{
    public class RespawnHandler : NetworkBehaviour
    {
        [SerializeField] private TankPlayer _playerPrefab;

        [SerializeField] private float _keptCoinPercentage;

        public override void OnNetworkSpawn()
        {
            if(!IsServer) return;

            TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

            foreach (TankPlayer player in players)
            {
                HandlePlayerSpawn(player);
            }

            TankPlayer.OnPlayerDespawned += HandlePlayerDespawn;
            TankPlayer.OnPlayerSpawned += HandlePlayerSpawn;
        }

        public override void OnNetworkDespawn()
        {
            if(!IsServer) return;
            
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawn;
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawn;
        }

        private void HandlePlayerSpawn(TankPlayer player)
        {
            player.Health.OnDie += (health) => HandlePlayerDie(player);
        }

        private void HandlePlayerDespawn(TankPlayer player)
        {
            player.Health.OnDie -= (health) => HandlePlayerDie(player);
        }

        private void HandlePlayerDie(TankPlayer player)
        {
            int coins =(int) (player.Wallet.TotalCoins.Value * (_keptCoinPercentage / 100));
            Destroy(player.gameObject);

            StartCoroutine(RespawnPlayer(player.OwnerClientId, coins));
        }

        private IEnumerator RespawnPlayer(ulong ownerClientId, int coinsValue)
        {
            yield return null;

            TankPlayer playerInstance = Instantiate(_playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
            playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);
            playerInstance.Wallet.TotalCoins.Value = coinsValue;
        }
    }
}