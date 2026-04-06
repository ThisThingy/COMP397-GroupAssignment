using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveItemRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;

    private string filePath;
    private System.Action<string> onLoad;
    private System.Action<string> onDelete;

    public void Setup(string path, bool showLoad, bool showDelete,
                      System.Action<string> onLoadClick,
                      System.Action<string> onDeleteClick)
    {
        filePath = path;
        onLoad = onLoadClick;
        onDelete = onDeleteClick;

        if (label != null)
            label.text = SaveSystem.GetDisplayName(path);

        if (loadButton != null)
        {
            loadButton.gameObject.SetActive(showLoad);
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(() => onLoad?.Invoke(filePath));
        }

        if (deleteButton != null)
        {
            deleteButton.gameObject.SetActive(showDelete);
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(() => onDelete?.Invoke(filePath));
        }
    }
}