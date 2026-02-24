using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverTiltY : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tilt")]
    [SerializeField] private float hoverYAngle = 10f;   
    [SerializeField] private float speed = 12f;        
    [SerializeField] private bool invert = false;       

    [Header("Optional")]
    [SerializeField] private bool alsoScale = false;    
    [SerializeField] private float hoverScale = 1.03f;  

    private RectTransform rectTransform;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    private Vector3 initialScale;
    private Vector3 targetScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialRotation = rectTransform.localRotation;
        targetRotation = initialRotation;

        initialScale = rectTransform.localScale;
        targetScale = initialScale;
    }

    private void OnEnable()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

      
        initialRotation = rectTransform.localRotation;
        initialScale = rectTransform.localScale;

        targetRotation = initialRotation;
        targetScale = initialScale;
    }

    private void Update()
    {
        rectTransform.localRotation = Quaternion.Slerp(
            rectTransform.localRotation,
            targetRotation,
            Time.unscaledDeltaTime * speed
        );

        if (alsoScale)
        {
            rectTransform.localScale = Vector3.Lerp(
                rectTransform.localScale,
                targetScale,
                Time.unscaledDeltaTime * speed
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        float angle = invert ? -hoverYAngle : hoverYAngle;
        targetRotation = initialRotation * Quaternion.Euler(0f, angle, 0f);

        if (alsoScale)
        {
            targetScale = initialScale * hoverScale;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetRotation = initialRotation;
        targetScale = initialScale;
    }
}