using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Achievement Boxes (UI Image)")]
    [SerializeField] private Image wipeOutEnemiesBox;
    [SerializeField] private Image walkMetersBox;
    [SerializeField] private Image surviveTenMinutesBox;

    [Header("Sprites")]
    [SerializeField] private Sprite uncheckedSprite;   // ACHIEVEMENT BOX
    [SerializeField] private Sprite checkedSprite;     // CHECKED BOX

    [Header("Enemy Check")]
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float enemyCheckInterval = 0.5f;

    [Header("Targets")]
    [SerializeField] private float walkTargetMeters = 676767f;
    [SerializeField] private float surviveTargetSeconds = 600f; // 10 min

    private bool wipeOutEnemiesUnlocked;
    private bool walkMetersUnlocked;
    private bool surviveTenMinutesUnlocked;

    private Vector3 lastPlayerPosition;
    private float totalDistanceWalked;
    private float survivalTime;
    private float enemyCheckTimer;

    private void Start()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
        }

        // 开局先全部显示未完成
        SetBoxSprite(wipeOutEnemiesBox, uncheckedSprite);
        SetBoxSprite(walkMetersBox, uncheckedSprite);
        SetBoxSprite(surviveTenMinutesBox, uncheckedSprite);
    }

    private void Update()
    {
        if (player != null)
        {
            UpdateWalkDistance();
        }

        UpdateSurvivalTime();
        UpdateEnemyCheck();
    }

    private void UpdateWalkDistance()
    {
        if (walkMetersUnlocked) return;

        float distanceThisFrame = Vector3.Distance(player.position, lastPlayerPosition);
        totalDistanceWalked += distanceThisFrame;
        lastPlayerPosition = player.position;

        if (totalDistanceWalked >= walkTargetMeters)
        {
            UnlockWalkMeters();
        }
    }

    private void UpdateSurvivalTime()
    {
        if (surviveTenMinutesUnlocked) return;

        survivalTime += Time.deltaTime;

        if (survivalTime >= surviveTargetSeconds)
        {
            UnlockSurviveTenMinutes();
        }
    }

    private void UpdateEnemyCheck()
    {
        if (wipeOutEnemiesUnlocked) return;

        enemyCheckTimer += Time.deltaTime;
        if (enemyCheckTimer < enemyCheckInterval) return;

        enemyCheckTimer = 0f;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        if (enemies.Length == 0)
        {
            UnlockWipeOutEnemies();
        }
    }

    private void UnlockWipeOutEnemies()
    {
        if (wipeOutEnemiesUnlocked) return;

        wipeOutEnemiesUnlocked = true;
        SetBoxSprite(wipeOutEnemiesBox, checkedSprite);
        Debug.Log("Achievement unlocked: WIPE OUT ALL ENEMIES");
    }

    private void UnlockWalkMeters()
    {
        if (walkMetersUnlocked) return;

        walkMetersUnlocked = true;
        SetBoxSprite(walkMetersBox, checkedSprite);
        Debug.Log("Achievement unlocked: WALK 676767 METERS");
    }

    private void UnlockSurviveTenMinutes()
    {
        if (surviveTenMinutesUnlocked) return;

        surviveTenMinutesUnlocked = true;
        SetBoxSprite(surviveTenMinutesBox, checkedSprite);
        Debug.Log("Achievement unlocked: SURVIVE FOR 10 MINUTES");
    }

    private void SetBoxSprite(Image targetBox, Sprite spriteToSet)
    {
        if (targetBox != null && spriteToSet != null)
        {
            targetBox.sprite = spriteToSet;
        }
    }

    // ---------- 可选：给你测试用 ----------
    [ContextMenu("Unlock Wipe Out All Enemies")]
    private void TestUnlockEnemies()
    {
        UnlockWipeOutEnemies();
    }

    [ContextMenu("Unlock Walk Meters")]
    private void TestUnlockWalk()
    {
        UnlockWalkMeters();
    }

    [ContextMenu("Unlock Survive 10 Minutes")]
    private void TestUnlockSurvive()
    {
        UnlockSurviveTenMinutes();
    }
}