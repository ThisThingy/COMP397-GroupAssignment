using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneStartAfterReload : MonoBehaviour
{
    public static bool PendingStart = false;

    [Header("Player")]
    [SerializeField] private GameObject playerRoot;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string gameplayActionMap = "Player";

    [Header("Camera")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Camera[] camerasToDisable;

    [Header("UI")]
    [SerializeField] private GameObject[] objectsToHide;
    [SerializeField] private GameObject[] objectsToShow;

    private void Start()
    {
        if (PendingStart)
        {
            StartCoroutine(ApplyStartNextFrame());
        }
    }

    private IEnumerator ApplyStartNextFrame()
    {
        // 先等场景初始化完
        yield return null;

        PendingStart = false;

        Time.timeScale = 1f;

        // 先启用 Player 根对象
        if (playerRoot != null && !playerRoot.activeSelf)
            playerRoot.SetActive(true);

        // 再等一帧，确保 PlayerInput 真正启用
        yield return null;

        if (playerInput == null && playerRoot != null)
            playerInput = playerRoot.GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            playerInput.enabled = true;
            playerInput.ActivateInput();

            if (!string.IsNullOrEmpty(gameplayActionMap))
                playerInput.SwitchCurrentActionMap(gameplayActionMap);
        }

        if (camerasToDisable != null)
        {
            for (int i = 0; i < camerasToDisable.Length; i++)
            {
                if (camerasToDisable[i] != null)
                    camerasToDisable[i].gameObject.SetActive(false);
            }
        }

        if (targetCamera != null)
            targetCamera.gameObject.SetActive(true);

        if (objectsToHide != null)
        {
            for (int i = 0; i < objectsToHide.Length; i++)
            {
                if (objectsToHide[i] != null)
                    objectsToHide[i].SetActive(false);
            }
        }

        if (objectsToShow != null)
        {
            for (int i = 0; i < objectsToShow.Length; i++)
            {
                if (objectsToShow[i] != null)
                    objectsToShow[i].SetActive(true);
            }
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("[SceneStartAfterReload] Start state applied after scene reload.");
    }
}