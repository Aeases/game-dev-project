using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;



public class OpenShop : MonoBehaviour

{
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject gameWonUI;
    [SerializeField] private Animator Table;
    

    public enum MenuState
    {
        InShop,
        InPauseMenu,
        InWinMenu,
        UnPaused
    }

    public static MenuState currentState {get; set; } = MenuState.UnPaused;

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
                Time.timeScale = 1f;
                if (Input.GetKeyDown(KeyCode.E) && PlayerControl.Instance.shop.activeInHierarchy) {
                    openShop();
                } else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Debug.Log("pressed paused");
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
                Time.timeScale = 1f;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    closeShop();
                    break;
                }
                break;
            case MenuState.InWinMenu:
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    currentState = MenuState.UnPaused;
                    closeWinScreen();
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
                }
                break;
            case MenuState.InPauseMenu:
                Time.timeScale = 0f;
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
        currentState = MenuState.InShop;
        shopUI.gameObject.SetActive(true);
    }

    public void closeShop()
    {
        shopUI.gameObject.SetActive(false);
        currentState = MenuState.UnPaused;
    }

    void openPauseMenu()
    {
        pauseUI.gameObject.SetActive(true);
    }
    void closePauseMenu()
    {
        pauseUI.gameObject.SetActive(false);
    }

    public void pauseMenuResume()
    {
        currentState = MenuState.UnPaused;
        pauseUI.gameObject.SetActive(false);
    }

    void openWinScreen()
    {
        // gameWonUI.SetActive(true);
    }

    void closeWinScreen()
    {
        // gameWonUI.SetActive(false);
    }

    
}
