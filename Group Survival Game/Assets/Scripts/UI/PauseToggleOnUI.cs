using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseToggleOnUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject gameplayMenuCanvas;
    [SerializeField] private bool hideGameplayMenuWhenPaused = false;

    [Header("Disable Player Look / Input When Paused")]
    [Tooltip("把负责视角转动的脚本拖进来（比如 MouseLook/CameraLook/CinemachineInput 等）。也可以拖 PlayerInput。")]
    [SerializeField] private Behaviour[] disableWhenPaused;

    [Header("Cursor")]
    [SerializeField] private bool manageCursor = true;
    [SerializeField] private bool lockCursorWhenPlaying = true;

    private bool isPaused;
    private bool isInventoryOpen;

    private void Start()
    {
        // 确保开局是正常状态
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);

        ApplyCursorPlaying();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        SetPaused(!isPaused);
    }
    public void SetInventoryOpen(bool open)
    {
        isInventoryOpen = open;

        // Disable player look when inventory is open (same as pause)
        if (disableWhenPaused != null)
        {
            for (int i = 0; i < disableWhenPaused.Length; i++)
            {
                if (disableWhenPaused[i] != null)
                    disableWhenPaused[i].enabled = !open;
            }
        }

        // Cursor handling
        if (manageCursor && !isPaused)
        {
            if (open)
                ApplyCursorPaused();   // show cursor
            else
                ApplyCursorPlaying(); // hide cursor
        }
    }

    // 给 PauseMenu 的 Resume 按钮绑定这个
    public void Resume()
    {
        SetPaused(false);
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(paused);

        if (gameplayMenuCanvas != null && hideGameplayMenuWhenPaused)
            gameplayMenuCanvas.SetActive(!paused);

        // 关键：暂停时间
        Time.timeScale = paused ? 0f : 1f;

        // 关键：禁用视角/输入脚本，防止相机继续转
        if (disableWhenPaused != null)
        {
            for (int i = 0; i < disableWhenPaused.Length; i++)
            {
                if (disableWhenPaused[i] != null)
                    disableWhenPaused[i].enabled = !paused;
            }
        }

        // 关键：鼠标切换（暂停时释放鼠标给UI）
        if (manageCursor)
        {
            if (paused)
                ApplyCursorPaused();
            else if (!isInventoryOpen) // Account for inventory
                ApplyCursorPlaying();
        }
    }

    private void ApplyCursorPaused()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ApplyCursorPlaying()
    {
        Cursor.visible = false;
        Cursor.lockState = lockCursorWhenPlaying ? CursorLockMode.Locked : CursorLockMode.None;
    }
}