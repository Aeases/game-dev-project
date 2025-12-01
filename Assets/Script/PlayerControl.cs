using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class PlayerControl : Shooter
{
    [Header("Reference")]
    public LayerMask groundLayer;
    public Camera movementCamera; // for moving the camera, not needed in main map
    public Camera cam;
    [Header("Bullets")]
    [Header("Movement")]
    private Vector3 moveInput;
    [Header("Dash")]
    public float dashSpeed = 1f;
    public float dashTime = 1f;
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
    private CharacterController characterController;
    public float shrinkScale = 0.8f;
    public float shrinkSpeed = 10f;
    private Vector3 targetScale = Vector3.one;
    private Coroutine healthRegenOverTime;
    [Header("HealthBar")]
    public HealthBar healthBar;
    private void Awake()
    {
        Instance = this;  
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {  
        base.Start(); // This sets health to max health, and loads initial element bullet
        healthRegenOverTime = StartCoroutine(healthRegeneration());
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
        if (OpenShop.isShopOpen)
        {
            return;
        }
        RotateToMouse();
        HandleShooting();
        PlayerMovement();
        if (movementCamera != null)
        {
            movementCamera.transform.position = new Vector3(transform.position.x, movementCamera.transform.position.y, transform.position.z); // Camera follows player (top-down)
        }
        healthBar.setMaxHealth(maxHealth);
        healthBar.setHealth(currentHealth);
    }

void PlayerMovement()
{
    Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    characterController.Move(move * Time.deltaTime * speed);


    // Update cooldown timer
    if (coolDownTimer > 0)
    {
        coolDownTimer -= Time.deltaTime;
    }

    if (Input.GetKeyDown(KeyCode.LeftShift) && coolDownTimer <= 0)
    {
        coolDownTimer = dashCoolDown;
        dashTimer = dashCoolDown; // Reset dash timer
        StartCoroutine(Dash());
    }

    if (isDashing == true)
    {
        dashTimer -= Time.deltaTime; // Duration of dashing
        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }
}
    IEnumerator Dash()
    {
        {
            float startTime = Time.time;
            isDashing = true;
            // Vector3 dashDirection = transform.forward; (if we want to change it so it faces the mouse to dash)
            Vector3 currentMovementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            while (Time.time < startTime + dashTime)
            {
                characterController.Move(currentMovementInput * dashSpeed * Time.deltaTime);
                if (movementCamera != null)
                {
                    movementCamera.transform.position = new Vector3(transform.position.x, movementCamera.transform.position.y, transform.position.z);
                }
                yield return null;
            }
        }
    }


    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(ShootSequence());
        }

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * shrinkSpeed);
    }

    IEnumerator ShootSequence()
    {
        shoot();
        targetScale = Vector3.one * shrinkScale;
        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Player/Shoot");

        yield return new WaitForSeconds(0.3f);

        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Player/Walk");
        targetScale = Vector3.one;
    }

    void RotateToMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

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
            if (currentHealth <= maxHealth)
            {
                currentHealth += healthRegen;
                currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
            }
            yield return wait;
        }
    }

}
