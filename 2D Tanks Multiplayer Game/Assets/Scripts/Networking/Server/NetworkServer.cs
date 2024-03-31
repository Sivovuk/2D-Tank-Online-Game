using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager _networkManager;
    private NetworkObject _playerPrefab;

    public Action<UserData> OnUserJoined;
    public Action<UserData> OnUserLeft;

    public Action<string> OnClientLeft;

    private Dictionary<ulong, string> _clientsIDToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> _authIDToUserData = new Dictionary<string, UserData>();
    
    public NetworkServer(NetworkManager networkManager, NetworkObject playerPrefab)
    {
        _networkManager = networkManager;
        _playerPrefab = playerPrefab;

        _networkManager.ConnectionApprovalCallback += ApprovalCheck;
        _networkManager.OnServerStarted += OnNetworkReady;
    }

    public bool OpenConnection(string IP, int port)
    {
        UnityTransport transport = _networkManager.gameObject.GetComponent<UnityTransport>();
        transport.SetConnectionData(IP, (ushort)port);
        return _networkManager.StartServer();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);
        
        _clientsIDToAuth[request.ClientNetworkId] = userData.UserAuthID;
        _authIDToUserData[userData.UserAuthID] = userData;
        OnUserJoined?.Invoke(userData);

        _ = SpawnPlayerDelayed(request.ClientNetworkId);
        response.Approved = true;
        response.CreatePlayerObject = false;
    }

    private async Task SpawnPlayerDelayed(ulong clientID)
    {
        await Task.Delay(1000);

        NetworkObject playerInstance = GameObject.Instantiate(_playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        playerInstance.SpawnAsPlayerObject(clientID);
    }

    private void OnNetworkReady()
    {
        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientID)
    {
        if (_clientsIDToAuth.TryGetValue(clientID, out string authID))
        {
            _clientsIDToAuth.Remove(clientID);
            OnUserLeft?.Invoke(_authIDToUserData[authID]);
            _authIDToUserData.Remove(authID);
            OnClientLeft?.Invoke(authID);
        }
    }

    public UserData GetUserDataByClientID(ulong clientID)
    {
        if (_clientsIDToAuth.TryGetValue(clientID, out string authID))
        {
            if (_authIDToUserData.TryGetValue(authID, out UserData data))
            {
                return data;
            }
            return null;
        }
        return null;
    }

    public void Dispose()
    {
        if(_networkManager == null) return;
        
        _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        _networkManager.OnServerStarted -= OnNetworkReady;
        _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;

        if (_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }
}
