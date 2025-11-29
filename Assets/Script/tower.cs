using UnityEngine;

public class tower : Shooter
{
    private bool isBuilt = false; // Tower starts as empty/unbuilt
    public int currentLevel = 1; // Tower level (1, 2, or 3)
    private float[] attackDelaysByLevel = { 1.0f, 0.5f, 0.33f }; // Level 1 = slowest, Level 2 = medium, Level 3 = fastest
    private float attackDelay = 1.0f;

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
    public int towerCost = 100; // Cost to buy tower
    public int upgradeCost = 80; // Cost to upgrade tower per level

    void Start()
    {
        isBuilt = false; // Tower starts unbuilt/empty
        enabled = true; // Enable to check for player interaction even when not built
    }

    void Update()
    {
        CheckPlayerInRange(); // Check if player is within interaction range

        /*if (!isBuilt) // If not built, check for purchase
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                if (nearbyPlayer != null)
                {
                    int playerSouls = nearbyPlayer.playerSouls;
                    if (playerSouls >= towerCost)
                    {
                        if (BuyTower(towerCost))
                        {
                            nearbyPlayer.playerSouls -= towerCost;
                        }
                    }
                }
            }
            return;
        }*/
        if (!isBuilt) // If not built, check for purchase
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.E)) //without souls yet
            {
                BuyTower(towerCost);
            }
        }

        // Check for upgrade when tower is built
        /*if (isBuilt && playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (nearbyPlayer != null)
            {
                int playerSouls = nearbyPlayer.playerSouls;
                if (playerSouls >= upgradeCost && currentLevel < 3)
                {
                    if (UpgradeLevel())
                    {
                        nearbyPlayer.playerSouls -= upgradeCost;
                    }
                }
            }
        }*/
        if (isBuilt && playerInRange && Input.GetKeyDown(KeyCode.E)) //without souls yet
        {
            UpgradeLevel();
        }

        //attack logic
        enemyInDetectionRange = Physics.CheckSphere(transform.position, detectionRange, whatIsEnemy);
        enemyInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsEnemy);

        if (isBuilt && (enemyInDetectionRange || enemyInAttackRange))
        {
            FindNearestEnemy();
            
            if (currentTarget != null)
            {
                RotateTowardTarget();
                if (enemyInAttackRange) Attack();
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
            shoot(0); // Use pattern 0 for normal bullets
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

    public bool BuyTower(int soulCost) // Buy/initialize the tower when player interacts with empty tower spot
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
}