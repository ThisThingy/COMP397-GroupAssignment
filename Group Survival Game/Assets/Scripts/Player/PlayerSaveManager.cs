using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveStates
{
    public Vector3 pos;
    public Quaternion rot;
}

public class PlayerSaveManager : MonoBehaviour
{
    public void SaveGame()
    {
        SaveStates data = new SaveStates();

        data.pos = gameObject.transform.position;
        data.rot = gameObject.transform.rotation;

        string json = JsonUtility.ToJson(data, true);

        string path = Path.Combine(Application.persistentDataPath, "save1.sav");
        Debug.Log("Saved to: " + Application.persistentDataPath);

        File.WriteAllText(path, json);
        Debug.Log("Saved to: " + path);
    }

    public void LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "save1.sav");

        if (File.Exists(path))
        {
            SaveStates dete = new SaveStates();
            string json = File.ReadAllText(path);
            SaveStates data = JsonUtility.FromJson<SaveStates>(json);

            dete.pos  = data.pos;
            dete.rot = data.rot;

            gameObject.transform.position = dete.pos;
            gameObject.transform.rotation = dete.rot;
        }
    }
}
