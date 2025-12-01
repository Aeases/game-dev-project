using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class Enemy : Shooter
{
    public GameObject damageTextPrefab;
    public DmgText dmgText;

    private NavMeshAgent _agent;
    private Transform _player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Vector3 normalscale = Vector3.one;
    public Vector3 enlargedscale = Vector3.one * 5f;
    public int enlargeframes = 50;
    public float intervalseconds = 1f;
    private Vector3 targetScale;
    // Rushing
    public Transform towerPoint;
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
        towerPoint = GameObject.Find("towerpoint").transform;
    } 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        transform.localScale = normalscale;
        StartCoroutine(EnlargeRoutine());
        base.Start(); // This sets health to max health, and loads initial element bullet
        soulType = Resources.Load<GameObject>(elementToSoulGameObject[currentElement]);
        waveController = GetComponentInParent<WaveController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
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
        _agent.SetDestination(towerPoint.position);

        Vector3 distanceToWalkPoint = transform.position - towerPoint.position;

        
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

    private IEnumerator EnlargeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalseconds);

            // Smoothly enlarge over 0.2 seconds
            yield return ScaleOverTime(enlargedscale, 0.2f);

            // Stay enlarged for 10 frames
            for (int i = 0; i < enlargeframes; i++)
            {
                yield return null;
            }

            // Smoothly shrink back over 0.2 seconds
            yield return ScaleOverTime(normalscale, 0.2f);
        }
    }

    private IEnumerator ScaleOverTime(Vector3 targetscale, float duration)
    {
        Vector3 initialscale = transform.localScale;
        float timer = 0f;

        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(initialscale, targetscale*2, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetscale*2;
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
