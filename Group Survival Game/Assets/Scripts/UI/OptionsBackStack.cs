using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OptionsBackStack : MonoBehaviour
{
    [Header("Roots")]
    [SerializeField] private GameObject optionsRoot;   // Canvas_OptionsMenu
    [SerializeField] private GameObject startMenu;     // Canvas_StartMenu
    [SerializeField] private GameObject pauseMenu;     // Canvas_PauseMenu
    [SerializeField] private GameObject gameplayMenu;  // Canvas_GamePlayMenu

    [Header("Pause / Input")]
    [SerializeField] private PauseToggleOnUI pauseController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string uiActionMap = "UI";

    [Header("Behavior")]
    [SerializeField] private bool keepPausedWhenReturnToPause = true;
    [SerializeField] private bool showGameplayMenuWhenReturnToPause = true;

    [Header("Optional Restart On Return")]
    [Tooltip("回到 StartMenu 时，需要重新启动的动画脚本。把 bgReveal 的驱动脚本、titlePivot 上的动画脚本拖进来。")]
    [SerializeField] private MonoBehaviour[] restartWhenReturnToStart;

    [Tooltip("回到 PauseMenu 时，需要重新启动的动画脚本。一般可留空，或者拖 pause menu 上需要重启动画的脚本。")]
    [SerializeField] private MonoBehaviour[] restartWhenReturnToPause;

    private readonly Stack<GameObject> stack = new Stack<GameObject>();

    private GameObject returnTarget;
    private bool returnPausedState;

    private void Reset()
    {
        optionsRoot = gameObject;
    }

    private void OnEnable()
    {
        ResetStack();
        ResetHoverStates(optionsRoot);
    }

    private void Update()
    {
        if (optionsRoot != null && !optionsRoot.activeInHierarchy) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Back();
        }
    }

    public void OpenFromStartMenu()
    {
        OpenFrom(startMenu, false);
    }

    public void OpenFromPauseMenu()
    {
        OpenFrom(pauseMenu, keepPausedWhenReturnToPause);
    }

    public void OpenFrom(GameObject sourceRoot, bool pausedState)
    {
        returnTarget = sourceRoot;
        returnPausedState = pausedState;

        if (sourceRoot != null)
            sourceRoot.SetActive(false);

        if (optionsRoot != null)
        {
            optionsRoot.SetActive(true);
            optionsRoot.transform.SetAsLastSibling();
            ResetHoverStates(optionsRoot);
        }

        ResetStack();
    }

    private void ResetStack()
    {
        stack.Clear();

        if (optionsRoot != null)
            stack.Push(optionsRoot);
    }

    public void Push(GameObject panel)
    {
        if (panel == null) return;
        if (stack.Count > 0 && stack.Peek() == panel) return;

        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
        ResetHoverStates(panel);
        stack.Push(panel);
    }

    public void Back()
    {
        if (stack.Count <= 1)
        {
            ReturnToSource();
            return;
        }

        GameObject top = stack.Pop();
        if (top != null && top != optionsRoot)
            top.SetActive(false);

        GameObject prev = stack.Peek();
        if (prev != null)
        {
            prev.SetActive(true);
            prev.transform.SetAsLastSibling();
            ResetHoverStates(prev);
        }
    }

    private void ReturnToSource()
    {
        if (optionsRoot != null)
            optionsRoot.SetActive(false);

        // ---------- 回到 PauseMenu ----------
        if (returnTarget == pauseMenu)
        {
            if (gameplayMenu != null && showGameplayMenuWhenReturnToPause)
                gameplayMenu.SetActive(true);

            if (pauseController != null && keepPausedWhenReturnToPause)
            {
                pauseController.Pause();
            }
            else
            {
                if (pauseMenu != null)
                    pauseMenu.SetActive(true);

                Time.timeScale = returnPausedState ? 0f : 1f;
            }

            ForcePauseAboveGameplay();

            SwitchToUIMap();

            ResetHoverStates(pauseMenu);
            ResetHoverStates(gameplayMenu);
            RestartAssignedBehaviours(restartWhenReturnToPause);
            return;
        }

        // ---------- 回到 StartMenu ----------
        if (returnTarget == startMenu)
        {
            // StartMenu 下不应该还留着 Pause / Gameplay
            if (pauseMenu != null)
                pauseMenu.SetActive(false);

            if (gameplayMenu != null)
                gameplayMenu.SetActive(false);

            if (startMenu != null)
            {
                startMenu.SetActive(true);
                startMenu.transform.SetAsLastSibling();
            }

            Time.timeScale = 1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            SwitchToUIMap();

            ResetHoverStates(startMenu);
            RestartAssignedBehaviours(restartWhenReturnToStart);
            return;
        }

        // ---------- 其他默认返回 ----------
        if (returnTarget != null)
        {
            returnTarget.SetActive(true);
            returnTarget.transform.SetAsLastSibling();
            ResetHoverStates(returnTarget);
        }

        Time.timeScale = returnPausedState ? 0f : 1f;
        SwitchToUIMap();
    }

    private void SwitchToUIMap()
    {
        if (playerInput != null)
        {
            playerInput.enabled = true;

            if (!string.IsNullOrEmpty(uiActionMap))
                playerInput.SwitchCurrentActionMap(uiActionMap);
        }
    }

    private void ForcePauseAboveGameplay()
    {
        if (gameplayMenu != null)
            gameplayMenu.transform.SetAsFirstSibling();

        if (pauseMenu != null)
            pauseMenu.transform.SetAsLastSibling();

        Canvas gameplayCanvas = gameplayMenu != null ? gameplayMenu.GetComponent<Canvas>() : null;
        Canvas pauseCanvas = pauseMenu != null ? pauseMenu.GetComponent<Canvas>() : null;

        if (gameplayCanvas != null && pauseCanvas != null)
        {
            gameplayCanvas.overrideSorting = true;
            pauseCanvas.overrideSorting = true;

            if (pauseCanvas.sortingOrder <= gameplayCanvas.sortingOrder)
                pauseCanvas.sortingOrder = gameplayCanvas.sortingOrder + 1;
        }
    }

    private void ResetHoverStates(GameObject root)
    {
        if (root == null) return;

        UIHoverScale[] hovers = root.GetComponentsInChildren<UIHoverScale>(true);
        for (int i = 0; i < hovers.Length; i++)
        {
            if (hovers[i] != null)
                hovers[i].ResetStateImmediate();
        }
    }

    private void RestartAssignedBehaviours(MonoBehaviour[] behaviours)
    {
        if (behaviours == null) return;

        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] == null) continue;

            behaviours[i].enabled = false;
            behaviours[i].enabled = true;
        }
    }
}