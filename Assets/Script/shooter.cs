

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public float maxHealth = 100f;
    public int speed = 5;
    public int attack = 10;
    
    public elementType currentElement;
    public float electricCooldown = 0.5f;   // Cooldown (avoid spamming)       
    private float electricCooldownTimer = 0f;
    public float currentHealth;


    protected GameObject currentBulletPrefab;
    protected static readonly Dictionary<elementType, string> elementToBulletGameObject = new Dictionary<elementType, string>
    {
        { elementType.Normal, "Bullets/Bullet" },
        { elementType.Fire, "Bullets/FireBullet" },
        { elementType.Water, "Bullets/WaterBullet" },
        { elementType.Grass, "Bullets/GrassBullet" },
        { elementType.Electric, "Bullets/ElectricityBullet" }
    };


    // Method Overload to just use current element if none is provided
    public void shoot()
    {
        shoot(currentElement);
    }

    protected virtual void Start()
    {
        currentBulletPrefab = Resources.Load<GameObject>(elementToBulletGameObject[currentElement]);
        currentHealth = maxHealth;
    }

    public void takeDamage(bullet collidingBullet)
    {
        float baseDamage = collidingBullet.baseDamage;
        float finalDamage = baseDamage;
        switch (currentElement) // Elemental Reaction
        {
            case elementType.Fire:
                if (collidingBullet.bulletElement == elementType.Water)
                {
                    finalDamage = baseDamage * 1.3f;
                }
                if (collidingBullet.bulletElement == elementType.Grass)
                {
                    finalDamage = baseDamage * 0.7f;
                }
                break;
            case elementType.Water:
                if (collidingBullet.bulletElement == elementType.Electric)
                {
                    finalDamage = baseDamage * 1.3f;
                }
                if (collidingBullet.bulletElement == elementType.Fire)
                {
                    finalDamage = baseDamage * 0.7f;
                }
                break;
            case elementType.Grass:
                if (collidingBullet.bulletElement == elementType.Fire)
                {
                    finalDamage = baseDamage * 1.3f;
                }
                if (collidingBullet.bulletElement == elementType.Electric)
                {
                    finalDamage = baseDamage * 0.7f;
                }
                break;
            case elementType.Electric:
                if (collidingBullet.bulletElement == elementType.Grass)
                {
                    finalDamage = baseDamage * 1.3f;
                }
                if (collidingBullet.bulletElement == elementType.Water)
                {
                    finalDamage = baseDamage * 0.7f;
                }
                break;
        }
        currentHealth -= finalDamage;
    }

    public void shoot(elementType element)
    {
        bool amEnemy = CompareTag("Enemy");
        GameObject spawnedBullet;


        // ShootPattern: 0 for normal, 1 for fire, 2 for water, 3 for grass, 4 for electricity
        switch (element)
        {
            case elementType.Normal:
                spawnedBullet = Instantiate(currentBulletPrefab, transform.position, transform.rotation);
                spawnedBullet.GetComponent<bullet>().isFriendly = !amEnemy;
                spawnedBullet.GetComponent<bullet>().baseDamage = attack;
                break;
            case elementType.Fire:
                for (int i = 0; i < 4; i++)
                {
                    float offsetAngle = 45f + i * 90f;  // Angle: 45°, 135°, 225°, 315°
                    Quaternion bulletRot = transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
                    spawnedBullet = Instantiate(currentBulletPrefab, transform.position, bulletRot);
                    spawnedBullet.GetComponent<bullet>().isFriendly = !amEnemy;
                    spawnedBullet.GetComponent<bullet>().baseDamage = attack;
                }
                break;
            case elementType.Water:
                for (int i = 0; i < 2; i++)
                {
                    float offsetAngle = -45f + i * 90f;  // Angle: -45°, 45°
                    Quaternion bulletRot = transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
                    spawnedBullet = Instantiate(currentBulletPrefab, transform.position, bulletRot);
                    spawnedBullet.GetComponent<bullet>().isFriendly = !amEnemy;
                    spawnedBullet.GetComponent<bullet>().baseDamage = attack;
                }
                break;
            case elementType.Grass:
                for (int i = 0; i < 4; i++)
                {
                    float offsetAngle = i * 90f;  // Angle: 90 180 270 360
                    Quaternion bulletRot = transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
                    spawnedBullet = Instantiate(currentBulletPrefab, transform.position, bulletRot);
                    spawnedBullet.GetComponent<bullet>().isFriendly = !amEnemy;
                    spawnedBullet.GetComponent<bullet>().baseDamage = attack;
                }
                break;
            case elementType.Electric:
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
            bullet.GetComponent<bullet>().baseDamage = attack;
            yield return new WaitForSeconds(delay);
        }
    }

    public void bossShoot()
    {
        bool amEnemy = CompareTag("Enemy");
        GameObject spawnedBullet;
        for (int i = 0; i < 31; i++)
        {
            float offsetAngle = i * 12f;  
            Quaternion bulletRot = transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
            spawnedBullet = Instantiate(currentBulletPrefab, transform.position, bulletRot);
            spawnedBullet.GetComponent<bullet>().isFriendly = !amEnemy;
            spawnedBullet.GetComponent<bullet>().baseDamage = attack;
        }
    }
    
    /*public IEnumerator bossShoot2()
    {
        while (true)
        {
            bool amEnemy = CompareTag("Enemy");
            GameObject spawnedBullet;
            for (int i = 0; i < 31; i++)
            {
                float offsetAngle = i * 12f;
                Quaternion bulletRot = transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
                spawnedBullet = Instantiate(currentBulletPrefab, transform.position, bulletRot);
                spawnedBullet.GetComponent<bullet>().isFriendly = !amEnemy;
                spawnedBullet.GetComponent<bullet>().baseDamage = attack;
            }
            yield return new WaitForSeconds(5f);
        }
    }
    */
}

