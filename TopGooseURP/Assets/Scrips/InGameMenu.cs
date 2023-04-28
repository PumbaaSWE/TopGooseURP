using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [Header("Hud")]
    [SerializeField] private Canvas HudUICanvas;
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private GameObject pauseScreenPanel;

    [Header("Button")]
    [SerializeField] private Button mainmenuButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitToMainMenuButton;
    [SerializeField] private Button exitToDesktopButton;

    [Header("Scene Number")]
    [Tooltip("The scene index for Main Menu. Look in the build settings if you dont know it. Defautlt should be 0")]
    [SerializeField] private int sceneNum = 0;

    [Header("GameInput")]
    [SerializeField] private GameInput gameInput;

    bool endScreenShown = false;
    private void Awake()
    {
        endScreenShown = false;
        Time.timeScale = 1f;
        mainmenuButton.onClick.AddListener(() =>
        {
            ChangeScene(sceneNum);
        });
        continueButton.onClick.AddListener(() =>
        {
            UnPauseGame();
        });
        exitToMainMenuButton.onClick.AddListener(() =>
        {
            ChangeScene(sceneNum);
        });
        exitToDesktopButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    private void ChangeScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = !Cursor.visible;
    }

    private void Start()
    {
        gameInput.InGameMenuAction += GameInput_InGameMenuAction;
    }

    private void GameInput_InGameMenuAction(object sender, System.EventArgs e)
    {
        PauseGame();
    }

    private void PauseGame()
    {
        if (!endScreenShown)
        {
            gameInput.GamePause();
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            HudUICanvas.gameObject.SetActive(false);
            pauseScreenPanel.gameObject.SetActive(true);
        }
    }

    private void UnPauseGame()
    {
        gameInput.GameUnPause();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseScreenPanel.gameObject.SetActive(false);
        HudUICanvas.gameObject.SetActive(true);
        Time.timeScale = 1f;
    }

    public void EndScreen()
    {
        endScreenShown = true;
        HudUICanvas.gameObject.SetActive(false);
        pauseScreenPanel.gameObject.SetActive(false);
        endScreenPanel.gameObject.SetActive(true);
    }
}
