using UnityEngine;
using UnityEngine.InputSystem;

public class UIAchievementToggle : MonoBehaviour
{
    [Header("Required")]
    [SerializeField] private GameObject gameplayMenuCanvas;      // Canvas_GamePlayMenu
    [SerializeField] private GameObject achievementMenuCanvas;   // Canvas_AchievementMenu

    [Header("Optional Blockers")]
    [SerializeField] private GameObject[] blockIfAnyActive;      // Pause / Options / GameOver 

    [Header("Behavior")]
    [SerializeField] private bool hideAchievementWhenBlocked = true;
    [SerializeField] private bool hideAchievementWhenGameplayHidden = true;

    private void Start()
    {
        if (achievementMenuCanvas != null)
            achievementMenuCanvas.SetActive(false);
    }

    private void Update()
    {
        if (gameplayMenuCanvas == null || achievementMenuCanvas == null)
            return;

        bool gameplayVisible = gameplayMenuCanvas.activeInHierarchy;
        bool blocked = IsBlocked();

      
        if (!gameplayVisible)
        {
            if (hideAchievementWhenGameplayHidden && achievementMenuCanvas.activeSelf)
                achievementMenuCanvas.SetActive(false);

            return;
        }

        if (blocked)
        {
            if (hideAchievementWhenBlocked && achievementMenuCanvas.activeSelf)
                achievementMenuCanvas.SetActive(false);

            return;
        }

        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            achievementMenuCanvas.SetActive(!achievementMenuCanvas.activeSelf);
        }
    }

    private bool IsBlocked()
    {
        if (blockIfAnyActive == null) return false;

        for (int i = 0; i < blockIfAnyActive.Length; i++)
        {
            if (blockIfAnyActive[i] != null && blockIfAnyActive[i].activeInHierarchy)
                return true;
        }

        return false;
    }
}