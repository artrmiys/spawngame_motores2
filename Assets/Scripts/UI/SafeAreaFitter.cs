using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    [SerializeField] Vector2 referenceResolution = new Vector2(1080, 1920);
    [SerializeField] float childPadding = 18f;

    RectTransform _rectTransform;
    Rect _lastSafeArea;
    Vector2Int _lastScreenSize;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        EnsureCanvasScaler();
    }

    void OnEnable() => ApplySafeArea();

    void LateUpdate()
    {
        var screenSize = new Vector2Int(Screen.width, Screen.height);
        if (_lastSafeArea != Screen.safeArea || _lastScreenSize != screenSize)
            ApplySafeArea();

        ClampDirectChildrenToSafeArea();
    }

    void ApplySafeArea()
    {
        EnsureCanvasScaler();

        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();

        _lastSafeArea = Screen.safeArea;
        _lastScreenSize = new Vector2Int(Screen.width, Screen.height);

        if (Screen.width <= 0 || Screen.height <= 0)
            return;

        Vector2 anchorMin = _lastSafeArea.position;
        Vector2 anchorMax = _lastSafeArea.position + _lastSafeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
        _rectTransform.offsetMin = Vector2.zero;
        _rectTransform.offsetMax = Vector2.zero;
    }

    void EnsureCanvasScaler()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        var scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
            scaler = canvas.gameObject.AddComponent<CanvasScaler>();

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        scaler.referencePixelsPerUnit = 100f;
    }

    void ClampDirectChildrenToSafeArea()
    {
        if (_rectTransform == null)
            return;

        Rect parentRect = _rectTransform.rect;
        if (parentRect.width <= 0f || parentRect.height <= 0f)
            return;

        for (int i = 0; i < _rectTransform.childCount; i++)
        {
            if (_rectTransform.GetChild(i) is RectTransform child)
                ClampChild(child, parentRect);
        }
    }

    void ClampChild(RectTransform child, Rect parentRect)
    {
        if (child.anchorMin != child.anchorMax)
            return;

        Rect childRect = child.rect;
        if (childRect.width <= 0f || childRect.height <= 0f)
            return;

        Vector2 anchorLocal = new Vector2(
            Mathf.Lerp(parentRect.xMin, parentRect.xMax, child.anchorMin.x),
            Mathf.Lerp(parentRect.yMin, parentRect.yMax, child.anchorMin.y));

        float minX = parentRect.xMin - anchorLocal.x + childRect.width * child.pivot.x + childPadding;
        float maxX = parentRect.xMax - anchorLocal.x - childRect.width * (1f - child.pivot.x) - childPadding;
        float minY = parentRect.yMin - anchorLocal.y + childRect.height * child.pivot.y + childPadding;
        float maxY = parentRect.yMax - anchorLocal.y - childRect.height * (1f - child.pivot.y) - childPadding;

        Vector2 position = child.anchoredPosition;
        position.x = minX <= maxX ? Mathf.Clamp(position.x, minX, maxX) : (minX + maxX) * 0.5f;
        position.y = minY <= maxY ? Mathf.Clamp(position.y, minY, maxY) : (minY + maxY) * 0.5f;
        child.anchoredPosition = position;
    }
}
