using UnityEngine;

public class CropCoin : MonoBehaviour
{
    public Coin coin;
    private Transform targetPoint;
    private bool movingToTarget = false;
    private float moveSpeed = 10f;
    private Vector3 velocity;
    private string targetTag;

    public void Init(Transform target, Vector2 randomForce, string tagFilter)
    {
        targetPoint = target;
        velocity = randomForce;
        targetTag = tagFilter; // lưu tag cần tìm Coin
        Invoke(nameof(StartMoving), 0.3f);
    }

    void StartMoving()
    {
        movingToTarget = true;
    }

    void Start()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject obj in objs)
        {
            Coin c = obj.GetComponent<Coin>();
            if (c != null)
            {
                coin = c;
                break;
            }
        }
    }

    void Update()
    {
        if (!movingToTarget)
        {
            transform.position += velocity * Time.deltaTime;
        }
        else if (targetPoint != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                if (coin != null)
                    coin.CoinValue += 1;

                Destroy(gameObject);
            }
        }
    }
}
