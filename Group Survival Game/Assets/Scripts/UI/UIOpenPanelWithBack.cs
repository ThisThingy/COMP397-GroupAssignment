using UnityEngine;
using UnityEngine.EventSystems;

public class UIOpenPanelWithBack : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private OptionsBackStack backStack; // Canvas_OptionsMenu 上的 OptionsBackStack
    [SerializeField] private GameObject panelToOpen;     // SaveListPanel
    [SerializeField] private SaveListUI saveListUI;     
    [SerializeField] private SaveListUI.Mode mode;       // Load 或 Manage

    public void OnPointerClick(PointerEventData eventData)
    {
        if (backStack != null && panelToOpen != null)
            backStack.Push(panelToOpen);

        if (saveListUI != null)
            saveListUI.SetMode(mode);
    }
}