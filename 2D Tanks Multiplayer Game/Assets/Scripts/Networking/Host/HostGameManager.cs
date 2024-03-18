using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    private Allocation _allocation;
    private string _jointCode;
    private string _lobbyID;

    public NetworkServer NetworkServer { get; private set; }

    private const int MaxConnections = 20;
    
    public async Task StartHostAsync()
    {
        try
        {
            _allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }
        
        try
        {
            _jointCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log(_jointCode);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        
        //  ako nece da radi vrati na "udp"
        RelayServerData relayServerData = new RelayServerData(_allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject
                    (
                        visibility: DataObject.VisibilityOptions.Member,
                        value: _jointCode
                    )
                }
            };

            string playerName = PlayerPrefs.GetString(NameSelector.PLAYER_NAME_KEY, "newUser123");
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync( playerName+" lobby", MaxConnections, lobbyOptions);
            _lobbyID = lobby.Id;
            HostSingletone.Instance.StartCoroutine(HeartbeatLobby(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }

        NetworkServer = new NetworkServer(NetworkManager.Singleton);
        
        UserData userData = new UserData()
        {
            UserName = PlayerPrefs.GetString(NameSelector.PLAYER_NAME_KEY, "newUser123"),
            UserAuthID = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        
        NetworkManager.Singleton.StartHost();

        NetworkServer.OnClientLeft += HandleClientLeft;

        NetworkManager.Singleton.SceneManager.LoadScene(SceneController.GAME_SCENE, LoadSceneMode.Single);
    }

    private IEnumerator HeartbeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(_lobbyID);
            yield return delay;
        }
    }

    public void Dispose()
    {
        Shutdown();
    }

    public async void Shutdown()
    {
        HostSingletone.Instance.StopCoroutine(nameof(HeartbeatLobby));

        if (string.IsNullOrEmpty(_lobbyID))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(_lobbyID);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            _lobbyID = string.Empty;
        }
        
        NetworkServer.OnClientLeft += HandleClientLeft;
        
        NetworkServer.Dispose();
    }

    private void HandleClientLeft(string authID)
    {
        try
        {
            LobbyService.Instance.RemovePlayerAsync(_lobbyID, authID);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
