
using System.Collections;
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Boss : Enemy
    {
        public float bossShotCooldown = 5f;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            base.Start(); // This sets health to max health, and loads initial element bullet
            StartCoroutine(bossShoot());

        }




    private IEnumerator bossShoot()
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
    }


