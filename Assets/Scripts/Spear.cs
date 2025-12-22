using Unity.Netcode;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Spear : NetworkBehaviour
{
    [Header("Stick")]
    [SerializeField] private float maxStickAngle = 45f;

    [Header("Bounce")]
    [SerializeField] private float bounceMultiplier = 0.5f;

    [Header("Recall")]
    [SerializeField] private float recallDelay = 0.4f;
    [SerializeField] private float recallSpeed = 25f;

    private Rigidbody2D rb;
    private Collider2D col;

    private bool isStuck;
    private bool isRecalling;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 1f;
        rb.simulated = true;
    }

    // ======================================================
    // БРОСОК (ТОЛЬКО СЕРВЕР)
    // ======================================================
    public void Throw(Vector2 velocity)
    {
        if (!IsServer) return;

        isStuck = false;
        isRecalling = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = velocity;

        RotateAlongVelocity();
        IgnorePlayersCollision(true);
    }

    private void Update()
    {
        if (!IsServer) return;
        if (isStuck || isRecalling) return;

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
            RotateAlongVelocity();
    }

    private void RotateAlongVelocity()
    {
        Vector2 v = rb.linearVelocity;
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // ======================================================
    // СТОЛКНОВЕНИЯ
    // ======================================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer || isStuck || isRecalling) return;
        if (!collision.collider.CompareTag("Ground")) return;

        Vector2 normal = collision.contacts[0].normal;
        Vector2 forward = transform.right;

        float angle = Vector2.Angle(forward, -normal);

        if (angle <= maxStickAngle)
        {
            Stick();
        }
        else
        {
            StartCoroutine(BounceThenRecall(normal));
        }
    }

    // ======================================================
    // ВТЫКАНИЕ
    // ======================================================
    private void Stick()
    {
        isStuck = true;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        gameObject.layer = LayerMask.NameToLayer("SpearPlatform");
        IgnorePlayersCollision(false);
    }

    // ======================================================
    // ОТСКОК + АВТОВОЗВРАТ
    // ======================================================
    private IEnumerator BounceThenRecall(Vector2 normal)
    {
        isRecalling = true;

        Vector2 reflected = Vector2.Reflect(rb.linearVelocity, normal);
        rb.linearVelocity = reflected * bounceMultiplier;

        yield return new WaitForSeconds(recallDelay);
        // Если сервер не сказал явно — просто уничтожаем
        NetworkObject.Despawn();
    }

    // ======================================================
    // РУЧНОЙ ВОЗВРАТ (ВЫЗЫВАЕТСЯ СЕРВЕРОМ)
    // ======================================================
    public void StartRecall(Vector3 targetPosition)
    {
        if (!IsServer || isRecalling) return;
        StartCoroutine(RecallRoutine(targetPosition));
    }

    private IEnumerator RecallRoutine(Vector3 target)
    {
        isRecalling = true;

        rb.bodyType = RigidbodyType2D.Kinematic;
        col.enabled = false;

        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                recallSpeed * Time.deltaTime
            );

            transform.Rotate(0, 0, 720f * Time.deltaTime);
            yield return null;
        }

        NetworkObject.Despawn();
    }

    // ======================================================
    // КОЛЛИЗИИ С ИГРОКАМИ
    // ======================================================
    private void IgnorePlayersCollision(bool ignore)
    {
        foreach (var p in FindObjectsOfType<PlayerMovement>())
        {
            Collider2D pc = p.GetComponent<Collider2D>();
            if (pc != null)
                Physics2D.IgnoreCollision(col, pc, ignore);
        }
    }
}
