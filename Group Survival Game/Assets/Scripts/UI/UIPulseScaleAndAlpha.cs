using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIPulseScaleAndAlpha : MonoBehaviour
{
    [Header("Scale Pulse")]
    [SerializeField] private bool enableScalePulse = true;
    [SerializeField] private float scaleAmount = 0.05f;  
    [SerializeField] private float scaleSpeed = 1.5f;    

    [Header("Alpha Pulse (optional)")]
    [SerializeField] private bool enableAlphaPulse = false;
    [SerializeField][Range(0f, 1f)] private float minAlpha = 0.5f; 
    [SerializeField] private float alphaSpeed = 1.5f;               

    [Header("Phase Offset (optional)")]
    [SerializeField] private float phaseOffset = 0f; 

    private RectTransform rectTransform;
    private Vector3 initialScale;

    private Graphic uiGraphic;      
    private CanvasGroup canvasGroup;

    private Color initialColor;
    private float initialAlpha = 1f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialScale = rectTransform.localScale;

        canvasGroup = GetComponent<CanvasGroup>();


        uiGraphic = GetComponent<Graphic>();

        if (canvasGroup != null)
        {
            initialAlpha = canvasGroup.alpha;
        }
        else if (uiGraphic != null)
        {
            initialColor = uiGraphic.color;
            initialAlpha = initialColor.a;
        }
    }

    private void OnEnable()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        initialScale = rectTransform.localScale;
    }

    private void Update()
    {
        float t = Time.unscaledTime + phaseOffset;

     
        if (enableScalePulse)
        {
            float wave = (Mathf.Sin(t * scaleSpeed * Mathf.PI * 2f) + 1f) * 0.5f; // 0..1
            float scaleMul = 1f + (wave * scaleAmount);

            rectTransform.localScale = new Vector3(
                initialScale.x * scaleMul,
                initialScale.y * scaleMul,
                initialScale.z
            );
        }

        if (enableAlphaPulse)
        {
            float waveA = (Mathf.Sin(t * alphaSpeed * Mathf.PI * 2f) + 1f) * 0.5f; // 0..1
            float a = Mathf.Lerp(minAlpha, initialAlpha, waveA); // min -> initial

            if (canvasGroup != null)
            {
                canvasGroup.alpha = a;
            }
            else if (uiGraphic != null)
            {
                Color c = uiGraphic.color;
                c.a = a;
                uiGraphic.color = c;
            }
        }
    }

    private void OnDisable()
    {

        if (rectTransform != null)
            rectTransform.localScale = initialScale;

        if (enableAlphaPulse)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = initialAlpha;
            }
            else if (uiGraphic != null)
            {
                Color c = uiGraphic.color;
                c.a = initialAlpha;
                uiGraphic.color = c;
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (scaleAmount < 0f) scaleAmount = 0f;
        if (scaleSpeed < 0f) scaleSpeed = 0f;
        if (alphaSpeed < 0f) alphaSpeed = 0f;
        minAlpha = Mathf.Clamp01(minAlpha);
    }
#endif
}