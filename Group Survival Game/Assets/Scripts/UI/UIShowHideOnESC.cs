using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIShowHideOnESC : MonoBehaviour
{
    [Header("Show / Hide")]
    [SerializeField] private GameObject[] objectsToShow;
    [SerializeField] private GameObject[] objectsToHide;

    [Header("Optional Time Control")]
    [SerializeField] private bool pauseGameTime = true;
    [SerializeField] private bool resumeGameTime = false;
    

    private void Awake()
    {
        
    }

    private void Update()
    {
        Debug.Log("Recieved");
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (objectsToShow != null)
            {
                for (int i = 0; i < objectsToShow.Length; i++)
                {
                    if (objectsToShow[i] != null)
                    {
                        objectsToShow[i].SetActive(true);
                        Cursor.lockState = CursorLockMode.None;
                    }
                }
            }

            if (objectsToHide != null)
            {
                for (int i = 0; i < objectsToHide.Length; i++)
                {
                    if (objectsToHide[i] != null)
                    {
                        objectsToHide[i].SetActive(false);
                        Cursor.lockState = CursorLockMode.None;
                    }
                }
            }

            if (pauseGameTime)
                Time.timeScale = 0f;

            if (resumeGameTime)
                Time.timeScale = 1f;
        }
    }


}
