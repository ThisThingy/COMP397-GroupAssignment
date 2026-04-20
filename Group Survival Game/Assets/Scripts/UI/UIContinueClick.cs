using UnityEngine;
using UnityEngine.EventSystems;

public class UIContinueClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PauseToggleOnUI pauseController;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (pauseController != null)
        {
            pauseController.Resume();
        }
        else
        {
            Time.timeScale = 1f;
            Debug.LogWarning("[UIContinueClick] pauseController -- only Time.timeScale=1");
        }
    }
}