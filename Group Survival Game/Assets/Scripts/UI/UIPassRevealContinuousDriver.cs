using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIPassRevealContinuousDriver : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Image targetImage;
    [SerializeField] private Material baseMaterial;

    [Header("Sprites")]
    [SerializeField] private Sprite blackWhiteSprite;
    [SerializeField] private Sprite colorSprite;
    [SerializeField] private Sprite passMaskSprite;

    [Header("Shader Settings")]
    [SerializeField] private float scrollSpeed = 0.5f;
    [SerializeField] private float tilingX = 1f;
    [SerializeField] private float tilingY = 1f;
    [SerializeField] private float offsetY = 0f;
    [SerializeField] [Range(0f, 1f)] private float maskStrength = 1f;

    private Material runtimeMaterial;
    private float manualScrollX;

    private static readonly int ColorTexID = Shader.PropertyToID("_ColorTex");
    private static readonly int PassMaskTexID = Shader.PropertyToID("_PassMaskTex");
    private static readonly int ManualScrollXID = Shader.PropertyToID("_ManualScrollX");
    private static readonly int TilingXID = Shader.PropertyToID("_TilingX");
    private static readonly int TilingYID = Shader.PropertyToID("_TilingY");
    private static readonly int OffsetYID = Shader.PropertyToID("_OffsetY");
    private static readonly int MaskStrengthID = Shader.PropertyToID("_MaskStrength");

    private void Awake()
    {
        Setup(true);
    }

    private void OnEnable()
    {
        Setup(true);
    }

    private void Update()
    {
        if (targetImage == null) return;
        if (runtimeMaterial == null) Setup(false);
        if (runtimeMaterial == null) return;

        manualScrollX += Time.unscaledDeltaTime * scrollSpeed;
        ApplyShaderValues();
    }

    public void RefreshForMenuOpen()
    {
        Setup(true);
    }

    public void RestartAnimation()
    {
        Setup(true);
    }

    private void Setup(bool resetScroll)
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        if (targetImage == null)
            return;

        if (baseMaterial == null)
            baseMaterial = targetImage.material;

        if (baseMaterial == null)
        {
            Debug.LogWarning("[UIPassRevealContinuousDriver] baseMaterial is null.", this);
            return;
        }

        CleanupRuntimeMaterial();

        runtimeMaterial = new Material(baseMaterial);
        runtimeMaterial.name = baseMaterial.name + " (Runtime)";
        targetImage.material = runtimeMaterial;

        if (blackWhiteSprite != null)
            targetImage.sprite = blackWhiteSprite;

        if (resetScroll)
            manualScrollX = 0f;

        ApplyShaderValues();

        targetImage.SetMaterialDirty();
        targetImage.SetVerticesDirty();
        Canvas.ForceUpdateCanvases();
    }

    private void ApplyShaderValues()
    {
        if (runtimeMaterial == null) return;

        if (colorSprite != null)
            runtimeMaterial.SetTexture(ColorTexID, colorSprite.texture);

        if (passMaskSprite != null)
            runtimeMaterial.SetTexture(PassMaskTexID, passMaskSprite.texture);

        runtimeMaterial.SetFloat(ManualScrollXID, manualScrollX);
        runtimeMaterial.SetFloat(TilingXID, tilingX);
        runtimeMaterial.SetFloat(TilingYID, tilingY);
        runtimeMaterial.SetFloat(OffsetYID, offsetY);
        runtimeMaterial.SetFloat(MaskStrengthID, maskStrength);
    }

    private void OnDisable()
    {
        CleanupRuntimeMaterial();
    }

    private void OnDestroy()
    {
        CleanupRuntimeMaterial();
    }

    private void CleanupRuntimeMaterial()
    {
        if (runtimeMaterial == null) return;

        if (targetImage != null && targetImage.material == runtimeMaterial)
            targetImage.material = baseMaterial;

        if (Application.isPlaying)
            Destroy(runtimeMaterial);
        else
            DestroyImmediate(runtimeMaterial);

        runtimeMaterial = null;
    }
}