using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Player;
using Networking.Client;
using Unity.Netcode;
using UnityEngine;

namespace UI.Leaderboard
{
    public class Leaderboard : NetworkBehaviour
    {
        [SerializeField] private Transform _leaderboardEntityHolder;
        [SerializeField] private Transform _teamLeaderboardEntityHolder;
        [SerializeField] private GameObject _teamLeaderboardBackground;
        [SerializeField] private LeaderboardEntityDisplay _leaderboardEntityPrefab;
        [SerializeField] private int _entitiesToDisplay = 8;
        [SerializeField] private Color _ownerColor;
        [SerializeField] private string[] _teamNames;
        [SerializeField] private TeamColorLookup _teamColorLookup;
        
        private NetworkList<LeaderboardEntityState> _leaderboardEntities;
        private List<LeaderboardEntityDisplay> _playerDisplays = new List<LeaderboardEntityDisplay>();
        private List<LeaderboardEntityDisplay> _teamDisplays = new List<LeaderboardEntityDisplay>();
        
        private void Awake()
        {
            _leaderboardEntities = new NetworkList<LeaderboardEntityState>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                if (ClientSingletone.Instance.ClientGameManager.UserData.UserGamePreferences.GameQueue == GameQueue.Team)
                {
                    _teamLeaderboardBackground.SetActive(true);
                    for (int i = 0; i < _teamNames.Length; i++)
                    {
                        LeaderboardEntityDisplay team = Instantiate(_leaderboardEntityPrefab, _teamLeaderboardEntityHolder);
                        team.Initialise(i, _teamNames[i], 0);
                        Color color = _teamColorLookup.GetTeamColor(i);
                        team.SetColor(color);
                        _teamDisplays.Add(team);
                    }
                }
                
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
                    if (!_playerDisplays.Any(x => x.ClientID == changeEvent.Value.ClientID))
                    {
                        LeaderboardEntityDisplay leaderboardEntity = Instantiate(_leaderboardEntityPrefab, _leaderboardEntityHolder);
                        leaderboardEntity.Initialise(changeEvent.Value.ClientID, changeEvent.Value.PlayerName, changeEvent.Value.Coins);
                        if (NetworkManager.Singleton.LocalClientId == changeEvent.Value.ClientID)
                        {
                            leaderboardEntity.SetColor(_ownerColor);
                        }

                        _playerDisplays.Add(leaderboardEntity);
                    }
                    break;
                case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                    LeaderboardEntityDisplay displayToRemove = _playerDisplays.FirstOrDefault(x => x.ClientID == changeEvent.Value.ClientID);
                    if (displayToRemove != null)
                    {
                        displayToRemove.transform.SetParent(null);
                        Destroy(displayToRemove.gameObject);
                        _playerDisplays.Remove(displayToRemove);
                    }
                    break;
                case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                    LeaderboardEntityDisplay displayToUpdate =
                        _playerDisplays.FirstOrDefault(x => x.ClientID == changeEvent.Value.ClientID);
                    if (displayToUpdate != null)
                    {
                        displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                    }
                    break;
            }
            
            _playerDisplays.Sort((x,y) => y.Coins.CompareTo(x.Coins));

            for (int i = 0; i < _playerDisplays.Count; i++)
            {
                _playerDisplays[i].transform.SetSiblingIndex(i);
                _playerDisplays[i].UpdateText();
                _playerDisplays[i].gameObject.SetActive(i <= _entitiesToDisplay - 1);
            }

            LeaderboardEntityDisplay myDisplay =
                _playerDisplays.FirstOrDefault(x => x.ClientID == NetworkManager.Singleton.LocalClientId);
            if (myDisplay != null)
            {
                if (myDisplay.transform.GetSiblingIndex() >= _entitiesToDisplay)
                {
                    _leaderboardEntityHolder.GetChild(_entitiesToDisplay-1).gameObject.SetActive(false);
                    myDisplay.gameObject.SetActive(true);
                }
            }

            if (!_teamLeaderboardBackground.activeSelf) return;
            LeaderboardEntityDisplay teamDisplay =
                _teamDisplays.FirstOrDefault(x => x.TeamIndex == changeEvent.Value.TeamIndex);

            if (teamDisplay != null)
            {
                if (changeEvent.Type == NetworkListEvent<LeaderboardEntityState>.EventType.Remove)
                {
                    teamDisplay.UpdateCoins(teamDisplay.Coins - changeEvent.Value.Coins);
                }
                else
                {
                    teamDisplay.UpdateCoins(teamDisplay.Coins + (changeEvent.Value.Coins - changeEvent.PreviousValue.Coins));
                }
            }
            
            _teamDisplays.Sort((x,y) => y.Coins.CompareTo(x.Coins));

            for (int i = 0; i < _teamDisplays.Count; i++)
            {
                _teamDisplays[i].transform.SetSiblingIndex(i);
                _teamDisplays[i].UpdateText();
            }
        }
        
        private void HandlePlayerSpawn(TankPlayer player)
        {
            _leaderboardEntities.Add(new LeaderboardEntityState
            {
                ClientID = player.OwnerClientId,
                TeamIndex = player.TeamIndex.Value,
                PlayerName = player.PlayerName.Value,
                Coins = 0
            });

            player.Wallet.TotalCoins.OnValueChanged += (oldCoins, newCoins) => HandleCoinsChange(player.OwnerClientId, newCoins);
        }

        private void HandlePlayerDespawn(TankPlayer player)
        {
            if(_leaderboardEntities.Count <= 0) return;
            
            if(IsServer && player.OwnerClientId == OwnerClientId) return;
            
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
                    TeamIndex = _leaderboardEntities[i].TeamIndex,
                    PlayerName = _leaderboardEntities[i].PlayerName,
                    Coins = coins
                };
                
                return;
            }
        }

    }
}