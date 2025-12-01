using UnityEngine;

public class Shrine : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public HealthBar healthBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = 250f;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.setMaxHealth(maxHealth);
        healthBar.setHealth(currentHealth);
    }
}
