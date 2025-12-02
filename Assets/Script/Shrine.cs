using UnityEngine;

public class Shrine : Shooter
{
    public HealthBar healthBar;
    public static Shrine Instance;

    private void Awake()
    {
        Instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        healthBar.setMaxHealth(maxHealth);
        healthBar.setHealth(currentHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
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
}
