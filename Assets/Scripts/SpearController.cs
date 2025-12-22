using Unity.Netcode;
using UnityEngine;

public class SpearController : NetworkBehaviour
{
    [SerializeField] private GameObject spearPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 20f;

    // ТОЛЬКО сервер решает, есть ли копьё
    private NetworkObject currentSpear;

    private void Update()
    {
        if (!IsOwner) return;

        // ЛКМ — запрос на бросок
        if (Input.GetMouseButtonDown(0))
        {
            RequestThrowServerRpc();
        }

        // ПКМ — возврат
        if (Input.GetMouseButtonDown(1))
        {
            RequestRecallServerRpc();
        }
    }

    // ------------------------
    // Запрос на бросок
    // ------------------------
    [ServerRpc]
    private void RequestThrowServerRpc(ServerRpcParams rpcParams = default)
    {
        if (currentSpear != null) return; // ? защита от спама

        GameObject spearObj = Instantiate(spearPrefab, throwPoint.position, Quaternion.identity);
        NetworkObject netObj = spearObj.GetComponent<NetworkObject>();

        netObj.Spawn();

        Spear spear = spearObj.GetComponent<Spear>();
        spear.Throw(GetThrowDirection() * throwForce);

        currentSpear = netObj;
    }

    // ------------------------
    // Запрос на возврат
    // ------------------------
    [ServerRpc]
    private void RequestRecallServerRpc()
    {
        if (currentSpear == null) return;

        Spear spear = currentSpear.GetComponent<Spear>();
        spear.StartRecall(transform.position);

        currentSpear = null;
    }

    // ------------------------
    // Направление броска
    // ------------------------
    private Vector2 GetThrowDirection()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;
        return (mouse - throwPoint.position).normalized;
    }
}
