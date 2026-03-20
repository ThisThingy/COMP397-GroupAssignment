using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnemyAI_CrudeCombat : MonoBehaviour
{
    [SerializeField] GameObject player, glovol;
    public bool attackStance = false;
    public float stanceReady = 3.5f;
    public int dmg = 4;
    public ChromaticAberration chroma;
    

    private void Start()
    {
        glovol.GetComponent<Volume>().profile.TryGet<ChromaticAberration>(out chroma);


    }

    public void setAttackActive()
    {
        attackStance = true;

    }
    public void setAttackFalse()
    {
        attackStance = false;
        stanceReady = 3.5f;

    }

    void Update()
    {
        if(attackStance == true)
        {
            if(stanceReady > 0)
            {
                stanceReady -= Time.deltaTime;
            }
            if(stanceReady <= 0)
            {
                player.GetComponent<PlayerHealth>().TakeDamage(dmg);
                showDmgEffecta();
                stanceReady = 3.5f;
            }
        }
    }

    void showDmgEffecta()
    {
        while (chroma.intensity.value != 1)
        {
            chroma.intensity.value += Time.deltaTime;
        }
        Invoke("unshowDemgEffecta", 0.5f);
    }

    void unshowDemgEffecta()
    {
        while (chroma.intensity.value > 0)
        {
            chroma.intensity.value -= Time.deltaTime * 0.04f;
        }
    }
}
