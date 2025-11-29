using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class Shop : MonoBehaviour
{
    public int coin = 1000;
    public int health = 100;
    public float healthRegen = 0f;
    public int attack = 10;
    public int speed = 5;

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI healthRegenText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI speedText;
    void Start()
    {
        coin = PlayerPrefs.GetInt("Coin", 1000);
        health = PlayerPrefs.GetInt("Health", 100);
        healthRegen = PlayerPrefs.GetFloat("HealthRegenText", 0f);
        attack = PlayerPrefs.GetInt("Attack", 10);
        speed = PlayerPrefs.GetInt("Speed", 5);

        UpdateUI();
    }

    public void buyHealth()
    {
        if (coin >= 100)
        {
            coin -= 100;
            coinText.text = coin.ToString();

            health += 10;
            healthText.text = health.ToString();
            UpdateUI();
        }
        else
        {
            print("Not enough gold");
        }
    }
    public void buyHealthRegen()
    {
        if (coin >= 100)
        {
            coin -= 100;
            coinText.text = coin.ToString();

            healthRegen += 0.2f;
            healthText.text = healthRegen.ToString();
            UpdateUI();
        }
        else
        {
            print("Not enough gold");
        }
    }
    public void buyAttack()
    {
        if (coin >= 100)
        {
            coin -= 100;
            coinText.text = coin.ToString();

            attack += 2;
            attackText.text = attack.ToString();
            UpdateUI();
        }
        else
        {
            print("Not enough gold");
        }
    }
    public void buySpeed()
    {
        if (coin >= 100)
        {
            coin -= 100;
            coinText.text = coin.ToString();

            speed += 3;
            speedText.text = speed.ToString();
            UpdateUI();
        }
        else
        {
            print("Not enough gold");
        }
    }
    void Update() 
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            ResetAllData(); // for testing 
        }
    }

    public void ResetAllData()  //for testing 
    {
        PlayerPrefs.DeleteAll(); 
        coin = 1000;
        health = 100;
        healthRegen = 0f;
        attack = 10;
        speed = 5;
        UpdateUI();
    }

    public void UpdateUI()
    {
        coinText.text = coin.ToString();
        healthText.text = health.ToString();
        healthRegenText.text = healthRegen.ToString();
        attackText.text = attack.ToString();
        speedText.text = speed.ToString();
    }
}
