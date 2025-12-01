using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
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

    private GameObject soulType;
    private static readonly Dictionary<elementType, string> elementToSoulGameObject = new Dictionary<elementType, string>
    {
        { elementType.Fire, "Souls/FireSoul" },
        { elementType.Water, "Souls/WaterSoul" },
        { elementType.Grass, "Souls/GrassSoul" },
        { elementType.Electric, "Souls/ElectricitySoul" }
    };

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private bool alreadyAttacked;


    private WaveController waveController = null;
    private void Awake()
    {
        _player = GameObject.Find("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
    } 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentBulletPrefab = Resources.Load<GameObject>(elementToBulletGameObject[currentElement]);
        health = 100f;
        attack = 5;
        speed = 4;
        soulType = Resources.Load<GameObject>(elementToSoulGameObject[currentElement]);
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
                takeDamage(bulletCol);
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
