using UnityEngine;
using UnityEngine.EventSystems;

public class UIStartEnablePlayer : MonoBehaviour, IPointerClickHandler
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject playerRoot;       
    [SerializeField] private GameObject startMenuCanvas;   
    [SerializeField] private GameObject gameplayMenuCanvas; 
    [SerializeField] private GameObject pauseMenuCanvas;
    [Tooltip("Loads Character with existing save")]
    public bool LoadWithSave = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (LoadWithSave == false)
        {
            Time.timeScale = 1f;

            if (playerRoot != null) playerRoot.SetActive(true);

            if (startMenuCanvas != null) startMenuCanvas.SetActive(false);
            if (gameplayMenuCanvas != null) gameplayMenuCanvas.SetActive(true);
            if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
        }
        else
        {
            Time.timeScale = 1f;

            if (playerRoot != null) playerRoot.SetActive(true);
            playerRoot.GetComponent<PlayerSaveManager>().LoadGame();
            if (startMenuCanvas != null) startMenuCanvas.SetActive(false);
            if (gameplayMenuCanvas != null) gameplayMenuCanvas.SetActive(true);
            if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
        }
    }
}