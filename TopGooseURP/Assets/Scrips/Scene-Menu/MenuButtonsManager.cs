using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonsManager : MonoBehaviour
{
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private Button backButton;
    [SerializeField] private Toggle hotkeysToggle;
    [SerializeField] private Toggle soundToggle;

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
            hotkeysToggle.Select();
            Settings.SetActive(true);
        });
        backButton.onClick.AddListener(() =>
        {
            Settings.SetActive(false);
            MainMenu.SetActive(true);            
        });
        soundToggle.onValueChanged.AddListener(delegate
        {
            HotkeysSettings.SetActive(false);
            SoundSettings.SetActive(true);
        });
        hotkeysToggle.onValueChanged.AddListener(delegate
        {
            SoundSettings.SetActive(false);
            HotkeysSettings.SetActive(true);
        });
    }
}
