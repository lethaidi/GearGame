using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraAutoFitWidth2D : MonoBehaviour
{
    [Header("?? phân gi?i g?c (trong Editor)")]
    public Vector2 referenceResolution = new Vector2(1920, 1080);

    [Header("Orthographic Size g?c (trong Editor)")]
    public float baseOrthoSize = 5f;

    private Camera cam;
    private float referenceAspect;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null || !cam.orthographic)
        {
            Debug.LogWarning("?? CameraAutoFitWidth2D ch? dùng cho camera Orthographic (2D).");
            return;
        }

        referenceAspect = referenceResolution.x / referenceResolution.y;
        UpdateCameraScale();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
            UpdateCameraScale();
    }
#endif

    void UpdateCameraScale()
    {
        if (cam == null) return;

        float currentAspect = (float)Screen.width / Screen.height;
        float widthScale = currentAspect / referenceAspect;

        // Gi? chi?u r?ng ?úng theo t? l? màn hình, chi?u cao theo camera g?c
        cam.orthographicSize = baseOrthoSize / widthScale;
    }
}
