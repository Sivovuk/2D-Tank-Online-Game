using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerGameManager : IDisposable
{
    private string _serverIP;
    private int _serverPort;
    private int _queryPort;

    private MultiplayAllocationService _multiplayAllocationService;
    private MatchplayBackfiller _matchplayBackfiller;
    
    public NetworkServer NetworkServer { get; private set; }
    
    public ServerGameManager(string serverIP, int serverPort, int serverQPort, NetworkManager manager)
    {
        _serverIP = serverIP;
        _serverPort = serverPort;
        _queryPort = serverQPort;
        NetworkServer = new NetworkServer(manager);
        _multiplayAllocationService = new MultiplayAllocationService();
    }

    public async Task StartServerAsync()
    {
        await _multiplayAllocationService.BeginServerCheck();

        try
        {
            MatchmakingResults matchmakingPayload = await GetMatchmakerPayload();

            if (matchmakingPayload != null)
            {
                await StartBackfilled(matchmakingPayload);
                NetworkServer.OnUserJoined += UserJoined;
                NetworkServer.OnUserLeft += UserLeft;
            }
            else
            {
                Debug.LogWarning("Matchmaker payload timed outs");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

        if (!NetworkServer.OpenConnection(_serverIP, _serverPort))
        {
            Debug.LogError("Network server not started as expected!");
        }
        
        NetworkManager.Singleton.SceneManager.LoadScene(SceneController.GAME_SCENE, LoadSceneMode.Single);

    }

    private async Task StartBackfilled(MatchmakingResults matchmakingPayload)
    {
        _matchplayBackfiller = new MatchplayBackfiller($"{_serverIP}:{_serverPort}", 
            matchmakingPayload.QueueName, 
            matchmakingPayload.MatchProperties, 
            20);

        if (_matchplayBackfiller.NeedsPlayers())
        { 
            await _matchplayBackfiller.BeginBackfilling();
        }
    }

    private async Task<MatchmakingResults> GetMatchmakerPayload()
    {
        Task<MatchmakingResults> matchmakerPayloadTask = _multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();

        if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(20000)) == matchmakerPayloadTask)
        {
            return matchmakerPayloadTask.Result;
        }

        return null;
    }

    private void UserJoined(UserData user)
    {
        _matchplayBackfiller.AddPlayerToMatch(user);
        _multiplayAllocationService.AddPlayer();
        if (!_matchplayBackfiller.NeedsPlayers() && _matchplayBackfiller.IsBackfilling)
        {
            _ = _matchplayBackfiller.StopBackfill();
        }
    }

    private void UserLeft(UserData user)
    {
        int playerCount = _matchplayBackfiller.RemovePlayerFromMatch(user.UserAuthID);
        _multiplayAllocationService.RemovePlayer();

        if (playerCount <= 0)
        {
            CloseServer();
            return;
        }

        if (_matchplayBackfiller.NeedsPlayers() && !_matchplayBackfiller.IsBackfilling)
        {
            _ = _matchplayBackfiller.BeginBackfilling();
        }
    }

    private async void CloseServer()
    {
        await _matchplayBackfiller.StopBackfill();
        Dispose();
        Application.Quit();
    }

    public void Dispose()
    {
        NetworkServer.OnUserJoined -= UserJoined;
        NetworkServer.OnUserLeft -= UserLeft;
        
        _matchplayBackfiller?.Dispose();
        _multiplayAllocationService?.Dispose();
        NetworkServer?.Dispose();
    }

}
