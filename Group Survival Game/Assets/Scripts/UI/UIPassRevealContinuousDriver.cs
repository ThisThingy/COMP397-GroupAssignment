using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIPassRevealContinuousDriver : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Image targetImage;

    [Header("Sprites")]
    [SerializeField] private Sprite blackWhiteSprite;
    [SerializeField] private Sprite colorSprite;
    [SerializeField] private Sprite passMaskSprite;

    [Header("Shader Settings")]
    [SerializeField] private float scrollSpeed = 0.3f;
    [SerializeField] private float tilingX = 2f;
    [SerializeField] private float tilingY = 1f;
    [SerializeField] private float offsetY = 0f;
    [SerializeField][Range(0f, 1f)] private float maskStrength = 1f;

    private Material runtimeMat;

    private static readonly int ColorTexID = Shader.PropertyToID("_ColorTex");
    private static readonly int PassMaskTexID = Shader.PropertyToID("_PassMaskTex");
    private static readonly int ScrollSpeedID = Shader.PropertyToID("_ScrollSpeed");
    private static readonly int TilingXID = Shader.PropertyToID("_TilingX");
    private static readonly int TilingYID = Shader.PropertyToID("_TilingY");
    private static readonly int OffsetYID = Shader.PropertyToID("_OffsetY");
    private static readonly int MaskStrengthID = Shader.PropertyToID("_MaskStrength");

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        if (targetImage == null || targetImage.material == null)
        {
            Debug.LogError("[UIPassRevealContinuousDriver] Image 或 Material 未设置。", this);
            enabled = false;
            return;
        }

        runtimeMat = new Material(targetImage.material);
        targetImage.material = runtimeMat;

        ApplyAll();
    }

    private void ApplyAll()
    {
        if (blackWhiteSprite != null)
            targetImage.sprite = blackWhiteSprite;

        if (colorSprite != null)
            runtimeMat.SetTexture(ColorTexID, colorSprite.texture);

        if (passMaskSprite != null)
            runtimeMat.SetTexture(PassMaskTexID, passMaskSprite.texture);

        runtimeMat.SetFloat(ScrollSpeedID, scrollSpeed);
        runtimeMat.SetFloat(TilingXID, tilingX);
        runtimeMat.SetFloat(TilingYID, tilingY);
        runtimeMat.SetFloat(OffsetYID, offsetY);
        runtimeMat.SetFloat(MaskStrengthID, maskStrength);
    }

    private void OnValidate()
    {
        if (runtimeMat != null)
        {
            runtimeMat.SetFloat(ScrollSpeedID, scrollSpeed);
            runtimeMat.SetFloat(TilingXID, tilingX);
            runtimeMat.SetFloat(TilingYID, tilingY);
            runtimeMat.SetFloat(OffsetYID, offsetY);
            runtimeMat.SetFloat(MaskStrengthID, maskStrength);
        }
    }

    private void OnDestroy()
    {
        if (runtimeMat != null)
        {
            if (Application.isPlaying)
                Destroy(runtimeMat);
            else
                DestroyImmediate(runtimeMat);
        }
    }
}