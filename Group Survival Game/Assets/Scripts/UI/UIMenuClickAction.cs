using UnityEngine;
using UnityEngine.EventSystems;

public class UIMenuClickAction : MonoBehaviour, IPointerClickHandler
{
    public enum ActionType
    {
        SwitchCamera,
        QuitGame,
        QuitAndSave
    }

    [Header("Action")]
    [SerializeField] private ActionType actionType = ActionType.SwitchCamera;

    [Header("Camera Switch (for Start / NewGame)")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Camera[] camerasToDisable;

    [Header("Optional UI")]
    [SerializeField] private GameObject[] objectsToHideOnClick;
    [SerializeField] private GameObject[] objectsToShowOnClick;

    [Header("Save / Quit")]
    [SerializeField] private GameObject player;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (actionType)
        {
            case ActionType.SwitchCamera:
                DoSwitchCamera();
                break;

            case ActionType.QuitGame:
                DoQuitGame();
                break;

            case ActionType.QuitAndSave:
                DoSaveQuit();
                break;
        }
    }

    private void DoSwitchCamera()
    {
        if (targetCamera == null)
        {
            Debug.LogWarning($"[{nameof(UIMenuClickAction)}] targetCamera 未设置：{gameObject.name}", this);
            return;
        }

        if (camerasToDisable != null)
        {
            for (int i = 0; i < camerasToDisable.Length; i++)
            {
                if (camerasToDisable[i] != null)
                    camerasToDisable[i].gameObject.SetActive(false);
            }
        }

        targetCamera.gameObject.SetActive(true);

        if (objectsToHideOnClick != null)
        {
            for (int i = 0; i < objectsToHideOnClick.Length; i++)
            {
                if (objectsToHideOnClick[i] != null)
                    objectsToHideOnClick[i].SetActive(false);
            }
        }

        if (objectsToShowOnClick != null)
        {
            for (int i = 0; i < objectsToShowOnClick.Length; i++)
            {
                if (objectsToShowOnClick[i] != null)
                    objectsToShowOnClick[i].SetActive(true);
            }
        }

        Debug.Log($"Switched to camera: {targetCamera.name}");
    }

    private void DoSaveQuit()
    {
        Debug.Log("Exit clicked -> Save and Quit");

        if (player != null)
        {
            string savePath = SaveSystem.SaveNew(player.transform);
            Debug.Log("Saved to: " + savePath);
        }
        else
        {
            Debug.LogWarning("[UIMenuClickAction] player 未设置，无法保存");
        }


        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void DoQuitGame()
    {
        Debug.Log("Exit clicked -> QuitGame()");

        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}