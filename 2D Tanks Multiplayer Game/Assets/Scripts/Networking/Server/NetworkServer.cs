using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager _networkManager;

    public Action<string> OnClientLeft;

    private Dictionary<ulong, string> _clientsIDToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> _authIDToUserData = new Dictionary<string, UserData>();
    
    public NetworkServer(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        _networkManager.ConnectionApprovalCallback += ApprovalCheck;
        _networkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);
        
        _clientsIDToAuth[request.ClientNetworkId] = userData.UserAuthID;
        _authIDToUserData[userData.UserAuthID] = userData;

        response.Approved = true;
        response.Position = SpawnPoint.GetRandomSpawnPos();
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true;
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
