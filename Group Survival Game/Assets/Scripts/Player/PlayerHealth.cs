using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject gameplayMenu;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string uiActionMap = "UI";

    private bool isDead = false;
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

    if (currentHealth <= 0)
    {
        Death();
        return;
    }

    if (gameOverMenu != null)
        gameOverMenu.SetActive(false);
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
    if (isDead) return;

    currentHealth -= damage;

    if (currentHealth < 0)
    {
        currentHealth = 0;
    }

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
     if (isDead) return;
     isDead = true;
 
     Debug.Log("Player has died.");

     if (gameplayMenu != null)
        gameplayMenu.SetActive(false);

     if (gameOverMenu != null)
     {
        gameOverMenu.SetActive(true);
        gameOverMenu.transform.SetAsLastSibling();
     }

     Time.timeScale = 0f;

     Cursor.visible = true;
     Cursor.lockState = CursorLockMode.None;

     if (playerInput != null)
{
    playerInput.enabled = true;
    playerInput.ActivateInput();

    if (!string.IsNullOrEmpty(uiActionMap))
        playerInput.SwitchCurrentActionMap(uiActionMap);
}
   }
}
