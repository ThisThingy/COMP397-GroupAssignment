using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// EnemyAI — Three-state FSM: Patrol → Chase → Attack
///
/// Requirements
/// ────────────
/// • NavMeshAgent on this GameObject (baked NavMesh in scene)
/// • A trigger Collider on a child GameObject for the attack range
///   (assign attackTrigger in the Inspector)
/// • The player must have the tag "Player"
/// • The player's health script must expose: public void TakeDamage(int damage)
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    // ── Inspector ──────────────────────────────────────────────────────────
    [Header("Detection")]
    [Tooltip("Radius at which the enemy detects and chases the Player.")]
    [SerializeField] private float detectionRange = 10f;

    [Tooltip("How often (seconds) the enemy re-checks whether the Player is in range.")]
    [SerializeField] private float detectionTickRate = 0.2f;

    [Header("Patrol")]
    [Tooltip("How far from its current position the enemy will pick a random patrol point.")]
    [SerializeField] private float patrolRadius = 15f;

    [Tooltip("How long the enemy waits at a patrol point before picking the next one.")]
    [SerializeField] private float patrolWaitTime = 2f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 5f;

    [Header("Patrol Speed")]
    [SerializeField] private float patrolSpeed = 2.5f;

    [Header("Attack")]
    [Tooltip("Damage dealt to the player per hit.")]
    [SerializeField] private int attackDamage = 25;

    [Tooltip("Time between attacks (seconds).")]
    [SerializeField] private float attackCooldown = 1f;

    [Tooltip("Child GameObject that holds the trigger Collider used as the attack range.")]
    [SerializeField] private Collider attackTrigger;

    // ── Private State ──────────────────────────────────────────────────────
    private enum AIState { Patrol, Chase, Attack }
    private AIState currentState = AIState.Patrol;

    private NavMeshAgent agent;
    private Transform playerTransform;

    private Vector3 lastKnownPlayerPosition;
    private bool hasLastKnownPosition = false;

    private bool isWaitingAtPatrolPoint = false;
    private bool canAttack = true;
    private bool playerInAttackRange = false;

    // ── Unity Lifecycle ────────────────────────────────────────────────────
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    private void FindPlayer()
    {
        
        // Validate attack trigger assignment
        if (attackTrigger == null)
            Debug.LogWarning($"[EnemyAI] '{name}': No attack trigger assigned. Attack range will not work.");

        // Find the player once at startup (tag lookup is cheap here)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogWarning("[EnemyAI] No GameObject with tag 'Player' found in scene.");
    }

    private void OnEnable()
    {
        // Kick off the detection loop as a coroutine to avoid per-frame OverlapSphere calls
        StartCoroutine(DetectionRoutine());
    }

    private void Update()
    {
        FindPlayer();
        switch (currentState)
        {
            case AIState.Patrol: HandlePatrol(); break;
            case AIState.Chase:  HandleChase();  break;
            case AIState.Attack: HandleAttack(); break;
        }
    }

    // ── Detection (Coroutine) ──────────────────────────────────────────────

    /// <summary>Polls detection range at a fixed tick rate to keep CPU cost low.</summary>
    private IEnumerator DetectionRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(detectionTickRate);

        while (true)
        {
            yield return wait;

            if (playerTransform == null) continue;

            float distance = Vector3.Distance(transform.position, playerTransform.position);
            bool playerVisible = distance <= detectionRange;

            switch (currentState)
            {
                case AIState.Patrol:
                    if (playerVisible)
                        TransitionTo(AIState.Chase);
                    break;

                case AIState.Chase:
                    if (!playerVisible)
                    {
                        // Record last known position before losing the player
                        lastKnownPlayerPosition = playerTransform.position;
                        hasLastKnownPosition = true;
                        TransitionTo(AIState.Patrol);
                    }
                    break;

                // Attack state is managed by trigger callbacks; detection just keeps tabs
                case AIState.Attack:
                    if (!playerVisible && !playerInAttackRange)
                        TransitionTo(AIState.Patrol);
                    break;
            }
        }
    }

    // ── State Handlers ─────────────────────────────────────────────────────

    private void HandlePatrol()
    {
        if (isWaitingAtPatrolPoint) return;

        // Reached the current destination (or has none yet)
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitThenPickNextPatrolPoint());
        }
    }

    private void HandleChase()
    {
        if (playerTransform == null) return;

        // Continuously update destination to follow the player
        agent.SetDestination(playerTransform.position);
        lastKnownPlayerPosition = playerTransform.position; // keep updating while we can see them
        hasLastKnownPosition = true;
    }

    private void HandleAttack()
    {
        // Face the player while attacking
        if (playerTransform != null)
        {
            Vector3 dir = (playerTransform.position - transform.position).normalized;
            dir.y = 0f;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(dir),
                                                      Time.deltaTime * 8f);
        }

        // Trigger-based attack; DealDamage is called from OnTriggerStay
    }

    // ── Patrol Helpers ─────────────────────────────────────────────────────

    private IEnumerator WaitThenPickNextPatrolPoint()
    {
        isWaitingAtPatrolPoint = true;
        agent.ResetPath();

        yield return new WaitForSeconds(patrolWaitTime);

        isWaitingAtPatrolPoint = false;

        // If we just arrived at a last-known-position, clear it so we patrol freely
        hasLastKnownPosition = false;

        SetRandomPatrolDestination();
    }

    private void SetRandomPatrolDestination()
    {
        // Try a few times to find a valid NavMesh point
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
            Vector3 candidate = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                return;
            }
        }

        Debug.LogWarning($"[EnemyAI] '{name}': Could not find a valid patrol point after 10 attempts.");
    }

    // ── Attack Trigger Callbacks ───────────────────────────────────────────

    /// <summary>
    /// Called by the child attack-range trigger when the Player enters it.
    /// Wire this up by attaching a TriggerRelay component (see bottom of file)
    /// or by placing this script on the child with the trigger collider.
    /// </summary>
    public void OnAttackTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInAttackRange = true;
        agent.ResetPath(); // Stop moving — we're in attack range
        TransitionTo(AIState.Attack);
    }

    public void OnAttackTriggerStay(Collider other)
    {
        if (currentState != AIState.Attack) return;
        if (!other.CompareTag("Player")) return;

        if (canAttack)
            StartCoroutine(PerformAttack(other));
    }

    public void OnAttackTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInAttackRange = false;

        // Resume chasing if the player is still in detection range
        if (playerTransform != null &&
            Vector3.Distance(transform.position, playerTransform.position) <= detectionRange)
        {
            TransitionTo(AIState.Chase);
        }
        else
        {
            lastKnownPlayerPosition = other.transform.position;
            hasLastKnownPosition = true;
            TransitionTo(AIState.Patrol);
        }
    }

    // ── Attack Logic ───────────────────────────────────────────────────────

    private IEnumerator PerformAttack(Collider playerCollider)
    {
        canAttack = false;

        // Grab the health component and apply damage
        PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
        else
        {
            Debug.LogWarning($"[EnemyAI] '{name}': Player does not have a PlayerHealth component.");
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // ── State Machine ──────────────────────────────────────────────────────

    private void TransitionTo(AIState newState)
    {
        if (currentState == newState) return;

        // ── Exit current state ──
        switch (currentState)
        {
            case AIState.Patrol:
                StopCoroutine(nameof(WaitThenPickNextPatrolPoint));
                isWaitingAtPatrolPoint = false;
                break;

            case AIState.Chase:
                break;

            case AIState.Attack:
                break;
        }

        currentState = newState;

        // ── Enter new state ──
        switch (newState)
        {
            case AIState.Patrol:
                agent.speed = patrolSpeed;
                agent.isStopped = false;

                if (hasLastKnownPosition)
                {
                    // Walk to last known position first, then patrol from there
                    agent.SetDestination(lastKnownPlayerPosition);
                }
                else
                {
                    SetRandomPatrolDestination();
                }
                break;

            case AIState.Chase:
                agent.speed = chaseSpeed;
                agent.isStopped = false;
                break;

            case AIState.Attack:
                agent.isStopped = true;
                agent.ResetPath();
                break;
        }
    }

    // ── Gizmos ─────────────────────────────────────────────────────────────
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Patrol radius
        Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        // Last known position
        if (hasLastKnownPosition)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(lastKnownPlayerPosition, 0.4f);
            Gizmos.DrawLine(transform.position, lastKnownPlayerPosition);
        }
    }
#endif
}

