using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using Unity.Collections;

namespace Core.Player
{
    public class TankPlayer : NetworkBehaviour
    {
        [Header("References")] [SerializeField]
        private CinemachineVirtualCamera _followCamera;

        [Header("Settings")] [SerializeField] private int _ownerPriority = 15;

        public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                UserData userData =
                    HostSingletone.Instance.HostGameManager.NetworkServer.GetUserDataByClientID(OwnerClientId);
                PlayerName.Value = userData.UserName;
            }

            if (IsOwner)
            {
                _followCamera.Priority = _ownerPriority;
            }
        }
    }
}