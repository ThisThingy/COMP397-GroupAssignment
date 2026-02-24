using UnityEngine;
using UnityEngine.EventSystems;

public class UIShowHideOnClick : MonoBehaviour, IPointerClickHandler
{
    [Header("Show / Hide")]
    [SerializeField] private GameObject[] objectsToShow;
    [SerializeField] private GameObject[] objectsToHide;

    [Header("Optional Time Control")]
    [SerializeField] private bool pauseGameTime = true;   
    [SerializeField] private bool resumeGameTime = false; 

    public void OnPointerClick(PointerEventData eventData)
    {
        if (objectsToShow != null)
        {
            for (int i = 0; i < objectsToShow.Length; i++)
            {
                if (objectsToShow[i] != null)
                    objectsToShow[i].SetActive(true);
            }
        }

        if (objectsToHide != null)
        {
            for (int i = 0; i < objectsToHide.Length; i++)
            {
                if (objectsToHide[i] != null)
                    objectsToHide[i].SetActive(false);
            }
        }

        if (pauseGameTime)
            Time.timeScale = 0f;

        if (resumeGameTime)
            Time.timeScale = 1f;
    }
}