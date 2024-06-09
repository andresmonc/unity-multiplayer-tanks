using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using System.Text;
using Unity.Services.Authentication;
public class HostGameManager : IDisposable
{
    private const int MaxConnections = 20;
    private const string GameSceneName = "Game";
    private Allocation allocation;
    private string joinCode;
    private string lobbyId;
    public NetworkServer NetworkServer { get; private set; }

    public async Task StartHostAsync()
    {
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
            joinCode = await GetJoinCode();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        try
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode",new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: joinCode
                    )
                }
            }
            };
            string playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Player#" + UnityEngine.Random.Range(100000, 999999));
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", MaxConnections, createLobbyOptions);
            lobbyId = lobby.Id;
            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }
        NetworkManager networkManager = NetworkManager.Singleton;
        NetworkServer = new NetworkServer(networkManager);
        UserData userData = new UserData
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Player#" + UnityEngine.Random.Range(100000, 999999)),
            userAuthID = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        networkManager.StartHost();
        networkManager.SceneManager.LoadScene(GameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public async Task<string> GetJoinCode()
    {
        string code = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log(code);
        return code;
    }

    private IEnumerator HeartBeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            Debug.Log("Heartbeat sent");
            yield return delay;
        }
    }

    public void Dispose()
    {
        Shutdown();
    }

    public async void Shutdown()
    {
        if (HostSingleton.Instance != null)
        {
            HostSingleton.Instance.StopCoroutine(nameof(HeartBeatLobby));
        }
        if (!string.IsNullOrEmpty(lobbyId))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
            lobbyId = string.Empty;
        }
        NetworkServer?.Dispose();    }
}
