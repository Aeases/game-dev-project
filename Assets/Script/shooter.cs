

using System;
using System.Collections;
using UnityEditor.UI;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    public float health = 100f;
    public GameObject currentBulletPrefab;
    protected int shootPattern = 0;
    public float electricCooldown = 0.5f;   // Cooldown (avoid spamming)       
    private float electricCooldownTimer = 0f;



    public void shoot(int pattern)
    {
        bool amEnemy = CompareTag("Enemy");
        GameObject spawnedBullet = null;


        // ShootPattern: 0 for normal, 1 for fire, 2 for water, 3 for grass, 4 for electricity
        switch (pattern)
        {
            case 0:
                spawnedBullet = Instantiate(currentBulletPrefab, transform.position, transform.rotation);
                spawnedBullet.GetComponent<bullet>().isFriendly = !amEnemy;
                break;
            case 1:
                for (int i = 0; i < 4; i++)
                {
                    float offsetAngle = 45f + i * 90f;  // Angle: 45°, 135°, 225°, 315°
                    Quaternion bulletRot = transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
                    spawnedBullet = Instantiate(currentBulletPrefab, transform.position, bulletRot);
                    spawnedBullet.GetComponent<bullet>().isFriendly = !amEnemy;
                }
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    float offsetAngle = -45f + i * 90f;  // Angle: -45°, 45°
                    Quaternion bulletRot = transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
                    spawnedBullet = Instantiate(currentBulletPrefab, transform.position, bulletRot);
                    spawnedBullet.GetComponent<bullet>().isFriendly = !amEnemy;
                }
                break;
            case 3:
                for (int i = 0; i < 4; i++)
                {
                    float offsetAngle = i * 90f;  // Angle: 90 180 270 360
                    Quaternion bulletRot = transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
                    spawnedBullet = Instantiate(currentBulletPrefab, transform.position, bulletRot);
                    spawnedBullet.GetComponent<bullet>().isFriendly = !amEnemy;
                }
                break;
            case 4:
                StartCoroutine(SpawnElectricBurst(!amEnemy));
                return;
            default:
                Debug.LogError("No Shooting Pattern Assigned");
                return;
        }
    }
    private IEnumerator SpawnElectricBurst(bool isFriendly)
    {
        int numBullets = 2;
        float delay = 0.1f;

        for (int i = 0; i < numBullets; i++)
        {
            GameObject bullet = Instantiate(currentBulletPrefab, transform.position, transform.rotation);
            bullet.GetComponent<bullet>().isFriendly = isFriendly;
            yield return new WaitForSeconds(delay);
        }
    }
}

