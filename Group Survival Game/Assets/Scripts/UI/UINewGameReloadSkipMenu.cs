using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UINewGameReloadSkipMenu : MonoBehaviour, IPointerClickHandler
{
    private const string SkipKey = "SKIP_START_MENU_ONCE";

    public void OnPointerClick(PointerEventData eventData)
    {
        Time.timeScale = 1f;

   
        PlayerPrefs.SetInt(SkipKey, 1);

        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}