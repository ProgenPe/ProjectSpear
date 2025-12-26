using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Spear : MonoBehaviour
{
    [Header("Stick")]
    [SerializeField] private float maxStickAngle = 45f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Bounce")]
    [SerializeField] private float bounceMultiplier = 0.5f;

    [Header("Recall")]
    [SerializeField] private float recallDelay = 0.4f;
    [SerializeField] private float recallSpeed = 25f;

    private Rigidbody2D rb;
    private Collider2D col;

    private bool isStuck;
    private bool isRecalling;
    private bool isFlying;

    private Transform recallTarget; // ← всегда задан

    public bool IsFlying => isFlying;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 1f;
    }

    // ЦЕЛЬ ВОЗВРАТА ЗАДАЁТСЯ СРАЗУ
    public void Init(Transform owner)
    {
        recallTarget = owner;
    }

    public void Throw(Vector2 velocity)
    {
        isStuck = false;
        isRecalling = false;
        isFlying = true;

        col.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = velocity;

        RotateAlongVelocity();
        IgnorePlayersCollision(true);
    }

    private void Update()
    {
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isStuck || isRecalling) return;

        Vector2 normal = collision.contacts[0].normal;
        float angle = Vector2.Angle(transform.right, -normal);

        bool isWall = ((1 << collision.gameObject.layer) & wallLayer) != 0;
        isFlying = false;

        if (!isWall || angle > maxStickAngle)
        {
            StartCoroutine(BounceThenRecall(normal));
            return;
        }

        Stick();
    }

    private void Stick()
    {
        isStuck = true;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        gameObject.layer = LayerMask.NameToLayer("SpearPlatform");
        IgnorePlayersCollision(false);
    }

    private IEnumerator BounceThenRecall(Vector2 normal)
    {
        isRecalling = true;

        rb.linearVelocity = Vector2.Reflect(rb.linearVelocity, normal) * bounceMultiplier;
        yield return new WaitForSeconds(recallDelay);

        StartCoroutine(RecallRoutine());
    }

    public void StartRecall()
    {
        if (isRecalling || isFlying) return;
        StartCoroutine(RecallRoutine());
    }

    private IEnumerator RecallRoutine()
    {
        isRecalling = true;
        isFlying = false;

        rb.bodyType = RigidbodyType2D.Kinematic;
        col.enabled = false;

        while (Vector3.Distance(transform.position, recallTarget.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                recallTarget.position,
                recallSpeed * Time.deltaTime
            );

            transform.Rotate(0, 0, 720f * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void IgnorePlayersCollision(bool ignore)
    {
        foreach (var p in FindObjectsOfType<PlayerMovement>())
        {
            var pc = p.GetComponent<Collider2D>();
            if (pc) Physics2D.IgnoreCollision(col, pc, ignore);
        }
    }
}
