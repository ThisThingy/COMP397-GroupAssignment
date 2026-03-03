using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorSetter : MonoBehaviour
{
    public bool indicated;
    public Image img;
    [SerializeField] GameObject desc, inter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(indicated == true)
        {
            img.fillAmount = Mathf.Lerp(img.fillAmount, 1.0f, 25f * Time.deltaTime);
        }
        else if (indicated == false)
        {
            img.fillAmount = Mathf.Lerp(img.fillAmount, 0.0f, 25f * Time.deltaTime);
        }
    }

    public void HoverEntry(string itemName)
    {
        indicated = true;
        desc.GetComponent<InidictextLocatora>().enter = true;
        desc.GetComponent<TMP_Text>().text = itemName;
        inter.GetComponent<InidictextLocatora>().enter = true;



    }

    public void HoverExit()
    {
        indicated = false;
        desc.GetComponent<InidictextLocatora>().enter = false;
        inter.GetComponent<InidictextLocatora>().enter = false;
    }
}
