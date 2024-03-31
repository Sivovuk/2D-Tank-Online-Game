using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Networking.Client;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _queueStatusText;
    [SerializeField] private TMP_Text _queueTimerText;
    [SerializeField] private TMP_Text _findMatchButtonText;
    [SerializeField] private TMP_InputField _jointCodeField;
    [SerializeField] private Toggle _teamToggle;
    [SerializeField] private Toggle _privateToggle;

    private bool isMatchmaking;
    private bool isCancelling;
    private bool isBusy;

    private float _timeInQueue;
    
    private void Start()
    {
        if (ClientSingletone.Instance == null) return;
        
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        
        _queueStatusText.text = string.Empty;
        _queueTimerText.text = string.Empty;
    }

    private void Update()
    {
        if (isMatchmaking)
        {
            _timeInQueue += Time.deltaTime;
            TimeSpan ts = TimeSpan.FromSeconds(_timeInQueue);
            _queueTimerText.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            if (_timeInQueue >= 60)
            {
                FindMatchPressed();
            }
        }
    }

    public async void FindMatchPressed()
    {
        if (isCancelling) return;
        
        if (isMatchmaking)
        {
            _queueStatusText.text = "Canceling...";
            isCancelling = true;
            await ClientSingletone.Instance.ClientGameManager.CancelMatchmaker();
            isCancelling = false;
            isMatchmaking = false;
            isBusy = false;
            _findMatchButtonText.text = "Find Match";
            _queueStatusText.text = string.Empty;
            _queueTimerText.text = String.Empty;
            return;
        }

        if (isBusy) return;
        
        ClientSingletone.Instance.ClientGameManager.MatchmakeAsync(_teamToggle.isOn, OnMatchMade);
        _findMatchButtonText.text = "Cancel";
        _queueStatusText.text = "Searching...";
        _timeInQueue = 0;
        isMatchmaking = true;
        isBusy = true;

    }

    private void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                _queueStatusText.text = "Connecting...";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                _queueStatusText.text = "TicketCreationError";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                _queueStatusText.text = "TicketCancellationError";
                break;
            case MatchmakerPollingResult.TicketRetrievalError:
                _queueStatusText.text = "TicketRetrievalError";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                _queueStatusText.text = "MatchAssignmentError";
                break;
        }
    }
    
    public async void StartHost()
    {
        if(isBusy) return;
        
        isBusy = true;
        
        await HostSingletone.Instance.HostGameManager.StartHostAsync(_privateToggle.isOn);
        
        isBusy = false;
    }

    public async void StartClient()
    {
        if(isBusy) return;
        
        isBusy = true;
        
        await ClientSingletone.Instance.ClientGameManager.StartClientAsync(_jointCodeField.text);
        
        isBusy = false;
    }
    
    public async void JoinAsync(Lobby lobby)
    {
        if (isBusy) return;
        isBusy = true;
        
        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingletone.Instance.ClientGameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        isBusy = false;
    }
}
