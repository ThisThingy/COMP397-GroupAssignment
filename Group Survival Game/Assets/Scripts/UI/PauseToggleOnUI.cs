using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseToggleOnUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject gameplayMenuCanvas;
    [SerializeField] private bool hideGameplayMenuWhenPaused = false;

    [Header("Disable Player Look When Paused")]
    [Tooltip(" MouseLook、CameraLook、CinemachineInputProvider。no PlayerInput")]
    [SerializeField] private Behaviour[] disableWhenPaused;

    [Header("Input Action Map")]
    [Tooltip(" PlayerInput ")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string gameplayActionMap = "Player";
    [SerializeField] private string pauseActionMap = "UI";

    [Header("Cursor")]
    [SerializeField] private bool manageCursor = true;
    [SerializeField] private bool lockCursorWhenPlaying = true;

    private bool isPaused;
    private bool isInventoryOpen;

    private void Start()
    {
        isPaused = false;
        isInventoryOpen = false;
        Time.timeScale = 1f;

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);

        if (gameplayMenuCanvas != null && hideGameplayMenuWhenPaused)
            gameplayMenuCanvas.SetActive(true);

        SetLookScriptsEnabled(true);

        if (playerInput != null)
        {
            playerInput.enabled = true;

            if (!string.IsNullOrEmpty(gameplayActionMap))
                playerInput.SwitchCurrentActionMap(gameplayActionMap);
        }

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

    public void Resume()
{
    SetPaused(false);
}

public void Pause()
{
    SetPaused(true);
}

    public void SetInventoryOpen(bool open)
    {
        isInventoryOpen = open;

       
        if (isPaused)
        {
            SetLookScriptsEnabled(false);
        }
        else
        {
            SetLookScriptsEnabled(!open);
        }

        if (manageCursor && !isPaused)
        {
            if (open)
                ApplyCursorPaused();
            else
                ApplyCursorPlaying();
        }
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(paused);

        if (gameplayMenuCanvas != null && hideGameplayMenuWhenPaused)
            gameplayMenuCanvas.SetActive(!paused);

        Time.timeScale = paused ? 0f : 1f;

        
        if (paused)
            SetLookScriptsEnabled(false);
        else
            SetLookScriptsEnabled(!isInventoryOpen);


        if (playerInput != null)
        {
            playerInput.enabled = true;

            string targetMap = paused ? pauseActionMap : gameplayActionMap;
            if (!string.IsNullOrEmpty(targetMap))
            {
                playerInput.SwitchCurrentActionMap(targetMap);
            }
        }

        if (manageCursor)
        {
            if (paused)
                ApplyCursorPaused();
            else if (!isInventoryOpen)
                ApplyCursorPlaying();
        }

        Debug.Log($"[PauseToggleOnUI] isPaused={isPaused}, timeScale={Time.timeScale}");
        if (playerInput != null && playerInput.currentActionMap != null)
        {
            Debug.Log($"[PauseToggleOnUI] currentActionMap={playerInput.currentActionMap.name}");
        }
    }

    private void SetLookScriptsEnabled(bool enabledState)
    {
        if (disableWhenPaused == null) return;

        for (int i = 0; i < disableWhenPaused.Length; i++)
        {
            if (disableWhenPaused[i] != null)
                disableWhenPaused[i].enabled = enabledState;
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