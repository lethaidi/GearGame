using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    public float moveSpeed = 2f;
    public Vector2 moveDirection = new Vector2(-1, 0);
    private float baseSpeed; // ✅ tốc độ gốc

    [Header("Tìm mục tiêu")]
    public string targetTag = "Player";
    public GameObject target;
    private Vector2 targetDirection;

    [Header("Giới hạn Platform")]
    private Vector2 minBounds, maxBounds;

    [Header("Thành phần")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public GameObject shadow;
    public CapsuleCollider2D capsuleCollider;

    [Header("Di chuyển đặc biệt trên Bridge")]
    public Vector2 moveDirectionLeft = new Vector2(-1, 0);
    public float moveSpeedLeft = 2f;
    public bool onBridge = false, sortLayer = false;

    [Header("Trạng thái tổng quát")]
    public bool canMove = false;
    public bool isStop = false;
    public bool activated = false;

    private float randomYDirection = 0f;

    [Header("Tấn công")]
    public bool isAhead = false;
    public bool isAttack = false;
    public bool isWin = false;

    [Header("Khoảng cách tấn công")]
    public float distance;
    public float attackRange = 1.5f, baseAttackRange;

    [Header("Biến mất khi thắng")]
    public HealthSystem healthSystem;

    [Header("Special Obj")]
    public bool isMino = false;
    private bool hasBoosted = false; // ✅ đánh dấu đã boost hay chưa

    void Start()
    {
        baseAttackRange = attackRange;
        capsuleCollider.enabled = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (shadow != null)
            shadow.SetActive(false);

        randomYDirection = Random.Range(-0.1f, 0.1f);
        baseSpeed = moveSpeed; // ✅ lưu tốc độ gốc
    }

    void Update()
    {
        if (onBridge)
        {
            transform.Translate(moveDirectionLeft.normalized * moveSpeedLeft * Time.deltaTime);
        }

        if (spriteRenderer != null && sortLayer)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }

        if (activated)
        {
            FindTarget();
            CheckAttack();
        }
    }

    void FixedUpdate()
    {
        if (!canMove || !activated) return;

        Vector2 directionToMove;

        if (target != null)
        {
            directionToMove = targetDirection;
        }
        else
        {
            directionToMove = moveDirection + new Vector2(0f, randomYDirection);
            directionToMove.Normalize();
        }

        Vector2 newPos = rb.position + directionToMove * moveSpeed * Time.fixedDeltaTime;

        newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
        newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);

        rb.MovePosition(newPos);
    }

    void FindTarget()
    {
        GameObject[] candidates = GameObject.FindGameObjectsWithTag(targetTag);

        float minDist = Mathf.Infinity;
        GameObject closest = null;

        foreach (GameObject obj in candidates)
        {
            if (obj == this.gameObject) continue;
            float dist = Vector2.Distance(transform.position, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj;
            }
        }

        target = closest;

        if (target != null)
        {
            targetDirection = (target.transform.position - transform.position).normalized;

            // ✅ Nếu là Mino và lần đầu phát hiện mục tiêu → boost tốc độ và lao tới tấn công
            if (isMino && !hasBoosted)
            {
                hasBoosted = true;
                moveSpeed = baseSpeed * 2f;
                animator.SetBool("isRun", true);

                // Kiểm tra khoảng cách ngay khi boost
                distance = Vector2.Distance(transform.position, target.transform.position);
                if (distance <= attackRange)
                {
                    // 💥 Hiệu ứng lao tới tấn công 1 lần
                    Bezier(
                        (Vector2)transform.position,
                        (Vector2)transform.position + new Vector2(-1f, 1f),
                        (Vector2)transform.position + new Vector2(-2f, 0f),
                        0.5f,
                        out Vector2 bezierPos
                    );

                    isAttack = true;
                    animator.SetBool("isAttack", true);
                    rb.velocity = Vector2.zero;
                    canMove = false;

                    // Sau khi đánh xong, quay về bình thường
                    StartCoroutine(ResetAfterFirstAttack());
                }
                else
                {
                    // Nếu chưa tới gần ngay → chỉ boost tốc độ trong vài giây
                    StartCoroutine(ResetSpeedAfterDelay(2f));
                }
            }
        }
        else
        {
            targetDirection = Vector2.zero;
            animator.SetBool("isRun", false);
            moveSpeed = baseSpeed;
            // ❌ Không reset hasBoosted để tránh boost lại lần nữa
        }
    }

    IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        moveSpeed = baseSpeed;
        animator.SetBool("isRun", false);
    }

    IEnumerator ResetAfterFirstAttack()
    {
        // Giả lập thời gian thực hiện cú đánh
        yield return new WaitForSeconds(1.2f);
        isAttack = false;
        animator.SetBool("isAttack", false);
        canMove = true;
        moveSpeed = baseSpeed;
        animator.SetBool("isRun", false);
    }

    void Bezier(Vector2 p0, Vector2 p1, Vector2 p2, float t, out Vector2 result)
    {
        float u = 1 - t;
        result = u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    void CheckAttack()
    {
        if (isWin)
        {
            canMove = false;
            animator.SetBool("isWin", true);
            StartCoroutine(WaitDisappear());
            return;
        }

        if (target == null)
        {
            isAttack = false;
            animator.SetBool("isAttack", false);
            canMove = true;
            return;
        }

        if (isStop)
        {
            animator.enabled = false;
            return;
        }

        Vector2 myPos = transform.position;
        Vector2 targetPos = target.transform.position;
        distance = Vector2.Distance(myPos, targetPos);

        Vector2 dirToTarget = (targetPos - myPos).normalized;

        isAhead = Vector2.Dot(dirToTarget, moveDirection.normalized) > 0;

        if (!isAhead)
        {
            target = null;
            animator.SetBool("isAttack", false);
            canMove = true;
            isAttack = false;
            return;
        }

        if (distance <= attackRange)
        {
            if (!isAttack)
            {
                isAttack = true;
                animator.SetBool("isAttack", true);
                rb.velocity = Vector2.zero;
                canMove = false;
            }
        }
        else
        {
            isAttack = false;
            animator.SetBool("isAttack", false);
            canMove = true;
        }
    }

    IEnumerator WaitDisappear()
    {
        yield return new WaitForSeconds(8f);
        if (healthSystem != null)
            healthSystem.Die();
        else
            Destroy(gameObject);
    }

    public void ResetState()
    {
        target = null;
        onBridge = false;
        sortLayer = false;
        isAttack = false;
        isWin = false;
        canMove = false;
        activated = false;
        isStop = false;
        isAhead = false;

        distance = 0f;
        rb.gravityScale = 1f;
        capsuleCollider.enabled = true;

        moveSpeed = baseSpeed;
        hasBoosted = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Platform"))
        {
            gameObject.tag = "Player";
            animator.SetBool("isStart", true);

            if (shadow != null)
            {
                shadow.SetActive(true);
                shadow.transform.rotation = Quaternion.identity;
            }

            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;

            Bounds bounds = other.bounds;
            minBounds = bounds.min;
            maxBounds = bounds.max;

            sortLayer = true;
            capsuleCollider.enabled = false;

            canMove = true;
            activated = true;
        }

        if (other.CompareTag("DelZone"))
        {
            Destroy(gameObject);
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
