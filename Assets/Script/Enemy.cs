using JetBrains.Annotations;
using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Shooter
{
    public GameObject damageTextPrefab;
    public DmgText dmgText;

    private NavMeshAgent _agent;
    private Transform _player;
    public LayerMask whatIsGround, whatIsPlayer;


    // Rushing
    public Vector3 towerPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float attackDelay;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private bool alreadyAttacked;

    public string[] elementTypes = new string[4];
    public string currentElement;

    private WaveController waveController = null;

    private void Awake()
    {
        _player = GameObject.Find("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
    } 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waveController = GetComponentInParent<WaveController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            if (waveController != null)
            {
                waveController.EnemyKilled(); // Decrements remainingEnemies by one
            }
        }
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Rushing();
        if (playerInSightRange && !playerInAttackRange) Chase();
        if (playerInSightRange && playerInAttackRange) Attack();

        
    }

    private void Rushing()
    {
        _agent.SetDestination(towerPoint);

        Vector3 distanceToWalkPoint = transform.position - towerPoint;

        
    }

    private void Attack()
    {
        transform.LookAt(_player);

        if (!alreadyAttacked)
        {
            
            shoot(1);
            
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), attackDelay);
        }
    }

    private void Chase()
    {
        _agent.SetDestination(_player.position);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


    private void OnTriggerEnter(Collider other)
    {
  
        var bulletCol = other.GetComponent<bullet>();

        if (bulletCol != null)
        {
            if (bulletCol.isFriendly == true)
            {
                Destroy(other.gameObject);
                float damage = 10f;
                switch (currentElement) // Elemental Reaction
                {
                    case "Fire":
                        if (other.gameObject.CompareTag("WaterBullet"))
                        {
                            damage = 15f;
                        }
                        if (other.gameObject.CompareTag("GrassBullet"))
                        {
                            damage = 5f;
                        }
                        break;
                    case "Water":
                        if (other.gameObject.CompareTag("ElectricityBullet"))
                        {
                            damage = 15f;
                        }
                        if (other.gameObject.CompareTag("FireBullet"))
                        {
                            damage = 5f;
                        }
                        break;
                    case "Grass":
                        if (other.gameObject.CompareTag("FireBullet"))
                        {
                            damage = 15f;
                        }
                        if (other.gameObject.CompareTag("ElectricityBullet"))
                        {
                            damage = 5f;
                        }
                        break;
                    case "Electricity":
                        if (other.gameObject.CompareTag("GrassBullet"))
                        {
                            damage = 15f;
                        }
                        if (other.gameObject.CompareTag("WaterBullet"))
                        {
                            damage = 5f;
                        }
                        break;
                }
                health -= damage;
                }
        }
    }

}
