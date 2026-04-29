using UnityEngine;

public class GearMain : MonoBehaviour
{
    public float mainValue;
    public Vector3 rotationSpeed = new Vector3(0, 0, 150);
    public int CountNear = 0;
    public bool isFight = false, isSn1 = true;
    public ButtonStore buttonStore;
    // Dùng để ẩn button khi snap
    public GameObject snapTarget;
    public GameObject lastSnapZone;

    public AudioSource audioSource;

    void Update()
    {
        buttonStore = FindObjectOfType<ButtonStore>();
        if (buttonStore.isPushFight)
        {
            isFight = true;
        }
        else {
            isFight = false;
        }

        if (isSn1)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(-rotationSpeed * Time.deltaTime);
        }
        CheckNearGear();
        CheckedWhichButton();
    }

    void CheckNearGear()
    {
        ControllGear[] gears = FindObjectsOfType<ControllGear>();
        int newCount = 0;

        foreach (ControllGear gear in gears)
        {
            if (!gear.isSnap) continue;

            float dist = Vector2.Distance(transform.position, gear.transform.position);
            if (dist < 1f)
            {
                newCount++;
            }
        }

        CountNear = newCount;
    }

    void CheckedWhichButton()
    {
        if (snapTarget != null && (snapTarget.CompareTag("SnapZone1") || snapTarget.CompareTag("SnapZone2")))
        {
            lastSnapZone = snapTarget.gameObject;
            lastSnapZone.SetActive(false);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("SnapZone1") || other.CompareTag("SnapZone2"))
        {
            snapTarget = other.gameObject;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Other"))
        {
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
            rotationSpeed = new Vector3(0, 0, 50);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Other"))
        {
            rotationSpeed = new Vector3(0, 0, 150);
        }
    }
}
