using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public bool isPassed = true;

    [Header("References")]
    public GameObject screenWave;
    public GameObject normalWave;
    public GameObject startScene;
    public GameObject head;
    public GameObject bottom;
    public GameObject background;

    public GameObject SpawnScreen;
    public AudioSource audioSource;

    public int isFirstPlay;   // biến hiện tại
    public GameObject guide;

    void Start()
    {
        // ⭐ Load giá trị đã lưu — nếu chưa có thì mặc định = 0 (LẦN ĐẦU)
        isFirstPlay = PlayerPrefs.GetInt("IsFirstPlay", 0);

        // Chờ 4.5s rồi xử lý
        StartCoroutine(WaitPlaySound());
    }

    void Update()
    {
        // Nếu được set sang 10 hoặc 13 thì lưu lại
        if (isFirstPlay == 10 || isFirstPlay == 13)
        {
            PlayerPrefs.SetInt("IsFirstPlay", isFirstPlay);
            PlayerPrefs.Save();
        }
    }

    IEnumerator WaitPlaySound()
    {
        yield return new WaitForSeconds(4.5f);
        audioSource.Play();

        // ⭐ Nếu lần đầu chơi
        if (isFirstPlay == 0)
        {
            PlayerPrefs.SetInt("IsFirstPlay", isFirstPlay);
            PlayerPrefs.Save();
            isFirstPlay++;
            StartWave();
        }

    }

    public void StartWave()
    {
        isPassed = true;
        SpawnscreenWave();

        GuideOutSide Guide = guide.GetComponent<GuideOutSide>();
        Guide.DisableAllGuide();

        startScene.SetActive(false);
        StartCoroutine(waitDisappear());
        StartCoroutine(waitChangeScene());
    }

    IEnumerator waitDisappear()
    {
        yield return new WaitForSeconds(0.1f);
        head.SetActive(false);
        bottom.SetActive(false);
    }

    IEnumerator waitChangeScene()
    {
        Vector3 startPos = background.transform.position;
        Vector3 endPos = new Vector3(startPos.x - 8f, startPos.y, startPos.z);

        yield return StartCoroutine(MoveBackground(startPos, endPos, 1f));
    }

    IEnumerator MoveBackground(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            background.transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        background.transform.position = to;
    }

    void SpawnscreenWave()
    {
        if (isFirstPlay < 10)
            SpawnScreen = Instantiate(screenWave, screenWave.transform.position, screenWave.transform.rotation);
        else
            SpawnScreen = Instantiate(normalWave, normalWave.transform.position, normalWave.transform.rotation);
    }
}
