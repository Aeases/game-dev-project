using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class Shop : MonoBehaviour
{
    private PlayerControl p;

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI healthRegenText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI speedText;

    private void Awake()
    {
        p = PlayerControl.Instance;
        if (p == null)
        {
            Debug.LogError("PlayerControl.Instance is missing");
        }
    }
    void Start()
    {
        p.coin = PlayerPrefs.GetInt("Coin", 1000);
        p.health = PlayerPrefs.GetFloat("Health", 100f);
        p.healthRegen = PlayerPrefs.GetFloat("HealthRegen", 0);
        p.attack = PlayerPrefs.GetInt("Attack", 10);
        p.speed = PlayerPrefs.GetInt("Speed", 5);

        UpdateUI();
    }

    

    public void buyHealth()
    {
        if (p.coin >= 100)
        {
            p.coin -= 100;
            coinText.text = p.coin.ToString();

            p.health += 10;
            healthText.text = p.health.ToString();
            SaveAll();
            UpdateUI();
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

            p.healthRegen += 0.2f;
            healthText.text = p.healthRegen.ToString();
            SaveAll();
            UpdateUI();
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
            SaveAll();
            UpdateUI();
        }
        else
        {
            print("Not enough gold");
        }
    }
    public void buySpeed()
    {
        if (p.coin >= 100)
        {
            p.coin -= 100;
            coinText.text = p.coin.ToString();

            p.speed += 3;
            speedText.text = p.speed.ToString();
            SaveAll();
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
        p.coin = 1000;
        p.health = 100f;
        p.healthRegen = 0f;
        p.attack = 10;
        p.speed = 5;
        SaveAll();
        UpdateUI();
    }

    public void UpdateUI()
    {
        coinText.text = p.coin.ToString();
        healthText.text = p.health.ToString();
        healthRegenText.text = p.healthRegen.ToString();
        attackText.text = p.attack.ToString();
        speedText.text = p.speed.ToString();
    }
    
    public void SaveAll()
    {
        PlayerPrefs.SetInt("Coin", p.coin);
        PlayerPrefs.SetFloat("Health", p.health);
        PlayerPrefs.SetFloat("HealthRegen", p.healthRegen);
        PlayerPrefs.SetInt("Attack", p.attack);
        PlayerPrefs.SetInt("Speed", p.speed);
        PlayerPrefs.Save();
    }

   

}
