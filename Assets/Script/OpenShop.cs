using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OpenShop : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject pauseUI;

    public enum MenuState
    {
        InShop,
        InPauseMenu,
        InWinMenu,
        UnPaused
    }

    public static MenuState currentState {get; private set; } = MenuState.UnPaused;

    public GameObject shopPressE;
    public static bool isShopOpen { get; private set; } = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = MenuState.UnPaused;
    }

    // Update is called once per frame
    void Update()
    {   

        switch(currentState)
        {
            case MenuState.UnPaused:
                if (Input.GetKeyDown(KeyCode.E) && PlayerControl.Instance.shop.activeInHierarchy) {
                    currentState = MenuState.InShop;
                    openShop();
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    currentState = MenuState.InPauseMenu;
                    openPauseMenu();
                }
                break;
            case MenuState.InShop:
                if (Input.GetKey(KeyCode.Escape))
                {
                    currentState = MenuState.UnPaused;
                    closeShop();
                }
                break;
            case MenuState.InWinMenu:
                if (Input.GetKey(KeyCode.Escape))
                {
                    currentState = MenuState.UnPaused;
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
                }
                break;
            case MenuState.InPauseMenu:
                if (Input.GetKey(KeyCode.Escape))
                {
                    currentState = MenuState.UnPaused;
                    closePauseMenu();
                }
                break;



        }



    }
    void openShop()
    {
        shopUI.gameObject.SetActive(true);
        Time.timeScale = 0f;
        isShopOpen = true;
    }

    void openPauseMenu()
    {
        return;
    }
    void closePauseMenu()
    {
        return;
    }
    
    public void closeShop()
    {
        shopUI.gameObject.SetActive(false);
        Time.timeScale = 1f;
        isShopOpen = false;
    }
}
