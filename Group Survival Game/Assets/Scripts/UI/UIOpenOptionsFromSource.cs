using UnityEngine;
using UnityEngine.EventSystems;

public class UIOpenOptionsFromSource : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private OptionsBackStack backStack;
    [SerializeField] private GameObject sourceRoot;
    [SerializeField] private bool keepPaused;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (backStack == null) return;

        backStack.OpenFrom(sourceRoot, keepPaused);
    }
}