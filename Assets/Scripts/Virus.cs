using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Virus : MonoBehaviour, IPointerDownHandler   // <-- HOZZÁADVA
{
    public float baseSpeed = 200f;
    public int scoreValue = 25;
    public float damageToServer = 20f;
    public GameObject explosionPrefab;

    private float speed;
    private Vector2 targetPos;
    private MalwareDefenseManager manager;
    private Rigidbody2D rb;
    private bool isDead = false;

    public void Initialize(Vector3 serverWorldPos, float speedMultiplier, MalwareDefenseManager mgr)
    {
        manager = mgr;
        targetPos = serverWorldPos;
        speed = baseSpeed * Mathf.Max(0.01f, speedMultiplier);
        rb = GetComponent<Rigidbody2D>();
        gameObject.layer = LayerMask.NameToLayer("Virus");
    }

    void Start()
    {
        rb = rb ?? GetComponent<Rigidbody2D>();
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = false;
    }

    void FixedUpdate()
    {
        if (isDead) return;

        Vector2 pos = rb.position;
        Vector2 dir = (targetPos - pos).normalized;
        Vector2 newPos = pos + dir * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        if (Vector2.Distance(newPos, targetPos) < 0.35f)
        {
            isDead = true;
            manager.OnVirusReachedServer(gameObject, damageToServer);
        }
    }

    // --- EZ REAGÁL TOUCH + KATTINTÁSRA EGYARÁNT ---
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDead) Die();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Server"))
        {
            manager.OnVirusReachedServer(gameObject, damageToServer);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (explosionPrefab)
        {
            GameObject exp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(exp, 2f);
        }

        manager.OnVirusDestroyed(gameObject, scoreValue);
    }

    void OnDestroy()
    {
        manager.RemoveVirusFromList(gameObject);
    }
}