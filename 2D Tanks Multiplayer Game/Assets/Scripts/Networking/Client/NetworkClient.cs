using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Client
{
    public class NetworkClient : IDisposable
    {
        private NetworkManager _networkManager;

        public NetworkClient(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void OnClientDisconnect(ulong clientID)
        {
            if(clientID != 0 && clientID != _networkManager.LocalClientId) return;

            Disconnect();
        }
        
        public void Disconnect()
        {
            if (!SceneController.Instance.IsActiveScene(SceneController.MAIN_MENU_SCENE))
            {
                SceneController.Instance.LoadScene(SceneController.MAIN_MENU_SCENE);
            }

            if (_networkManager.IsConnectedClient)
            {
                _networkManager.Shutdown();
            }
        }

        public void Dispose()
        {
            if(_networkManager == null) return;
            
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }

    }
}