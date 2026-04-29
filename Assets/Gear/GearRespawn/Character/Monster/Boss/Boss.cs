using UnityEngine;

public class Boss : MonoBehaviour
{
    public float jumpForceUp = 5f;
    public float jumpForceLeft = 3f;
    public float moveSpeedLeft = 2f;

    private Rigidbody2D rb;
    private float startY;
    public bool hasLanded = false;

    public ButtonStore buttonStore;

    public bool canMove = false;
    [Header("Attack Check")]
    public GameObject targetObj;
    public bool isAhead = false;  // có ở phía trước không
    public float attackRange, stopRange; // khoảng cách tấn công
    public bool isAttack = false, isStop = false, isShot = false;  // đang tấn công không
    public int DamageMin , DamageMax; // sát thương tấn công

    private float lastAttackTime = 0f;
    public float attackCooldown = 1f; // giây

    public AudioSource audioSource;
    public HealthSystem health;
    public Animator animator;

    void Start()
    {
        buttonStore = FindObjectOfType<ButtonStore>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startY = transform.position.y;
        rb.velocity = Vector2.zero;

        // Nhảy xiên lên trái
        Vector2 force = new Vector2(-jumpForceLeft, jumpForceUp);
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (!hasLanded && transform.position.y <= startY)
        {
            Land();
        }

        CheckAttack();
        Attack(); 
    }

    void FixedUpdate()
    {
        if (hasLanded)
        {
            if (buttonStore != null && buttonStore.isPushFight && canMove)
            {
                animator.SetBool("isWalk", true); 
                rb.velocity = new Vector2(-moveSpeedLeft, 0f);
            }
            else
            {
                rb.velocity = Vector2.zero; // Dừng hẳn trong FixedUpdate
            }
        }
    }

    void Land()
    {
        hasLanded = true;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        CameraShake camShake = Camera.main.GetComponent<CameraShake>();
        if (camShake != null)
        {
            StartCoroutine(camShake.Shake(0.5f, 0.3f)); // rung 0.5s, biên độ 0.3
        }
    }

    void CheckAttack()
    {
        string[] attackTags = { "Player" };
        GameObject nearestTarget = null;
        float nearestDist = Mathf.Infinity;

        foreach (string tag in attackTags)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objs)
            {
                float dist = Vector2.Distance(transform.position, obj.transform.position);

                // Kiểm tra trong tầm attackRange
                if (dist <= attackRange)
                {
                    Vector2 dirToTarget = (obj.transform.position - transform.position).normalized;
                    bool isInFront = Vector2.Dot(dirToTarget, Vector2.left) > 0;

                    if (isInFront && dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearestTarget = obj;
                    }
                }

                // Nếu trong tầm dừng đánh
                if (dist <= stopRange)
                {
                    // Giới hạn tốc độ đánh
                    if (Time.time >= lastAttackTime + attackCooldown)
                    {
                        HealthSystem health = obj.GetComponent<HealthSystem>();
                        animator.SetBool("isAttack", true);
                        isShot = true; 
                        if (health != null)
                        {
                            audioSource.Play(); // Phát âm thanh tấn công
                            int damage = Random.Range(DamageMin, DamageMax + 1);
                            health.TakeDamage(damage);
                        }
                        lastAttackTime = Time.time;
                    }
                    isStop = true;
                }
                else
                {
                    isStop = false; 
                }
            }
        }

        // Gán target
        if (nearestTarget != null)
        {
            targetObj = nearestTarget;
            isAhead = true;
        }
        else
        {
            targetObj = null;
            isAhead = false;
        }
    }

    void Attack()
    {
        if (isAhead && targetObj != null)
        {
            if (isStop)
            {
                canMove = false;
            }
        else canMove = true;

            isAttack = true;
        }
        else
        {
            animator.SetBool("isAttack", false);
            canMove = true; // Nếu không tấn công thì có thể di chuyển
            isAttack = false; 
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DelZone"))
        {
            health.TakeDamage(health.maxHealth);

        }
    }
}
