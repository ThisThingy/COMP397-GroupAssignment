using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveStates
{
    public Transform transforma;
}

public class PlayerSaveManager : MonoBehaviour
{
    public void SaveGame()
    {
        SaveStates data = new SaveStates();

        data.transforma = gameObject.transform;

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

            dete.transforma  = data.transforma;

            gameObject.transform.position = dete.transforma.position;
            gameObject.transform.rotation = dete.transforma.rotation;
        }
    }
}
