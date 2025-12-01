using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using System.Collections;

public class PlayerControl : Shooter
{
    [Header("Reference")]
    public LayerMask groundLayer;
    public Camera mainCamera;
    [Header("Bullets")]
    public GameObject[] bulletPrefab = new GameObject[5];// 0 for normal, 1 for fire, 2 for water, 3 for elec, 4 for grass 
    [Header("Movement")]
    private Vector3 moveInput;
    [Header("Dash")]
    public float dashPower = 25f;
    public float dashDuration = 0.15f;
    public float dashCoolDown = 0.8f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float coolDownTimer = 0f;
    private Vector3 dashDirection;
    [Header("InteractionMsg")]
    public GameObject shop;
    public GameObject eat;
    [Header("PlayerStats")]
    public float healthRegen = 0f;
    public float healthRegenInterval = 2f; // Regen every 2s
    public static PlayerControl Instance;
    public int coin = 1000;
    public float maxHealth = 100f;
    private Coroutine healthRegenOverTime;
    private void Awake()
    {
        Instance = this;  
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {  
        currentBulletPrefab = Resources.Load<GameObject>(elementToBulletGameObject[currentElement]);

        maxHealth = 100f;
        health = maxHealth - 20;

        health = maxHealth;

        speed = 5;
        attack = 10;
        healthRegenOverTime = StartCoroutine(healthRegeneration());
    }

    // Update is called once per frame
    void Update()
    {
        if (OpenShop.isShopOpen)
        {
            return;
        }
        RotateToMouse();
        PlayerMovementAndDash();
        HandleShooting();
        mainCamera.transform.position = new Vector3(transform.position.x, mainCamera.transform.position.y, transform.position.z); // Camera follows player (top-down)
    }

    void PlayerMovementAndDash()
    {
        if (coolDownTimer > 0f) // Calculating CD
        {
            coolDownTimer -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && coolDownTimer <= 0)
        {
            Vector3 currentMovementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;




            if(currentMovementInput.x != 0 || currentMovementInput.z != 0) // Check is player is moving or not
            {
                dashDirection = currentMovementInput;  // If Y then dash to moving direction
            } else
            {
                dashDirection = transform.forward; // If N then dash to where player is facing at 
            }
            isDashing = true;
            dashTimer = dashDuration; 
            coolDownTimer = dashCoolDown; // Reset the CD Timer
        }
        if (isDashing)
        {
            Vector3 dashMovement = dashDirection * dashPower * Time.deltaTime;
            transform.Translate(dashMovement, Space.World);

            dashTimer -= Time.deltaTime; // Duration of dashing
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        } else
        {
            // Move in world space (XZ plane)
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            moveInput = new Vector3(x, 0, z).normalized;
            Vector3 move = moveInput * speed * Time.deltaTime;
            transform.Translate(move, Space.World);
        }
    }
    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            shoot();
        }
    }

    void RotateToMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 targetPosition = hit.point;

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shop"))
        {
            shop.gameObject.SetActive(true);
        }

  
        var bulletCol = other.GetComponent<bullet>();

        if (bulletCol != null)
        {
            if (bulletCol.isFriendly == false)
            {
                Destroy(other.gameObject);
                takeDamage(bulletCol);
            }
        }
    }

    private void OnTriggerStay(Collider other) // Eat to switch 
    {
        // bulletPrefab 0 for normal; 1 for fire; 2 for water, 3 for electricity, 4 for grass
        // Below for eating to switch
        if (other.CompareTag("FireSoul"))
        {
            eat.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                Destroy(other.gameObject);
                eat.gameObject.SetActive(false);
                changeElement(elementType.Fire);
                
            }
        }
        if (other.CompareTag("WaterSoul"))
        {
            eat.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                Destroy(other.gameObject);
                eat.gameObject.SetActive(false);
                changeElement(elementType.Water);
                
            }
        }
        if (other.CompareTag("ElecSoul"))
        {
            eat.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                Destroy(other.gameObject);
                eat.gameObject.SetActive(false);
                changeElement(elementType.Electric);
               
            }
        }
        if (other.CompareTag("GrassSoul"))
        {
            eat.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                Destroy(other.gameObject);
                eat.gameObject.SetActive(false);
                changeElement(elementType.Grass);
          
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        shop.gameObject.SetActive(false);
        eat.gameObject.SetActive(false);
    }

    private void changeElement(elementType element)
    {
        currentElement = element;
        currentBulletPrefab = Resources.Load<GameObject>(elementToBulletGameObject[element]);
        Debug.Log("Changed Element");
    }

    public void addCoin()
    {
        coin += 10;
    }

    private IEnumerator healthRegeneration()
    {
        WaitForSeconds wait = new WaitForSeconds(healthRegenInterval);
        while (true)
        {
            if (health <= maxHealth)
            {
                health += healthRegen;
                health = Mathf.Clamp(health, 0f, maxHealth);
            }
            yield return wait;
        }
    }

}
