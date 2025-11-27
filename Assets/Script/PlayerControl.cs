using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using System.Collections.Generic;

public class PlayerControl : Shooter
{
    public LayerMask groundLayer;
    public Camera mainCamera;
    public GameObject[] bulletPrefab = new GameObject[5];// 0 for fire, 1 for water, 2 for elec, 3 for grass 
    public float speed = 5f;
    private Vector3 moveInput;
    private int shootPattern = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RotateToMouse();
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        moveInput = new Vector3(x, 0, z).normalized;

        mainCamera.transform.position = new Vector3(transform.position.x, mainCamera.transform.position.y, transform.position.z); 
        

        // Move in world space (XZ plane)
        Vector3 move = moveInput * speed * Time.deltaTime;
        transform.Translate(move, Space.World);
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
}
