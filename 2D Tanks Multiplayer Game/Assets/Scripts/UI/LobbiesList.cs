using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.UI;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private Transform _lobbyItemParent;
    [SerializeField] private LobbyItem _lobbyItemPrefab;
    
    private Lobby _joiningLobby;
    private bool isJoining;

    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if (isJoining) return;
        isJoining = true;

        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter
                (
                    field:QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value:"0"
                ),
                new QueryFilter
                (
                    field:QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value:"0"
                )
            };
            
            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            foreach (Transform child in _lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(_lobbyItemPrefab, _lobbyItemParent);
                lobbyItem.Initialise(this, lobby);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        

        isJoining = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining) return;
        isJoining = true;
        
        try
        {
             _joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
             string joinCode = _joiningLobby.Data["JoinCode"].Value;

             await ClientSingletone.Instance.ClientGameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        isJoining = false;
    }
}
