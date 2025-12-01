using UnityEngine;
using TMPro;
using System.Collections;

public class tower : Shooter
{
    private bool isBuilt = false; // Tower starts as empty/unbuilt
    public int currentLevel = 1; // Tower level (1, 2, or 3)
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

    protected override void Start()
    {
        base.Start();
        isBuilt = false; // Tower starts unbuilt/empty
        enabled = true; // Enable to check for player interaction even when not built
        HideAllPopups();
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
                        isBuilt = true;
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
        Debug.Log("nearest enemy: " + nearestEnemy.name);
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
        Debug.Log("player in range: " + playerInRange);
    }

    public bool BuyTower() // Buy/initialize the tower when player interacts with empty tower spot
    {
        if (isBuilt) return false; // Already built

        isBuilt = true;
        currentLevel = 1;
        attackDelay = attackDelaysByLevel[0]; // Level 1 = slowest
        
        if (normalBulletPrefab != null) currentBulletPrefab = normalBulletPrefab;

        enabled = true; // Enable the tower
        Debug.Log("buy tower logic");
        return true;
    }

    public bool UpgradeLevel() // Upgrade tower to next level (1 -> 2 -> 3)
    {
        if (!isBuilt) return false; // Not built yet
        if (currentLevel >= 3) return false; // Already max level

        currentLevel++;
        attackDelay = attackDelaysByLevel[currentLevel - 1]; // Array is 0-indexed
        Debug.Log("upgrade logic");
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
}