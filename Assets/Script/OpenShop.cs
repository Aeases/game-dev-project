using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OpenShop : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject gameWonUI;

    public enum MenuState
    {
        InShop,
        InPauseMenu,
        InWinMenu,
        UnPaused
    }

    public static MenuState currentState {get; private set; } = MenuState.UnPaused;

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
                    Debug.Log("pressed pasue");
                    currentState = MenuState.InPauseMenu;
                    openPauseMenu();
                }
                if (WaveController.gameWon == true)
                {
                    currentState = MenuState.InWinMenu;
                    openWinScreen();
                }
                break;
            case MenuState.InShop:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    currentState = MenuState.UnPaused;
                    closeShop();
                }
                break;
            case MenuState.InWinMenu:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    currentState = MenuState.UnPaused;
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
                    while (WaveController.gameWon == true)
                    {
                        Debug.Log("Busy waiting");
                    }
                    closeWinScreen();
                }
                break;
            case MenuState.InPauseMenu:
                if (Input.GetKeyDown(KeyCode.Escape))
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
    }

    public void closeShop()
    {
        shopUI.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    void openPauseMenu()
    {
        Time.timeScale = 0f;
    }
    void closePauseMenu()
    {
        Time.timeScale = 1f;
    }

    void openWinScreen()
    {
        // gameWonUI.SetActive(true);
        Time.timeScale = 0f;
    }

    void closeWinScreen()
    {
        // gameWonUI.SetActive(false);
        Time.timeScale = 1f;
    }

    
}
