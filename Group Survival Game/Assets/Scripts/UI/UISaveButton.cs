using UnityEngine;
using UnityEngine.EventSystems;

public class UISaveButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform player; 

    public void OnPointerClick(PointerEventData eventData)
    {
        if (player == null)
        {
            Debug.LogWarning("Player not assigned.");
            return;
        }

        SaveSystem.SaveNew(player);
    }
}