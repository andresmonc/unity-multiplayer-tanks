using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private LobbyItem lobbyItemPrefab;
    [SerializeField] private Transform lobbyItemParent;
    [SerializeField] private int lobbyItemCount = 10;
    private bool isJoining = false;
    private bool isRefreshing = false;

    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        Debug.Log("Refreshing list!");
        if (isRefreshing)
        {
            return;
        }
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = lobbyItemCount,
                Filters = new List<QueryFilter>(){
                    new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"
                    ),
                    new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0"
                    )
                }
            };
            QueryResponse lobbiesList = await Lobbies.Instance.QueryLobbiesAsync(options);
            Debug.Log("Lobbies List completed!");
            Debug.Log(lobbiesList.Results);
            RefreshLobbyDisplay(lobbiesList);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Unknown error while joining lobby: " + e);
        }
        isRefreshing = true;
    }

    private void RefreshLobbyDisplay(QueryResponse lobbiesList)
    {
        // Reinitialize existing lobby items or create new ones as needed
        int lobbyIndex = 0;
        for (int i = 0; i < lobbyItemParent.childCount && lobbyIndex < lobbiesList.Results.Count; i++)
        {
            // Reinitialize existing lobby item
            LobbyItem lobbyItem = lobbyItemParent.GetChild(i).GetComponent<LobbyItem>();
            lobbyItem.Initialize(this, lobbiesList.Results[lobbyIndex]);
            lobbyIndex++;
        }

        // If there are remaining lobbies, create new lobby items
        for (; lobbyIndex < lobbiesList.Results.Count; lobbyIndex++)
        {
            LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent).GetComponent<LobbyItem>();
            lobbyItem.Initialize(this, lobbiesList.Results[lobbyIndex]);
        }

        // Destroy any extra lobby items
        for (int i = lobbyIndex; i < lobbyItemParent.childCount; i++)
        {
            Destroy(lobbyItemParent.GetChild(i).gameObject);
        }
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining)
        {
            Debug.LogError("Already trying to join!");
            return;
        }
        isJoining = true;
        try
        {
            Debug.Log("Attempting to join lobby...");
            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joinedLobby.Data["JoinCode"].Value;
            await ClientSingleton.Instance.ClientGameManager.StartClientAsync(joinCode);
            Waiter.Wait(5, () => isJoining = false);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Unknown error while joining lobby: " + e);
            isJoining = false;
        }
    }

}
