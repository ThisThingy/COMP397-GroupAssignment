using UnityEngine;

public class DeadzoneTeleport : MonoBehaviour
{
    public Transform teleportTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
                other.transform.position = teleportTarget.position;
                controller.enabled = true;
            }
        }
    }
}
