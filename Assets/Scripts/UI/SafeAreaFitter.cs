using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    RectTransform _rectTransform;
    Rect _lastSafeArea;
    Vector2Int _lastScreenSize;

    void Awake() => _rectTransform = GetComponent<RectTransform>();

    void OnEnable() => ApplySafeArea();

    void Update()
    {
        var screenSize = new Vector2Int(Screen.width, Screen.height);
        if (_lastSafeArea != Screen.safeArea || _lastScreenSize != screenSize)
            ApplySafeArea();
    }

    void ApplySafeArea()
    {
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
}
