using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIRebindImageMaterialOnEnable : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Material sourceMaterial;
    [SerializeField] private bool cloneMaterialOnEnable = true;

    private Material runtimeMaterial;

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        if (sourceMaterial == null && targetImage != null)
            sourceMaterial = targetImage.material;
    }

    private void OnEnable()
    {
        StartCoroutine(RebindNextFrame());
    }

    private IEnumerator RebindNextFrame()
    {
        yield return null;
        RebindMaterial();
    }

    private void RebindMaterial()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        if (targetImage == null)
            return;

        if (sourceMaterial == null)
            sourceMaterial = targetImage.material;

        if (sourceMaterial == null)
        {
            Debug.LogWarning("[UIRebindImageMaterialOnEnable] sourceMaterial is null.", this);
            return;
        }

        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
            runtimeMaterial = null;
        }

        runtimeMaterial = cloneMaterialOnEnable
            ? new Material(sourceMaterial)
            : sourceMaterial;

        targetImage.material = runtimeMaterial;
        targetImage.SetMaterialDirty();
        targetImage.SetVerticesDirty();
        Canvas.ForceUpdateCanvases();
    }

    private void OnDisable()
    {
        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
            runtimeMaterial = null;
        }
    }
}