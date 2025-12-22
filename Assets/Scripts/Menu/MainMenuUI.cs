using Unity.Netcode;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject characterSelectPanel;

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        characterSelectPanel.SetActive(true);
    }

    public void Client()
    {
        NetworkManager.Singleton.StartClient();
        characterSelectPanel.SetActive(true);
    }
}
