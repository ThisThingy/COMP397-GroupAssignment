using UnityEngine;

public class EnemyAttackTriggerRelay : MonoBehaviour
{
    private EnemyAI enemyAI;

    private void Awake()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
        if (enemyAI == null)
            Debug.LogError("[EnemyAttackTriggerRelay] No EnemyAI found in parent hierarchy.");
    }

    private void OnTriggerEnter(Collider other)  => enemyAI?.OnAttackTriggerEnter(other);
    private void OnTriggerStay(Collider other)   => enemyAI?.OnAttackTriggerStay(other);
    private void OnTriggerExit(Collider other)   => enemyAI?.OnAttackTriggerExit(other);
}