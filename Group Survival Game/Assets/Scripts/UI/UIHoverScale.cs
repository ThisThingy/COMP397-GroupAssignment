using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale")]
    [SerializeField] private Vector3 hoverScale = new Vector3(0.92f, 0.92f, 1f);
    [SerializeField] private float speed = 12f;

    private RectTransform rectTransform;
    private Vector3 initialScale;
    private Vector3 targetScale;
    private bool initialized;

    private void Awake()
    {
        CacheReferences();
        ResetStateImmediate();
    }

    private void OnEnable()
    {
        CacheReferences();
        ResetStateImmediate();
    }

    private void OnDisable()
    {
        if (!initialized) return;
        ResetStateImmediate();
    }

    private void Update()
    {
        if (rectTransform == null) return;

        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.unscaledDeltaTime * speed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = new Vector3(
            initialScale.x * hoverScale.x,
            initialScale.y * hoverScale.y,
            initialScale.z * hoverScale.z
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = initialScale;
    }

    private void CacheReferences()
    {
        if (initialized) return;

        rectTransform = GetComponent<RectTransform>();
        initialScale = rectTransform.localScale;
        targetScale = initialScale;
        initialized = true;
    }

    public void ResetStateImmediate()
    {
        targetScale = initialScale;

        if (rectTransform != null)
            rectTransform.localScale = initialScale;
    }

    public void ResetStateSmooth()
    {
        targetScale = initialScale;
    }
}