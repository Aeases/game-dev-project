using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{
    public GameObject shootPos;
    public LayerMask groundLayer;
    public Camera mainCamera;
    public GameObject[] bulletPrefab = new GameObject[3];// 0 for normal; 1 for fire; 2 for water
    public float speed = 5f;
    private Vector3 moveInput;
    public GameObject currentBulletPrefab;
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

        // Move in world space (XZ plane)
        Vector3 move = moveInput * speed * Time.deltaTime;
        transform.Translate(move);
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
    void shoot(int pattern)
    {
        
        // ShootPattern 1 for fire, 2 for water, 3 for
        switch (pattern)
        {
            case 0:
                Instantiate(currentBulletPrefab, shootPos.transform.position, shootPos.transform.rotation);
                break;
            case 1:
                for (int i = 0; i < 4; i++)
                {
                    float offsetAngle = 45f + i * 90f;  // Angle: 45°, 135°, 225°, 315°
                    Quaternion bulletRot = shootPos.transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
                    Instantiate(currentBulletPrefab, shootPos.transform.position, bulletRot);
                }
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    float offsetAngle = 45f + i * 90f;  // Angle: 45°, 135°, 225°, 315°
                    Quaternion bulletRot = shootPos.transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
                    Instantiate(currentBulletPrefab, shootPos.transform.position, bulletRot);
                }
                break;
            case 3:
                for (int i = 0; i < 4; i++)
                {
                    float offsetAngle = i * 90f;  // Angle: 45°, 135°, 225°, 315°
                    Quaternion bulletRot = shootPos.transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
                    Instantiate(currentBulletPrefab, shootPos.transform.position, bulletRot);
                }
                break;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // bulletPrefab 0 for normal; 1 for fire; 2 for water
        if (other.CompareTag("FirePlat"))
        {
            currentBulletPrefab = bulletPrefab[1];
            shootPattern = 1;
            Debug.Log("Changed to fire");
        } else if (other.CompareTag("WaterPlat"))
        {
            currentBulletPrefab = bulletPrefab[2];
            shootPattern = 2;
            Debug.Log("Changed to water");
        }
    }


}
