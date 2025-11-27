using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    public GameObject damageTextPrefab;
    public DmgText dmgText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        int damage = 10;
        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("FireBullet") || other.gameObject.CompareTag("GrassBullet") || other.gameObject.CompareTag("ElectricityBullet"))
        {
            Destroy(other.gameObject);
        }
        
        if (other.gameObject.CompareTag("WaterBullet"))
        {
            Destroy(other.gameObject);
            damage = 15;
        }
        health -= damage;
        dmgText.SetDamage(damage);
        Instantiate(damageTextPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
    }

}
