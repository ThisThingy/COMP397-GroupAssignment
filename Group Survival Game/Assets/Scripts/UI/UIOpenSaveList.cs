using UnityEngine;
using UnityEngine.EventSystems;

public class UIOpenSaveList : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject saveListPanel; // 你的 SaveListPanel
    [SerializeField] private SaveListUI saveListUI;
    [SerializeField] private SaveListUI.Mode mode;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (saveListPanel != null) saveListPanel.SetActive(true);
        if (saveListUI != null) saveListUI.SetMode(mode);
    }
}