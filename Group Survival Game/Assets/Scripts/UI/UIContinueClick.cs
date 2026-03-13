using UnityEngine;
using UnityEngine.EventSystems;

public class UIContinueClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PauseToggleOnUI pauseController; // 你现在的暂停脚本（挂在 pause 图标上的那个）

    public void OnPointerClick(PointerEventData eventData)
    {
        if (pauseController != null)
        {
            pauseController.Resume(); // 用我们之前脚本里的 Resume()
        }
        else
        {
            // 兜底：至少恢复时间并关掉自己所在的面板（如果你愿意也可以删掉）
            Time.timeScale = 1f;
            Debug.LogWarning("[UIContinueClick] pauseController 未赋值，已仅恢复 Time.timeScale=1");
        }
    }
}