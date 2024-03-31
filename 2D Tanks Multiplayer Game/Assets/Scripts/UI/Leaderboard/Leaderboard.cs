using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Player;
using Unity.Netcode;
using UnityEngine;

namespace UI.Leaderboard
{
    public class Leaderboard : NetworkBehaviour
    {
        [SerializeField] private Transform _leaderboardEntityHolder;
        [SerializeField] private LeaderboardEntityDisplay _leaderboardEntityPrefab;
        [SerializeField] private int _entitiesToDisplay = 8;
        
        private NetworkList<LeaderboardEntityState> _leaderboardEntities;
        private List<LeaderboardEntityDisplay> _entityDisplays = new List<LeaderboardEntityDisplay>();

        private void Awake()
        {
            _leaderboardEntities = new NetworkList<LeaderboardEntityState>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                _leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChange;
                foreach (LeaderboardEntityState entity in _leaderboardEntities)
                {
                    HandleLeaderboardEntitiesChange
                    (
                        new NetworkListEvent<LeaderboardEntityState>()
                        {
                            Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                            Value = entity
                        }
                    );
                }
            }
            
            if (IsServer)
            {
                TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

                foreach (TankPlayer player in players)
                {
                    HandlePlayerSpawn(player);
                }

                TankPlayer.OnPlayerSpawned += HandlePlayerSpawn;
                TankPlayer.OnPlayerDespawned += HandlePlayerDespawn;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
            {
                _leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChange;
            }
            
            if (IsServer)
            {
                TankPlayer.OnPlayerSpawned -= HandlePlayerSpawn;
                TankPlayer.OnPlayerDespawned -= HandlePlayerDespawn;
            }
        }

        private void HandleLeaderboardEntitiesChange(NetworkListEvent<LeaderboardEntityState> changeEvent)
        {
            if (!gameObject.scene.isLoaded) return;
            
            switch (changeEvent.Type)
            {
                case NetworkListEvent<LeaderboardEntityState>.EventType.Add :
                    if (!_entityDisplays.Any(x => x.ClientID == changeEvent.Value.ClientID))
                    {
                        LeaderboardEntityDisplay leaderboardEntity = Instantiate(_leaderboardEntityPrefab, _leaderboardEntityHolder);
                        leaderboardEntity.Initialise(changeEvent.Value.ClientID, changeEvent.Value.PlayerName, changeEvent.Value.Coins);
                        _entityDisplays.Add(leaderboardEntity);
                    }
                    break;
                case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                    LeaderboardEntityDisplay displayToRemove = _entityDisplays.FirstOrDefault(x => x.ClientID == changeEvent.Value.ClientID);
                    if (displayToRemove != null)
                    {
                        displayToRemove.transform.SetParent(null);
                        Destroy(displayToRemove.gameObject);
                        _entityDisplays.Remove(displayToRemove);
                    }
                    break;
                case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                    LeaderboardEntityDisplay displayToUpdate =
                        _entityDisplays.FirstOrDefault(x => x.ClientID == changeEvent.Value.ClientID);
                    if (displayToUpdate != null)
                    {
                        displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                    }
                    break;
            }
            
            _entityDisplays.Sort((x,y) => y.Coins.CompareTo(x.Coins));

            for (int i = 0; i < _entityDisplays.Count; i++)
            {
                _entityDisplays[i].transform.SetSiblingIndex(i);
                _entityDisplays[i].UpdateText();
                _entityDisplays[i].gameObject.SetActive(i <= _entitiesToDisplay - 1);
            }

            LeaderboardEntityDisplay myDisplay =
                _entityDisplays.FirstOrDefault(x => x.ClientID == NetworkManager.Singleton.LocalClientId);
            if (myDisplay != null)
            {
                if (myDisplay.transform.GetSiblingIndex() >= _entitiesToDisplay)
                {
                    _leaderboardEntityHolder.GetChild(_entitiesToDisplay-1).gameObject.SetActive(false);
                    myDisplay.gameObject.SetActive(true);
                }
            }
        }
        
        
        private void HandlePlayerSpawn(TankPlayer player)
        {
            _leaderboardEntities.Add(new LeaderboardEntityState
            {
                ClientID = player.OwnerClientId,
                PlayerName = player.PlayerName.Value,
                Coins = 0
            });

            player.Wallet.TotalCoins.OnValueChanged += (oldCoins, newCoins) => HandleCoinsChange(player.OwnerClientId, newCoins);
        }

        private void HandlePlayerDespawn(TankPlayer player)
        {
            if(_leaderboardEntities.Count <= 0) return;
            
            foreach (LeaderboardEntityState entity in _leaderboardEntities)
            {
                if (entity.ClientID != player.OwnerClientId) continue;

                _leaderboardEntities.Remove(entity);
                break;
            }
            
            player.Wallet.TotalCoins.OnValueChanged -= (oldCoins, newCoins) => HandleCoinsChange(player.OwnerClientId, newCoins);

        }

        private void HandleCoinsChange(ulong clientID, int coins)
        {
            for (int i = 0; i < _leaderboardEntities.Count; i++)
            {
                if (_leaderboardEntities[i].ClientID != clientID) continue;

                _leaderboardEntities[i] = new LeaderboardEntityState
                {
                    ClientID = _leaderboardEntities[i].ClientID,
                    PlayerName = _leaderboardEntities[i].PlayerName,
                    Coins = coins
                };
                
                return;
            }
        }

    }
}