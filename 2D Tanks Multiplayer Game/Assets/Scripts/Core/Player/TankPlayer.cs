using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using DefaultNamespace.Networking.Server;
using Unity.Collections;

namespace Core.Player
{
    public class TankPlayer : NetworkBehaviour
    {
        [Header("References")] [SerializeField]
        private CinemachineVirtualCamera _followCamera;
        [SerializeField] private SpriteRenderer _minimapIcon;
        [SerializeField] private Texture2D _crosshair;

        [field:SerializeField] public Health Health { get; private set; }
        [field:SerializeField] public CoinWallet Wallet { get; private set; }

        [Header("Settings")] 
        [SerializeField] private int _ownerPriority = 15;
        [SerializeField] private Color _minimapIconColor;

        public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
        public NetworkVariable<int> TeamIndex = new NetworkVariable<int>();

        public static event Action<TankPlayer> OnPlayerSpawned;
        public static event Action<TankPlayer> OnPlayerDespawned; 

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                UserData userData = null;
                if (IsHost)
                {
                    userData = HostSingletone.Instance.HostGameManager.NetworkServer.GetUserDataByClientID(OwnerClientId);
                }
                else
                {
                    userData = ServerSingletone.Instance.ServerGameManager.NetworkServer.GetUserDataByClientID(OwnerClientId);
                }
                PlayerName.Value = userData.UserName;
                TeamIndex.Value = userData.TeamIndex;
                
                OnPlayerSpawned?.Invoke(this);
            }

            if (IsOwner)
            {
                _followCamera.Priority = _ownerPriority;
                _minimapIcon.color = _minimapIconColor;
                
                Cursor.SetCursor(_crosshair, new Vector2(_crosshair.width / 2, _crosshair.height / 2), CursorMode.Auto);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                OnPlayerDespawned?.Invoke(this);
            }
        }
        
    }
}