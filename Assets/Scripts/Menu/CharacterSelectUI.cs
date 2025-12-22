using Unity.Netcode;
using UnityEngine;
using TMPro;

public class CharacterSelectUI : MonoBehaviour
{
    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;

    private void Start()
    {
        NetworkGameManager.Instance.SelectionStates
            .OnListChanged += UpdateUI;
    }

    private void UpdateUI(NetworkListEvent<PlayerSelectionState> changeEvent)
    {
        player1Text.text = "Player 1: Ч";
        player2Text.text = "Player 2: Ч";

        foreach (var state in NetworkGameManager.Instance.SelectionStates)
        {
            string text = $"Character {state.CharacterId}";

            if (state.ClientId ==
                NetworkManager.Singleton.LocalClientId)
            {
                player1Text.text = $"You: {text}";
            }
            else
            {
                player2Text.text = $"Friend: {text}";
            }
        }
    }

    public void SelectCharacter(int id)
    {
        if (NetworkGameManager.Instance == null)
        {
            Debug.LogWarning("NetworkGameManager ещЄ не готов!");
            return;
        }

        NetworkGameManager.Instance.SelectCharacterServerRpc(
            NetworkManager.Singleton.LocalClientId, id);
    }

    public void Cancel()
    {
        NetworkGameManager.Instance
            .CancelSelectionServerRpc(
                NetworkManager.Singleton.LocalClientId);
    }
}
