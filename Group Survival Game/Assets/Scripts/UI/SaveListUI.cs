using UnityEngine;

public class SaveListUI : MonoBehaviour
{
    public enum Mode { Manage, Load }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform contentParent;
    [SerializeField] private SaveItemRowUI rowPrefab;
    [SerializeField] private Mode mode = Mode.Load;

    [Header("Panels")]
    [SerializeField] private GameObject saveListPanel;
    [SerializeField] private GameObject optionsMenuCanvas;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject gameplayMenuCanvas;    // ✅ Canvas_GamePlayMenu（HUD）
    [SerializeField] private GameObject playerRoot;

    [Header("Resume Gameplay After Load")]
    [Tooltip("67")]
    [SerializeField] private Behaviour[] behavioursToEnableAfterLoad;

    [Header("Cursor After Load")]
    [SerializeField] private bool lockCursorAfterLoad = true;
    [SerializeField] private bool hideCursorAfterLoad = true;

    private void OnEnable()
    {
        Refresh();
    }

    public void SetMode(Mode m)
    {
        mode = m;
        Refresh();
    }

    public void Refresh()
    {
        if (contentParent == null || rowPrefab == null)
        {
            Debug.LogWarning("[SaveListUI] contentParent 或 rowPrefab 未赋值。");
            return;
        }

        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        var files = SaveSystem.ListSaveFilesNewestFirst();

        bool showLoad = (mode == Mode.Load);
        bool showDelete = (mode == Mode.Manage);

        for (int i = 0; i < files.Count; i++)
        {
            var row = Instantiate(rowPrefab, contentParent);
            row.Setup(files[i], showLoad, showDelete, OnLoadClicked, OnDeleteClicked);
        }
    }

    private void OnLoadClicked(string path)
    {
        if (player == null)
        {
            Debug.LogWarning("[SaveListUI] Player not assigned.");
            return;
        }

        bool ok = SaveSystem.LoadFromFile(player, path);
        Debug.Log(ok ? $"Loaded: {path}" : $"Load failed: {path}");

        if (!ok) return;

        CloseMenusAndResumeGameplay();
    }

    private void OnDeleteClicked(string path)
    {
        bool ok = SaveSystem.DeleteFile(path);
        Debug.Log(ok ? $"Deleted: {path}" : $"Delete failed: {path}");
        Refresh();
    }

    private void CloseMenusAndResumeGameplay()
    {
      
        if (playerRoot != null)
            playerRoot.SetActive(true);


        if (saveListPanel != null) saveListPanel.SetActive(false);
        if (optionsMenuCanvas != null) optionsMenuCanvas.SetActive(false);
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);

        
        if (gameplayMenuCanvas != null) gameplayMenuCanvas.SetActive(true);

 
        Time.timeScale = 1f;


        if (behavioursToEnableAfterLoad != null)
        {
            foreach (var b in behavioursToEnableAfterLoad)
                if (b != null) b.enabled = true;
        }


        if (hideCursorAfterLoad) Cursor.visible = false;
        Cursor.lockState = lockCursorAfterLoad ? CursorLockMode.Locked : CursorLockMode.None;
    }
}