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
    private bool isHovering;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialScale = rectTransform.localScale;
        targetScale = initialScale;
    }

    private void OnEnable()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        initialScale = rectTransform.localScale;
        targetScale = initialScale;
    }

    private void Update()
    {
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.unscaledDeltaTime * speed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetScale = new Vector3(
            initialScale.x * hoverScale.x,
            initialScale.y * hoverScale.y,
            initialScale.z * hoverScale.z
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetScale = initialScale;
    }

}