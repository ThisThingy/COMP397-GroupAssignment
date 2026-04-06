using UnityEngine;
using UnityEngine.EventSystems;

public class UIBackToPreviousPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private OptionsBackStack backStack;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (backStack != null)
            backStack.Back();
    }
}