using UnityEngine;
using TMPro;
using System.Collections;

public class tower : Shooter
{
    private bool isBuilt = false; // Tower starts as empty/unbuilt
    public int currentLevel = 0; // Tower level (1, 2, or 3)
    public float[] attackDelaysByLevel = {1.0f, 0.5f, 0.033f}; // Level 1 = slowest, Level 2 = medium, Level 3 = fastest
    public float attackDelay = 0.1f;

    public float detectionRange = 10f; // Range to detect enemies
    public float attackRange = 8f; // Range to attack enemies
    public LayerMask whatIsEnemy; // Layer mask for enemy detection
    public LayerMask whatIsPlayer; // Layer mask for player detection
    public float playerInteractionRange = 3f; // Range for player to interact with tower
    
    private Transform currentTarget;
    private bool enemyInDetectionRange;
    private bool enemyInAttackRange;

    private bool alreadyAttacked; // Cooldown flag for attacks

    public GameObject normalBulletPrefab; // Normal bullet prefab to use

    private bool playerInRange = false; // Track if player is near tower
    private PlayerControl nearbyPlayer = null; // Reference to nearby player
    public int towerCost; // Cost to buy tower
    public int upgradeCost; // Cost to upgrade tower per level

    public GameObject buy;
    public TextMeshProUGUI bText;

    public GameObject upgrade;
    public TextMeshProUGUI gText;

    public GameObject insufficient;
    public TextMeshProUGUI iText;

    public GameObject maximum;
    public TextMeshProUGUI mText;
    private bool showingInsufficient = false;

    [Header("Tower Levels Visuals")]
    public GameObject[] levelVisuals; // drag in level0..3 here in order (0,1,2,3)

    protected override void Start()
    {
        base.Start();
        isBuilt = false; // Tower starts unbuilt/empty
        enabled = true; // Enable to check for player interaction even when not built
        HideAllPopups();
        UpdateTowerVisual();
    }

    void Update()
    {
        CheckPlayerInRange();

        // ===== NOT BUILT =====
        if (!isBuilt)
        {
            if (playerInRange && !showingInsufficient)
            {
                HideAllPopups();
                ShowBuyPopup();

                if (Input.GetKeyDown(KeyCode.E) && nearbyPlayer != null)
                {
                    if (nearbyPlayer.coin >= towerCost)
                    {
                        nearbyPlayer.coin -= towerCost;
                        BuyTower();
                        HideAllPopups();
                    }
                    else
                    {
                        if (!showingInsufficient)
                            StartCoroutine(ShowInsufficientForSeconds(1f));
                    }
                }
            }
            else if (!playerInRange && !showingInsufficient)
            {
                HideAllPopups();
            }

            return;
        }

        // ===== BUILT =====
        if (playerInRange && !showingInsufficient)
        {
            if (currentLevel >= 3)
            {
                HideAllPopups();
                ShowMaximumPopup();
            }
            else
            {
                HideAllPopups();
                ShowUpgradePopup();
            }

            if (Input.GetKeyDown(KeyCode.E) && nearbyPlayer != null)
            {
                if (currentLevel >= 3)
                {
                    HideAllPopups();
                    ShowMaximumPopup();
                    Invoke(nameof(HideMaximumPopup), 2f);
                    return;
                }

                if (nearbyPlayer.coin >= upgradeCost)
                {
                    nearbyPlayer.coin -= upgradeCost;
                    UpgradeLevel();
                    HideAllPopups();
                }
                else
                {
                    if (!showingInsufficient)
                        StartCoroutine(ShowInsufficientForSeconds(1f));
                }
            }
        }
        else if (!playerInRange && !showingInsufficient)
        {
            HideAllPopups();
        }

        // ===== ATTACK LOGIC =====
        enemyInDetectionRange = Physics.CheckSphere(transform.position, detectionRange, whatIsEnemy);
        enemyInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsEnemy);

        if (isBuilt && (enemyInDetectionRange || enemyInAttackRange))
        {
            FindNearestEnemy();

            if (currentTarget != null)
            {
                RotateTowardTarget();
                if (enemyInAttackRange)
                    Attack();
            }
        }
        LockVisualRotation();
    }
    private void FindNearestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRange, whatIsEnemy);
        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (Collider enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }

        currentTarget = nearestEnemy;
    }

    private void RotateTowardTarget()
    {
        if (currentTarget == null)
            return;

        // get target position but keep y same as this object
        Vector3 targetPos = currentTarget.position;
        targetPos.y = transform.position.y;

        // look at target horizontally only
        transform.LookAt(targetPos);
    }

    private void Attack()
    {
        if (currentTarget == null) return;

        if (!alreadyAttacked)
        {
            shoot(elementType.Normal);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), attackDelay);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void CheckPlayerInRange()
    {
        // detect the player using physics overlap
        Collider[] players = Physics.OverlapSphere(transform.position, playerInteractionRange, whatIsPlayer);

        if (players.Length > 0)
        {
            playerInRange = true;
            nearbyPlayer = players[0].GetComponent<PlayerControl>(); // assume first collider is the player
        }
        else
        {
            playerInRange = false;
            nearbyPlayer = null;
        }
    }

    public bool BuyTower()
    {
        if (isBuilt) return false;

        isBuilt = true;
        currentLevel = 1;  // first actual level
        attackDelay = attackDelaysByLevel[currentLevel - 1];
        if (normalBulletPrefab != null) currentBulletPrefab = normalBulletPrefab;

        UpdateTowerVisual();
        enabled = true; 
        return true;
    }

    public bool UpgradeLevel()
    {
        if (!isBuilt) return false;
        if (currentLevel >= 3) return false;

        currentLevel++;
        attackDelay = attackDelaysByLevel[currentLevel - 1]; 

        UpdateTowerVisual();
        return true;
    }

    public bool IsBuilt() // Check if tower is built/initialized
    {
        return isBuilt;
    }

    public int GetLevel() // Get current level of the tower
    {
        return currentLevel;
    }

    public float GetAttackDelay() // Get current attack delay based on level
    {
        return attackDelay;
    }

    private void OnDrawGizmosSelected()
    {
        if (!isBuilt) return; // Only draw if tower is built

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Draw detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Draw attack range
    }
    private void ShowBuyPopup()
    {
        buy.SetActive(true);
        bText.text = "Press E to Buy Tower (" + towerCost + " Souls)";
    }

    private void HideBuyPopup()
    {
        buy.SetActive(false);
    }

    private void ShowUpgradePopup()
    {
        upgrade.SetActive(true);
        gText.text = "Press E to Upgrade Tower (" + upgradeCost + " Souls)";
    }

    private void HideUpgradePopup()
    {
        upgrade.SetActive(false);
    }

    private void ShowInsufficientPopup()
    {
        insufficient.SetActive(true);
        iText.text = "Not enough souls!";
    }

    private void HideInsufficientPopup()
    {
        insufficient.SetActive(false);
    }

    private void ShowMaximumPopup()
    {
        maximum.SetActive(true);
        mText.text = "Tower is at MAX level!";
    }

    private void HideMaximumPopup()
    {
        maximum.SetActive(false);
    }
    private void HideAllPopups()
    {
        HideBuyPopup();
        HideUpgradePopup();
        HideInsufficientPopup();
        HideMaximumPopup();
    }
    private IEnumerator ShowInsufficientForSeconds(float sec)
    {
        showingInsufficient = true;
        HideAllPopups();
        ShowInsufficientPopup();
        yield return new WaitForSeconds(sec);
        HideInsufficientPopup();
        showingInsufficient = false;
    }
    private void UpdateTowerVisual()
    {
        if (levelVisuals == null || levelVisuals.Length == 0)
            return;

        for (int i = 0; i < levelVisuals.Length; i++)
            if (levelVisuals[i] != null)
                levelVisuals[i].SetActive(false);

        int index = Mathf.Clamp(currentLevel, 0, levelVisuals.Length - 1);

        if (levelVisuals[index] != null)
            levelVisuals[index].SetActive(true);

        Debug.Log("Tower visual update called, current level: " + currentLevel);
    }
    private void LockVisualRotation()
    {
        if (levelVisuals == null) return;

        for (int i = 0; i < levelVisuals.Length; i++)
        {
            if (levelVisuals[i] != null)
            {
                // reset rotation every frame so they ignore parent rotation
                levelVisuals[i].transform.rotation = Quaternion.identity;
            }
        }
    }
}