using UnityEngine;

public class PlayerAutoDamageTest : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private int damagePerTick = 30;
    [SerializeField] private float interval = 1f;
    [SerializeField] private bool startOnPlay = true;

    private float timer;
    private bool isRunning;

    void Start()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        isRunning = startOnPlay;
        timer = interval;
    }

    void Update()
    {
        if (!isRunning || playerHealth == null)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            playerHealth.TakeDamage(damagePerTick);
            timer = interval;
        }
    }

    public void StartAutoDamage()
    {
        isRunning = true;
        timer = interval;
    }

    public void StopAutoDamage()
    {
        isRunning = false;
    }
}