using System;
using System.Collections;
using System.Collections.Generic;
using Networking.Client;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : NetworkBehaviour
{
    [SerializeField] private TMP_Text _lobbyCodeTMP;

    private NetworkVariable<FixedString32Bytes> _lobbyCode = new NetworkVariable<FixedString32Bytes>("");
    
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            _lobbyCode.OnValueChanged += HandleLobbyCodeChange;
            HandleLobbyCodeChange("", _lobbyCode.Value);
        }
        
        if (!IsHost) return;

        _lobbyCodeTMP.text = HostSingletone.Instance.HostGameManager.JointCode;
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            _lobbyCode.OnValueChanged -= HandleLobbyCodeChange;
        }
    }

    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingletone.Instance.HostGameManager.Shutdown();
        }

        ClientSingletone.Instance.ClientGameManager.Disconnect();
    }
    
    private void HandleLobbyCodeChange(FixedString32Bytes oldCode, FixedString32Bytes newCode)
    {
        _lobbyCodeTMP.text = newCode.ToString();
    }

}
