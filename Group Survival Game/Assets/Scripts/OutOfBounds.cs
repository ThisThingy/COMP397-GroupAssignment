using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    // attach this script to any trigger collider that instantly kills the player
    // specifically if the player falls out of the map

    public int damage = 676767; // damage to apply

    private void OnTriggerEnter(Collider other)
    {
        // check if it's the player
        if (other.CompareTag("Player"))
        {
            // get health script from the player
            var health = other.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}