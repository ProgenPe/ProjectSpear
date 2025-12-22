using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public GameObject[] characterPrefabs;
    public Transform[] spawnPoints;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        var gameManager = NetworkGameManager.Instance;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            int charId = gameManager.GetCharacterForClient(client.ClientId);

            GameObject prefab = characterPrefabs[charId];
            int index = (int)(client.ClientId % (ulong)spawnPoints.Length);
            Transform spawnPoint = spawnPoints[index];

            GameObject player = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.ClientId);
        }
    }
}
