using TMPro;
using UnityEngine;

public class InidictextLocatora : MonoBehaviour
{
    public bool enter = false;
    public Transform entry;
    public Transform exit;
    public float speed;
    public TMP_Text textcol;
    Color col;

    void Start()
    {
        col = textcol.color;
        col.a = 0.0f;
    }

    void Update()
    {
        if(enter == true)
        {
            gameObject.transform.position = Vector3.Lerp(transform.position, entry.position, speed * Time.deltaTime);
            col.a = 1.0f;
            textcol.color = Color.Lerp(textcol.color, col, speed * Time.deltaTime);  
        }else
        if(enter == false)
        {
            gameObject.transform.position = Vector3.Lerp(transform.position, exit.position, speed * Time.deltaTime);
            col.a = 0.0f;
            textcol.color = Color.Lerp(textcol.color, col, speed * Time.deltaTime);
        }
    }
}
