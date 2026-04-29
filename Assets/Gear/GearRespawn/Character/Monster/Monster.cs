using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    public float moveSpeed = 2f;
    public Vector2 moveDirection = new Vector2(-1, 0); // mặc định

    [Header("Tìm mục tiêu")]
    public string targetTag = "Player"; // mục tiêu sẽ được tìm theo tag
    public GameObject targetObj;
    private Vector2 targetDirection;

    [Header("Giới hạn Platform")]
    private Vector2 minBounds, maxBounds;

    [Header("Thành phần")]
    private Rigidbody2D rb;
    public Animator animator;
    public GameObject shadow;
    public CapsuleCollider2D capsuleCollider;
    private SpriteRenderer spriteRenderer;
    public HealthSystem health; // Tham chiếu đến HealthSystem để kiểm tra sức khỏe

    [Header("Di chuyển đặc biệt trên Bridge")]
    public Vector2 moveDirectionLeft = new Vector2(-1, 0);
    public float moveSpeedLeft = 2f;
    public bool onBridge = false, sortLayer = false;

    public bool canMove = false;
    public bool activated = false; // ✅ chỉ khi chạm Platform mới hoạt động EnemyMovement
    private float randomYDirection = 0f;

    [Header("Tấn công")]
    public bool isAhead = false;
    public bool isAttack = false;
    [Header("Khoảng cách tấn công")]
    public float distance;
    public float attackRange;

    void Start()
    {
        capsuleCollider.enabled = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        shadow.SetActive(false);

        // Gán độ lệch ngẫu nhiên theo Y chỉ 1 lần
        randomYDirection = Random.Range(-0.1f, 0.1f);
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
        else if (!sortLayer)
        {
            spriteRenderer.sortingOrder = 7; // Đặt lại về giá trị mặc định nếu không sắp xếp
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

        if (targetObj != null)
        {
            //kiểm tra nếu target active false thì bỏ target
            if (!targetObj.activeInHierarchy)
            {
                targetObj = null;
                directionToMove = moveDirection + new Vector2(0f, randomYDirection);
                directionToMove.Normalize();
            }
            else
                directionToMove = targetDirection; // hướng tới target
        }
        else
        {
            // chưa thấy target thì lệch trục Y
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

        targetObj = closest;

        if (targetObj != null)
        {
            targetDirection = (targetObj.transform.position - transform.position).normalized;
        }
        else
        {
            targetDirection = Vector2.zero;
        }
    }

    void CheckAttack()
    {
        if (targetObj == null)
        {
            isAttack = false;
            animator.SetBool("isAttack", false);
            canMove = true;
            return;
        }

        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 targetPos = new Vector2(targetObj.transform.position.x, targetObj.transform.position.y);
        distance = Vector2.Distance(myPos, targetPos);

        Vector2 dirToTarget = targetPos - myPos;
        isAhead = Vector2.Dot(dirToTarget.normalized, moveDirection.normalized) > 0;

        // 👇 Nếu đã vượt qua (target không còn phía trước) → bỏ mục tiêu
        if (!isAhead)
        {
            targetObj = null;
            animator.SetBool("isAttack", false);
            canMove = true;
            isAttack = false;
            return;
        }

        // ✅ Nếu target vẫn ở trước mặt và trong tầm đánh
        if (distance <= attackRange)
        {
            animator.SetBool("isAttack", true);
            rb.velocity = Vector2.zero;
            canMove = false;
            isAttack = true;
        }
        else
        {
            animator.SetBool("isAttack", false);
            canMove = true;
            isAttack = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Platform"))
        {
            animator.SetBool("isStart", true);
            shadow.SetActive(true);
            shadow.transform.rotation = Quaternion.identity;

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
            health.TakeDamage(health.maxHealth);

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
