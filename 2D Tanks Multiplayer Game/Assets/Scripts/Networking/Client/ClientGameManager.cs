using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

//  this class authenticate Client and load main menu scene
public class ClientGameManager
{
    private JoinAllocation _joinAllocation;
    private const string MenuSceneName = "MainMenu";
    private const string GameSceneName = "Game";
    
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        AuthState authState = await AuthenticationWraper.DoAuth();

        if (authState == AuthState.NotAuthenticated)
        {
            return false;
        }
        
        return true;
        
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
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

        NetworkManager.Singleton.StartClient();
    }
}
