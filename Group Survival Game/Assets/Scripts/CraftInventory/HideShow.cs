using UnityEngine;

public class HideShow : MonoBehaviour
{
    [SerializeField] GameObject invetoryUI;
    public bool show = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(show == false)
            {
                show = true;
                invetoryUI.SetActive(true);
            }
            else
            {
                show = false;
                invetoryUI.SetActive(false);
            }
        }
    }
}
