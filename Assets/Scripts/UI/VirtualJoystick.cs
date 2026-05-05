using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] RectTransform background;
    [SerializeField] RectTransform handle;
    [SerializeField] float handleRange = 1f;

    public Vector2 Direction { get; private set; }

    Canvas _canvas;
    Camera _cam;

    void Start()
    {
        if (background == null)
            background = GetComponent<RectTransform>();
        if (handle == null && transform.childCount > 0)
            handle = transform.GetChild(0).GetComponent<RectTransform>();

        if (background == null || handle == null)
        {
            enabled = false;
            return;
        }

        _canvas = GetComponentInParent<Canvas>();
        _cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        handle.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, _cam, out Vector2 localPoint);

        Vector2 clampedPoint = Vector2.ClampMagnitude(localPoint, background.sizeDelta.x * 0.5f * handleRange);
        handle.anchoredPosition = clampedPoint;
        Direction = clampedPoint / (background.sizeDelta.x * 0.5f * handleRange);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.anchoredPosition = Vector2.zero;
        Direction = Vector2.zero;
    }
}
