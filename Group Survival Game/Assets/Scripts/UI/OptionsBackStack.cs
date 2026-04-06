using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OptionsBackStack : MonoBehaviour
{
    [Header("Roots")]
    [SerializeField] private GameObject optionsRoot;   // Canvas_OptionsMenu
    [SerializeField] private GameObject startMenu;     // Canvas_StartMenu
    [SerializeField] private GameObject pauseMenu;     // Canvas_PauseMenu

    [Header("Behavior")]
    [SerializeField] private bool keepPausedWhenReturnToPause = true;

    private readonly Stack<GameObject> stack = new Stack<GameObject>();

    // 记录这次是从哪个入口进 Options 的
    private GameObject returnTarget;
    private bool returnPausedState;

    private void Reset()
    {
        optionsRoot = gameObject;
    }

    private void OnEnable()
    {
        ResetStack();
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
            optionsRoot.SetActive(true);

        ResetStack();
    }

    private void ResetStack()
    {
        stack.Clear();

        if (optionsRoot != null)
        {
            optionsRoot.SetActive(true);
            stack.Push(optionsRoot);
        }
    }

    public void Push(GameObject panel)
    {
        if (panel == null) return;

        if (stack.Count > 0 && stack.Peek() == panel) return;

        panel.SetActive(true);
        stack.Push(panel);
    }

    public void Back()
    {
        // 如果已经退到 Options 根层，就回到最初入口
        if (stack.Count <= 1)
        {
            if (optionsRoot != null)
                optionsRoot.SetActive(false);

            if (returnTarget != null)
                returnTarget.SetActive(true);

            Time.timeScale = returnPausedState ? 0f : 1f;
            return;
        }

        // 还在子面板层，就退回上一个面板
        GameObject top = stack.Pop();
        if (top != null && top != optionsRoot)
            top.SetActive(false);

        GameObject prev = stack.Peek();
        if (prev != null)
            prev.SetActive(true);
    }
}