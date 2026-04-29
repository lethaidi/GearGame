using UnityEngine;
using System.Collections;


public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public Transform coinTarget;
    public string targetTag;
    public int amountCoin = 10;
    public float explosionForce = 2f;

    public bool Spawn = false, isRandom = false;
    public int minRandom, maxRandom;

    public Coin coin;
    public string coinReward;

    public ScriptableSkills skillCoin;
    private int lastSkillCoin;

    void Start()
    {
        if (skillCoin != null)
        {
            lastSkillCoin = skillCoin.valueSkill;
            maxRandom += lastSkillCoin; // Initialize maxRandom with skillCoin value
        }
        if (!Spawn)
            return;
        GameObject targetObj = GameObject.FindWithTag(targetTag);
        if (targetObj != null)
            coinTarget = targetObj.transform;

        SpawnCoins();
    }
    void Update()
    {
        if (skillCoin != null && skillCoin.valueSkill != lastSkillCoin)
        {
            int diff = skillCoin.valueSkill - lastSkillCoin;
            maxRandom += diff;
            lastSkillCoin = skillCoin.valueSkill;
        }

        randomCoin(); // Randomize coin amount if needed
        if (coinReward != null)
        {
            findCoin();
        }
        if (coin != null)
        {
            amountCoin = coin.CoinValue;
        }
        if (Spawn)
        {
            Spawn = false; // Reset Spawn flag
            SpawnCoins();
        }
    }
    void SpawnCoins()
    {
        if (coinTarget == null) return;

        for (int i = 0; i < amountCoin; i++)
        {
            Vector3 spawnPos = transform.position;
            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

            Vector2 randomDir = Random.insideUnitCircle.normalized * explosionForce;

            CropCoin coinScript = coin.GetComponent<CropCoin>();
            if (coinScript != null)
                coinScript.Init(coinTarget, randomDir, targetTag); // truyền luôn tag
        }
    }
    IEnumerator waitCountWave()
    {
        yield return new WaitForSeconds(3f);
        Spawn = true;
    }

    void findCoin()
    {
        if (!string.IsNullOrEmpty(coinReward))
        {
            GameObject obj = GameObject.Find(coinReward);
            if (obj != null) coin = obj.GetComponent<Coin>();
            amountCoin = coin.CoinValue;
        }
    }
    void randomCoin()
    {
        if (isRandom)
        {
            amountCoin = Random.Range(minRandom, maxRandom + 1);
            isRandom = false;
        }
    }
}
