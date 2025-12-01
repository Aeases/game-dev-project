using UnityEngine;
using UnityEngine.InputSystem;

public class OpenShop : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;
    public GameObject shopPressE;
    public static bool isShopOpen { get; private set; } = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && PlayerControl.Instance.shop.activeInHierarchy) {
            openShop();
        }   



        if (Input.GetKey(KeyCode.Escape))
        {
            closeShop();
        }
    }
    void openShop()
    {
        shopUI.gameObject.SetActive(true);
        Time.timeScale = 0f;
        isShopOpen = true;
    }
    
    public void closeShop()
    {
        shopUI.gameObject.SetActive(false);
        Time.timeScale = 1f;
        isShopOpen = false;
    }
}
