using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonsManager : MonoBehaviour
{
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private Button backButton;
    [SerializeField] private Button hotkeysButton;
    [SerializeField] private Button soundButton;

    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject Settings;
    [SerializeField] private GameObject SoundSettings;
    [SerializeField] private GameObject HotkeysSettings;

    private void Awake()
    {
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        settingButton.onClick.AddListener(() =>
        {
            MainMenu.SetActive(false);
            HotkeysSettings.SetActive(false);
            SoundSettings.SetActive(true);
            Settings.SetActive(true);

        });
        backButton.onClick.AddListener(() =>
        {
            Settings.SetActive(false);
            MainMenu.SetActive(true);            
        });
        soundButton.onClick.AddListener(() =>
        {
            HotkeysSettings.SetActive(false);
            SoundSettings.SetActive(true);
        });
        hotkeysButton.onClick.AddListener(() =>
        {
            SoundSettings.SetActive(false);
            HotkeysSettings.SetActive(true);
        });
    }
}
