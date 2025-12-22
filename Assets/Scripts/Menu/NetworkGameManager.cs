using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkGameManager>();
            }
            return _instance;
        }
    }
    private static NetworkGameManager _instance;

    // clientId -> characterId
    private Dictionary<ulong, int> playerCharacters = new();
    private HashSet<int> takenCharacters = new();

    public NetworkList<PlayerSelectionState> SelectionStates;



    private void Awake()
    {
        //if (Instance == null) Instance = this;
        //else Destroy(gameObject);

        SelectionStates = new NetworkList<PlayerSelectionState>();
    }

    public override void OnNetworkSpawn()
    {
        DontDestroyOnLoad(gameObject);

        if (IsServer)
        {
            SelectionStates.Clear();
        }
    }

    // ===== ¬€¡Œ– =====
    [ServerRpc(RequireOwnership = false)]
    public void SelectCharacterServerRpc(ulong clientId, int characterId)
    {
        if (takenCharacters.Contains(characterId))
            return;

        CancelInternal(clientId);

        takenCharacters.Add(characterId);
        playerCharacters[clientId] = characterId;

        UpdateSelectionState(clientId, characterId);

        TryStartGame();
    }

    // ===== Œ“Ã≈Õ¿ =====
    [ServerRpc(RequireOwnership = false)]
    public void CancelSelectionServerRpc(ulong clientId)
    {
        CancelInternal(clientId);
        TryStartGame();
    }

    private void CancelInternal(ulong clientId)
    {
        if (!playerCharacters.ContainsKey(clientId))
            return;

        int charId = playerCharacters[clientId];
        takenCharacters.Remove(charId);
        playerCharacters.Remove(clientId);

        RemoveSelectionState(clientId);
    }

    // ===== —≈“≈¬Œ≈ —Œ—“ŒﬂÕ»≈ =====
    private void UpdateSelectionState(ulong clientId, int characterId)
    {
        RemoveSelectionState(clientId);
        SelectionStates.Add(new PlayerSelectionState
        {
            ClientId = clientId,
            CharacterId = characterId
        });
    }

    private void RemoveSelectionState(ulong clientId)
    {
        for (int i = 0; i < SelectionStates.Count; i++)
        {
            if (SelectionStates[i].ClientId == clientId)
            {
                SelectionStates.RemoveAt(i);
                break;
            }
        }
    }

    private void TryStartGame()
    {
        if (playerCharacters.Count ==
            NetworkManager.Singleton.ConnectedClients.Count &&
            playerCharacters.Count == 2)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                "Level1", LoadSceneMode.Single);
        }
    }

    public int GetCharacterForClient(ulong clientId)
    {
        return playerCharacters[clientId];
    }
}
