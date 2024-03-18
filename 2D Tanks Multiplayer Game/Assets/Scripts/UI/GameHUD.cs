using System.Collections;
using System.Collections.Generic;
using Networking.Client;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingletone.Instance.HostGameManager.Shutdown();
        }

        ClientSingletone.Instance.ClientGameManager.Disconnect();
    }
}
