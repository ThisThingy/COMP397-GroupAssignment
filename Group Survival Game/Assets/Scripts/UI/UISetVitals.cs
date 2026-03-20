using UnityEngine;
using UnityEngine.TextCore;
using TMPro;

public class UISetVitals : MonoBehaviour
{
    
    public PlayerHealth plah;
    TextMeshProUGUI current;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        current = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        current.text = plah.showHealth().ToString();
    }
}
