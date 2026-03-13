using UnityEngine;

public class SceneStartup : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject playerRoot;          // Player
    [SerializeField] private GameObject startMenuCanvas;     // Canvas_StartMenu
    [SerializeField] private GameObject gameplayMenuCanvas;  // Canvas_GamePlayMenu
    [SerializeField] private GameObject pauseMenuCanvas;     // Canvas_PauseMenu

    private const string SkipKey = "SKIP_START_MENU_ONCE";

    private void Start()
    {
        Time.timeScale = 1f;

        bool skipOnce = PlayerPrefs.GetInt(SkipKey, 0) == 1;
        PlayerPrefs.SetInt(SkipKey, 0); // 用完就清掉

        if (skipOnce)
        {
            // 直接进游戏（跳过开始菜单）
            if (playerRoot) playerRoot.SetActive(true);
            if (startMenuCanvas) startMenuCanvas.SetActive(false);
            if (gameplayMenuCanvas) gameplayMenuCanvas.SetActive(true);
            if (pauseMenuCanvas) pauseMenuCanvas.SetActive(false);
        }
        else
        {
            // 正常开局：先开始菜单
            if (playerRoot) playerRoot.SetActive(false);
            if (startMenuCanvas) startMenuCanvas.SetActive(true);
            if (gameplayMenuCanvas) gameplayMenuCanvas.SetActive(false);
            if (pauseMenuCanvas) pauseMenuCanvas.SetActive(false);
        }
    }
}