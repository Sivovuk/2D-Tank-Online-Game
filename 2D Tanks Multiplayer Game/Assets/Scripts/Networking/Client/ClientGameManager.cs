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
        private MatchplayMatchmaker _matchmaker;
        public UserData UserData { get; private set; }

        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();

            _networkClient = new NetworkClient(NetworkManager.Singleton);
            _matchmaker = new MatchplayMatchmaker();

            AuthState authState = await AuthenticationWraper.DoAuth();

            if (authState == AuthState.Authenticated)
            {
                UserData = new UserData()
                {
                    UserName = PlayerPrefs.GetString(NameSelector.PLAYER_NAME_KEY, "newUser123"),
                    UserAuthID = AuthenticationService.Instance.PlayerId
                };

                return true;
            }

            return false;

        }

        public void StartClient(string ip, int port)
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, (ushort)port);
            ConnectClient();
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
            
            ConnectClient();
        }

        private void ConnectClient()
        {
            string payload = JsonUtility.ToJson(UserData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

            NetworkManager.Singleton.StartClient();
        }

        public async void MatchmakeAsync(bool isTeamQueue, Action<MatchmakerPollingResult> onMatchmakeResponse)
        {
            if (_matchmaker.IsMatchmaking)
            {
                return;
            }

            UserData.UserGamePreferences.GameQueue = isTeamQueue ? GameQueue.Team : GameQueue.Solo;
            MatchmakerPollingResult matchmakerPollingResult = await GetMatchAsync();
            onMatchmakeResponse?.Invoke(matchmakerPollingResult);
        }

        private async Task<MatchmakerPollingResult> GetMatchAsync()
        {
            MatchmakingResult matchmakingResult = await _matchmaker.Matchmake(UserData);

            if (matchmakingResult.result == MatchmakerPollingResult.Success)
            {
                StartClient(matchmakingResult.ip, matchmakingResult.port);
            }

            return matchmakingResult.result;
        }
        
        public async Task CancelMatchmaker()
        {
            await _matchmaker.CancelMatchmaking();
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