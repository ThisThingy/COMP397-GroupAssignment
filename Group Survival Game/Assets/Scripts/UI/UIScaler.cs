using UnityEngine;
using UnityEngine.UI;

public class UIScaler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        gameObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.width, Screen.height);  
    }

    private void Update()
    {
        gameObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.width, Screen.height);
    }

}
