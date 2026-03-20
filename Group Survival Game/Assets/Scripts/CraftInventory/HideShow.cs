using UnityEngine;

public class HideShow : MonoBehaviour
{
    [SerializeField] GameObject inventoryUI;
    public bool show = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(show == false)
            {
                show = true;
                inventoryUI.SetActive(true);
                Cursor.visible = true;
            }
            else
            {
                show = false;
                inventoryUI.SetActive(false);
                Cursor.visible = false;
                
            }
        }
    }
}
