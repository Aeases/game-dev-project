using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class PlayerControl : Shooter
{
    [Header("Reference")]
    public LayerMask groundLayer;
    public Camera mainCamera;
    [Header("Bullets")]
    public GameObject[] bulletPrefab = new GameObject[5];// 0 for normal, 1 for fire, 2 for water, 3 for elec, 4 for grass 
    [Header("Movement")]
    public float speed = 5f;
    private Vector3 moveInput;
    [Header("Dash")]
    public float dashPower = 25f;
    public float dashDuration = 0.15f;
    public float dashCoolDown = 0.8f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float coolDownTimer = 0f;
    private Vector3 dashDirection;
    [Header("Shop")]
    public GameObject pressE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
            shoot(shootPattern);
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
        if (other.CompareTag("Shop")){
            pressE.gameObject.SetActive(true);
        }
        // bulletPrefab 0 for normal; 1 for fire; 2 for water
        if (other.CompareTag("FirePlat"))
        {
            currentBulletPrefab = bulletPrefab[1];
            shootPattern = 1;
            Debug.Log("Changed to Fire");
        }
        else if (other.CompareTag("WaterPlat"))
        {
            currentBulletPrefab = bulletPrefab[2];
            shootPattern = 2;
            Debug.Log("Changed to Water");
        }
        else if (other.CompareTag("ElectricityPlat"))
        {
            currentBulletPrefab = bulletPrefab[3];
            shootPattern = 4;
            Debug.Log("Changed to Elec");
        }
        else if (other.CompareTag("GrassPlat"))
        {
            currentBulletPrefab = bulletPrefab[4];
            shootPattern = 3;
            Debug.Log("Changed to Grass");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        pressE.gameObject.SetActive(false);
    }
}
