﻿using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace TankGame.UI
{
    public class LobbyItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _lobbyNameText;
        [SerializeField] private TMP_Text _lobbyPlayersText;

        private LobbiesList _lobbiesList;
        private Lobby _lobby;
        
        public void Initialise(LobbiesList lobbiesList, Lobby lobby)
        {
            _lobbiesList = lobbiesList;
            _lobby = lobby;
            _lobbyNameText.text = lobby.Name;
            _lobbyPlayersText.text = "Players :\n" + lobby.Players.Count + " / " + lobby.MaxPlayers;
        }

        public void Join()
        {
            _lobbiesList.JoinAsync(_lobby);
        }
    }
}