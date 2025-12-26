using UnityEngine;

public class SpearController : MonoBehaviour
{
    [SerializeField] private GameObject spearPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 20f;

    private Spear currentSpear;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Throw();

        if (Input.GetMouseButtonDown(1))
            Recall();
    }

    private void Throw()
    {
        if (currentSpear != null) return;

        GameObject obj = Instantiate(spearPrefab, throwPoint.position, Quaternion.identity);
        currentSpear = obj.GetComponent<Spear>();

        // ВАЖНО: цель возврата задаётся сразу
        currentSpear.Init(transform);

        currentSpear.Throw(GetThrowDirection() * throwForce);
    }

    private void Recall()
    {
        if (currentSpear == null) return;
        if (currentSpear.IsFlying) return;

        currentSpear.StartRecall();
        currentSpear = null;
    }

    private Vector2 GetThrowDirection()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;
        return (mouse - throwPoint.position).normalized;
    }
}
