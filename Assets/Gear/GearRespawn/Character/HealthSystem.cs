using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Health UI")]
    public Image imageHealthBar; // Thanh máu
    public Image backImageHeal; // Hình nền thanh máu
    public GameObject body;
    public float smoothSpeed = 5f;
    private float displayedFillAmount;
    public GameObject floatingDamagePrefab; // Prefab hiển thị sát thương
    private float totalDamage;
    private float timeCalculate;
    public bool isBuff = false;
    public SpriteRenderer charImage;

    [Header("Visual Effects")]
    public Sprite whiteSprite; // Sprite trắng để flash
    private Sprite originalSprite;
    private SpriteRenderer sr;

    [Header("Die Effect")]
    public GameObject dieEff, prefab;
    public bool isDead = false, isSpawnprefab = false;
    public Transform spawnPoint;
    public Ally ally;

    [Header("ScriptableObject")]
    public CharacterStats baseStats;
    public TaskSO taskKillEnemy;
    public bool isAddTask = false;

    public bool isEnemy = false, isUp = false;
    [Header("ObjectPool")]
    public ObjectPool objectPool;

    void Start()
    {
        ally = GetComponent<Ally>();

        if (isBuff && charImage != null)
        {
            BuffHealth();
        }
        totalDamage = 0f;
        timeCalculate = 0f;

        if (baseStats != null)
        {
            maxHealth = baseStats.maxHealth;
        }
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalSprite = sr.sprite;

        currentHealth = maxHealth;
        displayedFillAmount = 1f;

        if (imageHealthBar != null)
            imageHealthBar.fillAmount = displayedFillAmount;

        if (dieEff != null)
            dieEff.SetActive(false);
    }

    void Update()
    {
        
        //hiển thị tổng sát thương sau 0.5s
        if (totalDamage > 0)
        {
            timeCalculate += Time.deltaTime;

            if (timeCalculate >= 0.2f)
            {
                FloatDamage(totalDamage);
                totalDamage = 0;
                timeCalculate = 0;
            }
            if (imageHealthBar != null)
            {
                float targetFill = (float)currentHealth / maxHealth;
                displayedFillAmount = Mathf.Lerp(displayedFillAmount, targetFill, Time.deltaTime * smoothSpeed);
                imageHealthBar.fillAmount = displayedFillAmount;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        // Hiệu ứng bị hất nhẹ và đang bay (Up) thì không bị hất nữa
        float distanceThrow = isEnemy ? -1f : 1f;
        float randomDis = Random.Range(0f, 0.3f);
        if (isUp == false)
            StartCoroutine(BezierMove(transform.position, new Vector2(transform.position.x - (randomDis * distanceThrow), transform.position.y), 0.5f));

        // Cập nhật máu
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Cộng dồn damage và reset thời gian đếm
        totalDamage += damage;
        timeCalculate = 0f;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (sr != null && whiteSprite != null)
            StartCoroutine(FlashWhite());
    }


    public void Die()
    {
        if (taskKillEnemy != null && taskKillEnemy.currentValue < taskKillEnemy.targetValue && !isAddTask)
        {
            isAddTask = true;
            taskKillEnemy.currentValue++;
        }
        isDead = true;

        if (sr != null)
            sr.sprite = null;

        imageHealthBar.gameObject.SetActive(false);
        backImageHeal.gameObject.SetActive(false);
        body.SetActive(false);

        StartCoroutine(DieEffect());
    }

    IEnumerator DieEffect()
    {
        yield return new WaitForEndOfFrame();

        if (ally != null && prefab != null && isSpawnprefab && !ally.isWin)
        {
            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        }

        if (dieEff != null)
            dieEff.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        if (objectPool != null)
        {
            this.gameObject.tag = "Ally";

            objectPool.ReturnObject(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }


    IEnumerator FlashWhite()
    {
        if (sr == null || whiteSprite == null) yield break;

        body.SetActive(false); // Tắt body khi flash
        sr.sprite = whiteSprite;

        yield return new WaitForSeconds(0.2f);

        body.SetActive(true); // Bật lại body sau khi flash
        sr.sprite = originalSprite;
    }

    IEnumerator BezierMove(Vector2 start, Vector2 end, float duration)
    {
        Vector2 control = new Vector2((start.x + end.x) / 2, Mathf.Max(start.y, end.y) + 1f);
        float elapsed = 0f;
        isUp = true;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float u = 1 - t;
            Vector2 position = u * u * start + 2 * u * t * control + t * t * end;
            transform.position = position;

            elapsed += Time.deltaTime;
            yield return null;
        }
        isUp = false;
        transform.position = end; // đảm bảo kết thúc đúng vị trí
    }

    void FloatDamage(float damage)
    {
        if (floatingDamagePrefab != null)
        {
            GameObject floatingDamage = Instantiate(floatingDamagePrefab, transform.position, Quaternion.identity);
            FloatingDamage dmg = floatingDamage.GetComponent<FloatingDamage>();
            if (dmg != null)
            {
                dmg.SetDamage(Mathf.RoundToInt(damage));
            }
        }
    }

    public void ResetState()
    {
        isDead = false;
        currentHealth = maxHealth;
        // Reset thanh máu
        if (imageHealthBar != null)
        {
            imageHealthBar.gameObject.SetActive(true);
            imageHealthBar.fillAmount = 1f;
        }
        body.SetActive(true);
        dieEff.SetActive(false);
    }

    void BuffHealth()
    {
        if (isBuff)
        {
            maxHealth = Mathf.RoundToInt(maxHealth * 3f);
            currentHealth = maxHealth;
            //scale obj bự lên gấp 2 lần rồi thu nhỏ từ từ về kích thước ban đầu
            StartCoroutine(WaitBigger());

            isBuff = false;
        }
    }
    IEnumerator WaitBigger()
    {
        // Phóng to ngay lập tức
        transform.localScale = new Vector3(-0.30f, 0.30f, 0.30f);
        charImage.color = Color.red;
        // Chờ 5 giây
        yield return new WaitForSeconds(15f);

        // Thu nhỏ từ từ trong 1 giây
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(-0.19f, 0.19f, 0.19f);
        charImage.color = Color.white;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null; // chờ frame tiếp theo
        }

        // Đảm bảo scale chính xác khi kết thúc
        transform.localScale = endScale;
    }
}

