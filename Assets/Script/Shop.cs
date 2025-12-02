using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class Shop : MonoBehaviour
{
    private PlayerControl p;
    private Shrine s;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI healthRegenText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI fireRateText;

    private void Awake()
    {
        p = PlayerControl.Instance;
        if (p == null)
        {
            Debug.LogError("PlayerControl.Instance is missing");
        }
        s = Shrine.Instance;
        if (s == null)
        {
            Debug.LogError("Shrine.Instance is missing");
        }
    }
    void Start()
    {
        p.coin = PlayerPrefs.GetInt("Coin", 1000);
        p.currentHealth = PlayerPrefs.GetFloat("Health", 100f);
        p.maxHealth = PlayerPrefs.GetFloat("maxHealth", 100f);
        p.healthRegen = PlayerPrefs.GetFloat("HealthRegen", 0);
        p.attack = PlayerPrefs.GetInt("Attack", 10);
        p.fireRate = PlayerPrefs.GetFloat("FireRate", 0.8f);

        UpdateUI();
    }

    

    public void buyHealth()
    {
        if (p.coin >= 100)
        {
            p.coin -= 100;
            coinText.text = p.coin.ToString();

            p.maxHealth += 10;
            p.currentHealth += 10;
            healthText.text = p.currentHealth.ToString();

        }
        else
        {
            print("Not enough gold");
        }
    }
    public void buyHealthRegen()
    {
        if (p.coin >= 100)
        {
            p.coin -= 100;
            coinText.text = p.coin.ToString();

            p.healthRegen += 5f;
            healthText.text = p.healthRegen.ToString();

  
        }
        else
        {
            print("Not enough gold");
        }
    }
    public void buyAttack()
    {
        if (p.coin >= 100)
        {
            p.coin -= 100;
            coinText.text = p.coin.ToString();

            p.attack += 2;
            attackText.text = p.attack.ToString();

        }
        else
        {
            print("Not enough gold");
        }
    }
    public void buyFireRate()
    {
        if (p.coin >= 100)
        {
            p.coin -= 100;
            coinText.text = p.coin.ToString();

            p.fireRate -= 0.10f;
            fireRateText.text = p.fireRate.ToString();
        }
        else
        {
            print("Not enough gold");
        }
    }

    public void repairTemple()
    {
        if (p.coin >= 100)
        {
            p.coin -= 100;
            coinText.text = p.coin.ToString();

            p.currentHealth += 100;


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

        SaveAll();
        UpdateUI();
    }

    public void ResetAllData()  //for testing 
    {
        PlayerPrefs.DeleteAll(); 
        p.coin = 1000;
        p.currentHealth = 100f;
        p.maxHealth = 100f;
        p.healthRegen = 0f;
        p.attack = 10;
        p.fireRate = 0.8f;
    }

    public void UpdateUI()
    {
        coinText.text = p.coin.ToString();
        healthText.text = p.maxHealth.ToString(); 
        healthRegenText.text = p.healthRegen.ToString();
        attackText.text = p.attack.ToString();
        fireRateText.text = p.fireRate.ToString();
    }
    
    public void SaveAll()
    {
        // PlayerPrefs.SetInt("Coin", p.coin);
        // PlayerPrefs.SetFloat("Health", p.currentHealth);
        // PlayerPrefs.SetFloat("maxHealth", p.maxHealth);
        // PlayerPrefs.SetFloat("HealthRegen", p.healthRegen);
        // PlayerPrefs.SetInt("Attack", p.attack);
        // PlayerPrefs.SetInt("Speed", p.speed);
        // PlayerPrefs.Save();
    }
}
