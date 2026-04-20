using UnityEngine;
using TMPro;
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public int attackDamage = 20;
    public float attackRange = 1f;  //no logic yet
    public float attackSpeed = 1f;  //no logic yet
    public LayerMask enemyLayers;   //no logic yet
    public TextMeshProUGUI healthText;

    void Start()
    {
     currentHealth = maxHealth;
     UpdateHealthText();
    }
   void UpdateHealthText()
   {
     if (healthText != null)
     {
        healthText.text = currentHealth.ToString();
     }
   }
    public int showHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthText();
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    public void Heal(int amount)    //temp, idk if we'll need it
   {
     currentHealth += amount;
     if (currentHealth > maxHealth)
     {
        currentHealth = maxHealth;
     }

    UpdateHealthText();
   }

    public void Attack(GameObject target)
    {
        PlayerHealth enemyHealth = target.GetComponent<PlayerHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(attackDamage);
        }
    }
    
    private void Death()
    {
        Debug.Log("Player has died.");
    }
}
