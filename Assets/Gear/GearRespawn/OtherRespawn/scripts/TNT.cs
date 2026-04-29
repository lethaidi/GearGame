using System.Collections;
using UnityEngine;

public class TNT : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    public float moveSpeed = 2f;
    public Vector2 moveDirection = new Vector2(-1, 0); // mặc định

    [Header("Giới hạn Platform")]
    private Vector2 minBounds, maxBounds;
    public float RotationSpeed;

    [Header("Thành phần")]
    private Rigidbody2D rb;
    public CapsuleCollider2D capsuleCollider;

    [Header("Di chuyển đặc biệt trên Bridge")]
    public Vector2 moveDirectionLeft = new Vector2(-1, 0);
    public float moveSpeedLeft = 2f;
    public bool onBridge = false;

    public bool canMove = false;
    public bool activated = false;
    private float randomYDirection = 0f;

    public AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        if (capsuleCollider != null) capsuleCollider.enabled = true;
        randomYDirection = Random.Range(-0.1f, 0.1f);
    }

    void Update()
    {
        if (onBridge)
        {
            transform.Translate(moveDirectionLeft.normalized * moveSpeedLeft * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (!canMove || !activated) return;

        Vector2 directionToMove = moveDirection + new Vector2(0f, randomYDirection);
        directionToMove.Normalize();

        Vector2 newPos = rb.position + directionToMove * moveSpeed * Time.fixedDeltaTime;

        newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
        newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);

        rb.MovePosition(newPos);

        // 👉 Thêm xoay tại đây
        transform.Rotate(0f, 0f, RotationSpeed * Time.fixedDeltaTime);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Platform"))
        {
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;

            Bounds bounds = other.bounds;
            minBounds = bounds.min;
            maxBounds = bounds.max;

            capsuleCollider.enabled = false;

            onBridge = false;
            canMove = true;
            activated = true;
        }

        if (other.CompareTag("DelZone"))
        {
            Destroy(this.gameObject);
        }

        if (other.CompareTag("Bridge"))
        {
            onBridge = true;
            capsuleCollider.enabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Bridge"))
        {
            onBridge = false;
        }
    }
}
