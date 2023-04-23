using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [Header("Hud")]
    [SerializeField] Canvas HudUICanvas;
    [SerializeField] GameObject endScreenPanel;
    [SerializeField] GameObject pauseScreenPanel;

    [Header("Button")]
    [SerializeField] Button mainmenuButton;
    [SerializeField] Button continueButton;
    [SerializeField] Button exitToMainMenuButton;
    [SerializeField] Button exitToDesktopButton;

    [Header("Scene Number")]
    [Tooltip("The scene index for Main Menu. Look in the build settings if you dont know it. Defautlt should be 0")]
    [SerializeField] int sceneNum = 0;
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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !endScreenShown)
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = !Cursor.visible;
        HudUICanvas.gameObject.SetActive(false);
        pauseScreenPanel.gameObject.SetActive(true);
    }

    private void UnPauseGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = !Cursor.visible;
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
