using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI_Base : EnemyStats
{
    public Transform player;

    NavMeshAgent nvme;
    EnemyStats stats_enemigo;
    EnemyAI_Pointpicker enpoi;
    Vector3 targLoc;
    public EnemyAI_CrudeCombat enecom;

    public float distanceDetection = 10.0f;
    public float attentionSpan = 0.0f;
    public bool caseMode; // IF CaseMode Is on, the enemy will continually search for player, if it is off it will just stand guard.
    public int casingModeId = 0;//Only used if casing mode is on.
                                //0 - Cruising - Slowly checking random location
                                //1 - Curious - Slowly checking General direction of
                                //2 - Hunting - Enemy now knows of player location and continously chase until out of range, drop back to 1 if lost.
                                //3 - Battle - Enemy is in battle stance, circles around player and trade blows.

    void Start()
    {
        targLoc = Vector3.zero;
        enpoi = gameObject.GetComponent<EnemyAI_Pointpicker>();
        nvme = gameObject.GetComponent<NavMeshAgent>();
        nvme.speed = 5.5f;
        targLoc = enpoi.GetRandomNavMeshPosition(transform.position);
        nvme.SetDestination(targLoc);
        stats_enemigo = GetComponent<EnemyStats>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceDetection = Vector3.Distance(transform.position, player.position);
        Debug.LogAssertion(distanceDetection);
        if (distanceDetection > 20)
        {
            enecom.setAttackFalse();
            casingModeId = 0;
        }    
        if (distanceDetection <= 20 && distanceDetection > 10)
        {
            enecom.setAttackFalse();
            casingModeId = 1;
        }
        if (distanceDetection <= 10 && distanceDetection > 5)
        {
            enecom.setAttackFalse();
            casingModeId = 2;
        }
        if(distanceDetection <= 2)
        {
            enecom.setAttackActive();
            casingModeId = 3;
        }


        if (caseMode == true)
        {
            switch (casingModeId)
            {
                case 0:
                    Cruising();
                break;

                case 1:
                    Curious();
                break;
                case 2:
                    Hunting();
                break;
                case 3:
                    Attack();
                break;
            }
        }
    }

    void StandGuard()
    {

    }

    void Cruising()
    {
        if(attentionSpan <= 0)
        {
            targLoc = enpoi.GetRandomNavMeshPosition(transform.position);
            nvme.SetDestination(targLoc);
            nvme.speed = 1.5f;
            attentionSpan = 5.0f;
        }
        else
        {
            nvme.SetDestination(targLoc);
            attentionSpan -= Time.deltaTime;
        }
    }

    void Curious()
    {
        if (attentionSpan <= 0)
        {
            targLoc = enpoi.GetRandomNavMeshPosition(player.position);
            nvme.SetDestination(targLoc);
            nvme.speed = 2.5f;
            attentionSpan = 3.5f;
        }
        else
        {
            nvme.SetDestination(targLoc);
            attentionSpan -= Time.deltaTime;
        }
    }

    void Hunting()
    {
        if (attentionSpan <= 0)
        {
            nvme.SetDestination(player.position);
            nvme.speed = 3.0f;
            attentionSpan = 2.5f;
        }
        else
        {
            nvme.SetDestination(player.position);
            attentionSpan -= Time.deltaTime;
        }
    }

    void Attack()
    {
        if (attentionSpan <= 0)
        {
            nvme.SetDestination(player.position);
            nvme.speed = 3.0f;
            attentionSpan = 2.5f;
        }
        else
        {
            nvme.SetDestination(player.position);
            attentionSpan -= Time.deltaTime;
        }
    }
}


public class EnemyStats : MonoBehaviour
{
    [SerializeField] public int Lv = 0;
    float Health = 100;
    float Stamina = 100;

    public void LvAdjusta()
    {
        Health = Health + (Lv * 5);
        Stamina = Stamina + (Lv * 3);
    }
}
