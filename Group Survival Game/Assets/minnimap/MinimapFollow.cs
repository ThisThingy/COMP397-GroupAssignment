using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // Player
    [SerializeField] private float height = 50f;
    [SerializeField] private bool rotateWithTarget = false;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = target.position;
        pos.y += height;
        transform.position = pos;

        if (rotateWithTarget)
        {
            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}