using System.Collections;
using System.Collections.Generic;
using UnityEngine; 


public class DefeatManager : MonoBehaviour
{
    public GameObject closeWave, background, startScene, head, bottom, fixedWave;
    public GameObject spawnedCloseWave;
    public WaveManager waveManager;
    public CountWave countWave;
    public bool hasClicked = false, isNextBackGr = false; // Cờ để kiểm tra click

    public CoinSpawner Dia;
    public CoinSpawner Green;
    public ExperienceBar ExpBar;

    public AudioSource audioSource;

    void Update()
    {
        if(countWave == null)
        {
            countWave = FindObjectOfType<CountWave>();
        }
        if (fixedWave == null)
        {
            fixedWave = waveManager.SpawnScreen;
        }
        if(ExpBar == null)
        {
            ExpBar = FindObjectOfType<ExperienceBar>();
        }
        if (countWave.isWinR10)
        {
            isNextBackGr = true;
        }
    }

    public void Replay()
    {
        if (hasClicked) return; // Nếu đã click trước đó thì bỏ qua

        hasClicked = true; // Đánh dấu đã click
        AudioListener.pause = false;
        Time.timeScale = 1f;
        StartCoroutine(ChanceBackGround());
    }

    IEnumerator ChanceBackGround()
    {
        SpawnCloseWave();
        startScene.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        head.SetActive(true);

        bottom.SetActive(true);
        ButtonChance buttonChance = bottom.GetComponent<ButtonChance>();
        buttonChance.buttonAnimators[0].SetBool("isSelect", true);

        if (ExpBar != null)
        {
            //Random exp between 5 and 30
            int randomExp = Random.Range(5, 30);
            ExpBar.AddExp(randomExp);
        }
        // play sound
        audioSource.Play();

        Destroy(fixedWave);
        deleteAll();

        Vector3 startPos = background.transform.position;
        Vector3 endPos = new Vector3(5f, startPos.y, startPos.z);

        yield return StartCoroutine(MoveBackgroundCoroutine(startPos, endPos, 1f));
    }

    IEnumerator MoveBackgroundCoroutine(Vector3 from, Vector3 to, float duration)
    {
        if(isNextBackGr && countWave.isChangeBG)
        {
            FindObjectOfType<LevelManager>().NextLevel();
        }
        float elapsed = 0f;
        while (elapsed < duration)
        {
            background.transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        background.transform.position = to;

        yield return new WaitForSecondsRealtime(1f);
        Destroy(spawnedCloseWave);
        hasClicked = false;
        if(isNextBackGr)
        {
            Dia.Spawn = true;
            Green.Spawn = true;
            isNextBackGr = false;
            countWave.isWinR10 = false;
            
        }

    }

    void deleteAll()
    {
        List<GameObject> allObjects = new List<GameObject>();

        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Main"));
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Attacker"));
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Gear+1"));
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Gear+2"));
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Gear+4"));
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Gear+8"));
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Ally"));
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Weapons"));
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("CropCoin"));

        foreach (GameObject obj in allObjects)
        {
            Destroy(obj);
        }
    }

    void SpawnCloseWave()
    {
        spawnedCloseWave = Instantiate(closeWave, closeWave.transform.position, closeWave.transform.rotation);
    }
}
