

using UnityEngine;

public class Shooter : MonoBehaviour
{

    public float health = 100f;
    public GameObject currentBulletPrefab;


    public void shoot(int pattern)
    {

        // ShootPattern 1 for fire, 2 for water, 3 for
        switch (pattern)
        {
            case 0:
                Instantiate(currentBulletPrefab, transform.position, transform.rotation);
                break;
            case 1:
                for (int i = 0; i < 4; i++)
                {
                    float offsetAngle = 45f + i * 90f;  // Angle: 45°, 135°, 225°, 315°
                    Quaternion bulletRot = transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
                    Instantiate(currentBulletPrefab, transform.position, bulletRot);
                }
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    float offsetAngle = -45f + i * 90f;  // Angle: -45°, 45°
                    Quaternion bulletRot = transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
                    Instantiate(currentBulletPrefab, transform.position, bulletRot);
                }
                break;
            case 3:
                for (int i = 0; i < 4; i++)
                {
                    float offsetAngle = i * 90f;  // Angle: 90 180 270 360
                    Quaternion bulletRot = transform.rotation * Quaternion.Euler(0f, offsetAngle, 0f);
                    Instantiate(currentBulletPrefab, transform.position, bulletRot);
                }
                break;
            case 4:
                int numBullets = 3;
                float spacing = 5f;  // Distance between each bullet in the chain (adjust as needed)
                Vector3 forward = transform.forward;
                for (int i = 0; i < numBullets; i++)
                {
                    Vector3 spawnPos = transform.position + forward * (i * spacing);
                    Instantiate(currentBulletPrefab, spawnPos, transform.rotation);
                }
                break;

        }

    }


}