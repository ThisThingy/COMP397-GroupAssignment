using UnityEngine;

public class LookatRaycast : MonoBehaviour
{
    [SerializeField] Camera currentCam;
    public IndicatorSetter indset;
    public Object_Interactable intrObj;
    RaycastHit touched;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray LookatScanner = currentCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(LookatScanner, out touched, 10.0f))
        {
            if (touched.transform.tag == "interactable")
            {
                indset.HoverEntry(touched.collider.GetComponent<Object_Interactable>().itemName);
            }
        }
        else
        {
            indset.HoverExit();
        }


    }
}
