using UnityEngine;
using UnityEngine.EventSystems;

public class UIStartEnablePlayer : MonoBehaviour, IPointerClickHandler
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject playerRoot;       
    [SerializeField] private GameObject startMenuCanvas;   
    [SerializeField] private GameObject gameplayMenuCanvas; 
    [SerializeField] private GameObject pauseMenuCanvas;    

    public void OnPointerClick(PointerEventData eventData)
    {
        Time.timeScale = 1f;

        if (playerRoot != null) playerRoot.SetActive(true);

        if (startMenuCanvas != null) startMenuCanvas.SetActive(false);
        if (gameplayMenuCanvas != null) gameplayMenuCanvas.SetActive(true);
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
    }
}