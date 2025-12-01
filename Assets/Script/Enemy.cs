using JetBrains.Annotations;
using NUnit.Framework;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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


    private WaveController waveController = null;
    public GameObject soulType;
    private void Awake()
    {
        _player = GameObject.Find("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
    } 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = 100f;
        attack = 5;
        speed = 4;
        currentElement = elementType.Normal;
        waveController = GetComponentInParent<WaveController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            die();
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
        Vector3 targetPos = _player.position;
        targetPos.y = transform.position.y; // lock y-axis 
        transform.LookAt(targetPos);

        if (!alreadyAttacked)
        {
            
            shoot();
            
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
                float baseDamage = PlayerControl.Instance.attack;
                float finalDamage = baseDamage;
                switch (currentElement) // Elemental Reaction
                {
                    case elementType.Fire:
                        if (other.gameObject.CompareTag("WaterBullet"))
                        {
                            finalDamage = baseDamage * 1.3f;
                        }
                        if (other.gameObject.CompareTag("GrassBullet"))
                        {
                            finalDamage = baseDamage * 0.7f;
                        }
                        break;
                    case elementType.Water:
                        if (other.gameObject.CompareTag("ElectricityBullet"))
                        {
                            finalDamage = baseDamage * 1.3f;
                        }
                        ;
                        if (other.gameObject.CompareTag("FireBullet"))
                        {
                            finalDamage = baseDamage * 0.7f;
                        }
                        break;
                    case elementType.Grass:
                        if (other.gameObject.CompareTag("FireBullet"))
                        {
                            finalDamage = baseDamage * 1.3f;
                        }
                        if (other.gameObject.CompareTag("ElectricityBullet"))
                        {
                            finalDamage = baseDamage * 0.7f;
                        }
                        break;
                    case elementType.Electric:
                        if (other.gameObject.CompareTag("GrassBullet"))
                        {
                            finalDamage = baseDamage * 1.3f;
                        }
                        if (other.gameObject.CompareTag("WaterBullet"))
                        {
                            finalDamage = baseDamage * 0.7f;
                        }
                        break;
                }
                health -= finalDamage;
                }
        }
    }

        private void die() //drop soul
        {
            Destroy(gameObject);
            PlayerControl.Instance.addCoin();
            Instantiate(soulType, transform.position - new Vector3(0, 1, 0), transform.rotation);
        }
}
