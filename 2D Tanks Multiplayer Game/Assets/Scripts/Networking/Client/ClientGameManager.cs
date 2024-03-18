using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Client
{
//  this class authenticate Client and load main menu scene
    public class ClientGameManager : IDisposable
    {
        private JoinAllocation _joinAllocation;
        private NetworkClient _networkClient;

        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();

            _networkClient = new NetworkClient(NetworkManager.Singleton);

            AuthState authState = await AuthenticationWraper.DoAuth();

            if (authState == AuthState.NotAuthenticated)
            {
                return false;
            }

            return true;

        }

        public async Task StartClientAsync(string jointCode)
        {
            try
            {
                _joinAllocation = await Relay.Instance.JoinAllocationAsync(jointCode);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            //  ako nece da radi vrati na "udp"
            RelayServerData relayServerData = new RelayServerData(_joinAllocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            UserData userData = new UserData()
            {
                UserName = PlayerPrefs.GetString(NameSelector.PLAYER_NAME_KEY, "newUser123"),
                UserAuthID = AuthenticationService.Instance.PlayerId
            };
            string payload = JsonUtility.ToJson(userData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

            NetworkManager.Singleton.StartClient();
        }


        public void Disconnect()
        {
            _networkClient.Disconnect();
        }
        
        public void Dispose()
        {
            _networkClient?.Dispose();
        }
    }
}