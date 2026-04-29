using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllGear : MonoBehaviour
{
    // Drag
    private Vector3 offset;
    public bool isDragging = false;
    public bool isSnap = false, isStart = false, dragStore = false;
    public Transform snapTarget = null, tempSnap;
    public GameObject lastSnapZone = null;
    public GameObject telePoint;
    public bool isReachSnap = false;

    // Rotation
    public GameObject Gear;
    public bool but1 = false;
    public bool but2 = false;
    public bool isReach = false;
    public bool isRotate = false, isReachHead = false;
    public float rotationSpeed = 90f;
    public Quaternion targetRotation;
    private bool hasSetTarget = false;

    // Calculate
    public float total;
    public float PlusValue, baseValue;
    public float connectDistance = 1f;
    public float subtotalSelf, penalty;

    // Checked Near Main
    public bool NearMain = false;
    private bool previousReachHead = false;
    public int Count = 0;
    public GearMain gearMain;
    public GameObject timeRespawn;

    //Price
    public Price price;
    public Coin coin;
    public bool isBuy = false;

    //Sold
    public int priceSold;
    public bool isReachBin = false;

    //Pics Setting
    public GameObject number, gear;

    //Sound
    public AudioSource audioSource;
    public AudioClip clip1;
    public AudioClip clip2;

    //ScriptableObject 
    public TaskSO task;

    // ✅ Cache global tất cả bánh răng & GearMain để tối ưu hiệu năng
    private static List<ControllGear> allGears = new List<ControllGear>();
    private static GameObject mainObj;

    void OnEnable()
    {
        if (!allGears.Contains(this))
            allGears.Add(this);
    }

    void OnDisable()
    {
        allGears.Remove(this);
    }

    void Start()
    {
        if (coin == null)
            coin = FindObjectOfType<Coin>();

        if (gearMain == null)
            gearMain = FindObjectOfType<GearMain>();

        if (mainObj == null)
            mainObj = GameObject.FindWithTag("Main");

        if (timeRespawn != null)
            timeRespawn.SetActive(false);
    }

    void Update()
    {
        // Chỉnh SortingOrder khi kéo
        if (isDragging)
        {
            gear.GetComponent<SpriteRenderer>().sortingOrder = 7;
            number.GetComponent<SpriteRenderer>().sortingOrder = 8;
        }
        else
        {
            gear.GetComponent<SpriteRenderer>().sortingOrder = 5;
            number.GetComponent<SpriteRenderer>().sortingOrder = 6;
        }

        // Cập nhật tag
        if (isBuy)
            gameObject.tag = isDragging ? "GearBought" : "Other";

        if (timeRespawn != null)
            timeRespawn.SetActive(isSnap);

        // Logic chính
        CheckNearGear();
        CheckedNearMain();
        HandleDragging();
        HandleUnsnapCheck();

        if ((isReachHead && !previousReachHead) || isStart || !isReachHead)
        {
            _ = CalculateRecursive(new HashSet<ControllGear>(), this);
        }

        previousReachHead = isReachHead;
        CheckedRotate();
        Rotate();

        // Scale hiệu ứng kéo
        if (tempSnap == null)
        {
            transform.localScale = Vector3.one * (isDragging ? 0.9f : 1.1f);
        }
        else
        {
            transform.localScale = Vector3.one * 0.9f;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#endif
    }

    // =============================================
    // 🔹 INPUT CONTROL
    // =============================================
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = GetMouseWorldPos();
            if (IsPointerOverThis(mousePos)) OnBeginDrag(mousePos);
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            OnEndDrag();
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
        touchPos.z = 0;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (IsPointerOverThis(touchPos)) OnBeginDrag(touchPos);
                break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (isDragging) transform.position = touchPos + offset;
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (isDragging) OnEndDrag();
                break;
        }
    }

    bool IsPointerOverThis(Vector3 worldPos)
    {
        var col = GetComponent<Collider2D>();
        return col != null && col.OverlapPoint(worldPos);
    }

    // =============================================
    // 🔹 DRAGGING
    // =============================================
    void OnBeginDrag(Vector3 inputPos)
    {
        if (gearMain.isFight) return;

        offset = transform.position - inputPos;
        isDragging = true;
        isSnap = false;
        isRotate = false;

        if (lastSnapZone != null)
        {
            lastSnapZone.SetActive(true);
            lastSnapZone = null;
        }

        audioSource?.PlayOneShot(clip1);
    }

    void OnEndDrag()
    {
        isDragging = false;

        // Nếu thả vào thùng rác
        if (isReachBin && isBuy)
        {
            coin.CoinValue += priceSold;
            Destroy(gameObject);
            return;
        }

        // Nếu snap hợp lệ
        if (snapTarget != null && price.isPrice)
        {
            price.gameObject.SetActive(false);

            if (!isBuy)
            {
                coin.CoinValue -= price.price;
                if (task != null && task.currentValue < task.targetValue)
                    task.currentValue++;
                isBuy = true;
            }

            string tag = snapTarget.tag;
            dragStore = true;
            isStart = true;
            isSnap = true;
            transform.position = snapTarget.position;

            lastSnapZone = snapTarget.gameObject;
            tempSnap = snapTarget;
            SnapButon snap= lastSnapZone.GetComponent<SnapButon>();
            if (tempSnap != null)
            {
                snap.isSnapped = true;
            }

            StartCoroutine(WaitDisAct(0.1f, lastSnapZone));
            but1 = (tag == "SnapZone1");
            but2 = (tag == "SnapZone2");

            snapTarget = null;
        }
        else if (!dragStore)
        {
            if (audioSource != null && isReachSnap)
            {
                price.isAnim = true;
                audioSource.PlayOneShot(clip2);
            }

            transform.position = telePoint.transform.position;
            isSnap = false;
            but1 = true;
            but2 = false;
        }

        var plusGear = GetComponentInParent<PlusGear>();
        plusGear?.TryMergeIfPossible();
    }

    IEnumerator WaitDisAct(float timeWait, GameObject Obj)
    {
        yield return new WaitForSeconds(timeWait);
        Obj.SetActive(false);
    }

    void HandleDragging()
    {
        if (isDragging && !isSnap)
            transform.position = GetMouseWorldPos() + offset;
    }

    void HandleUnsnapCheck()
    {
        if (!isSnap || !isDragging || lastSnapZone == null) return;

        if (Vector3.Distance(transform.position, lastSnapZone.transform.position) > 0.5f)
        {
            isSnap = false;
            SnapButon snap = lastSnapZone.GetComponent<SnapButon>();
            if (snap != null)
            {
                snap.isSnapped = false;
            }
            lastSnapZone.SetActive(true);
            lastSnapZone = null;
            snapTarget = null;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    // =============================================
    // 🔹 CALCULATIONS
    // =============================================
    public float CalculateRecursive(HashSet<ControllGear> visited, ControllGear origin)
    {
        if (!isSnap || visited.Contains(this)) return 0;
        visited.Add(this);
        subtotalSelf = PlusValue;

        if (this == origin && isReachHead)
        {
            foreach (var gear in allGears)
                gear.isReachHead = false;

            SetReachHeadRecursive(new HashSet<ControllGear>());
        }

        foreach (var gear in allGears)
        {
            if (gear == this || visited.Contains(gear) || !gear.isSnap) continue;

            float dist = Vector3.Distance(transform.position, gear.transform.position);
            if (dist <= connectDistance)
            {
                subtotalSelf += gear.CalculateRecursive(visited, origin);
            }
        }

        float finalSubtotal = subtotalSelf;
        bool hasNearMain = false;

        foreach (var gear in visited)
        {
            if (gear.NearMain)
            {
                hasNearMain = true;
                break;
            }
        }

        if (this == origin)
        {
            penalty = Mathf.Max(0, gearMain.CountNear - Count) * 0.1f;
            total = hasNearMain ? Mathf.Max(0, finalSubtotal - penalty + baseValue) : 0f;
        }

        return finalSubtotal;
    }

    private void SetReachHeadRecursive(HashSet<ControllGear> visited)
    {
        if (visited.Contains(this)) return;
        visited.Add(this);
        isReachHead = true;

        foreach (var gear in allGears)
        {
            if (gear == this || visited.Contains(gear) || !gear.isSnap) continue;

            float dist = Vector3.Distance(transform.position, gear.transform.position);
            if (dist <= connectDistance)
            {
                gear.SetReachHeadRecursive(visited);
            }
        }
    }

    void CheckNearGear()
    {
        if (!isSnap)
        {
            Count = 0;
            return;
        }

        HashSet<ControllGear> visited = new HashSet<ControllGear>();
        Queue<ControllGear> queue = new Queue<ControllGear>();
        int newCount = 0;

        queue.Enqueue(this);
        visited.Add(this);

        while (queue.Count > 0)
        {
            ControllGear current = queue.Dequeue();

            foreach (ControllGear gear in allGears)
            {
                if (gear == current || visited.Contains(gear) || !gear.isSnap) continue;

                float dist = Vector2.Distance(current.transform.position, gear.transform.position);
                if (dist <= connectDistance)
                {
                    visited.Add(gear);
                    queue.Enqueue(gear);
                    if (gear.NearMain) newCount++;
                }
            }
        }

        if (NearMain) newCount++;
        Count = newCount;
    }

    // =============================================
    // 🔹 ROTATION
    // =============================================
    void CheckedRotate()
    {
        if (isRotate) return;

        if (isStart)
        {
            if (but1)
                Gear.transform.rotation = Quaternion.Euler(0, 0, 22.5f);
            else if (but2)
                Gear.transform.rotation = Quaternion.Euler(0, 0, 0);
            else
                Gear.transform.rotation = Quaternion.identity;

            isStart = false;
            isRotate = true;
        }
    }

    void Rotate()
    {
        if (!isReachHead) return;

        isReach = true;

        if (!hasSetTarget)
        {
            float currentZ = Gear.transform.rotation.eulerAngles.z;
            float targetZ = currentZ + (but1 && isSnap ? 45f : (but2 && isSnap ? -45f : 0f));
            targetZ = NormalizeAngle(targetZ);
            targetRotation = Quaternion.Euler(0, 0, targetZ);
            hasSetTarget = true;
        }

        Gear.transform.rotation = Quaternion.RotateTowards(
            Gear.transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        if (Quaternion.Angle(Gear.transform.rotation, targetRotation) < 0.001f)
        {
            isReach = false;
            isReachHead = false;
            hasSetTarget = false;
        }
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        return (angle < 0) ? angle + 360f : angle;
    }

    void CheckedNearMain()
    {
        if (!isSnap || mainObj == null) return;
        NearMain = Vector3.Distance(transform.position, mainObj.transform.position) < 1f;
    }

    // =============================================
    // 🔹 COLLISION
    // =============================================
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SnapZone1") || other.CompareTag("SnapZone2"))
        {
            isReachSnap = true;
            snapTarget = other.transform;
        }
        else if (other.CompareTag("Bin"))
        {
            isReachBin = true;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("SnapZone1") || other.CompareTag("SnapZone2"))
            snapTarget = other.transform;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((other.CompareTag("SnapZone1") || other.CompareTag("SnapZone2")) && snapTarget == other.transform)
        {
            isReachSnap = false;
            snapTarget = tempSnap;
        }
        else if (other.CompareTag("Bin"))
        {
            isReachBin = false;
        }
    }
}
