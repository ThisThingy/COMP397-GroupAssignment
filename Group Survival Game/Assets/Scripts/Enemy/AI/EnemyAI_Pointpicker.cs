using UnityEngine;
using UnityEngine.AI;

public class EnemyAI_Pointpicker : MonoBehaviour
{
    public float range = 10.0f; // Max distance from the center to search for a valid point

    public Vector3 GetRandomNavMeshPosition(Vector3 center)
    {
        for (int i = 0; i < 30; i++) // Try up to 30 times to find a valid point
        {
            Vector3 randomDirection = Random.insideUnitSphere * range;
            randomDirection += center; // Offset from the center point

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.LogWarning("Couldn't find nearest randompos.");
        return Vector3.zero;
    }
}
