using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoPlay : MonoBehaviour
{
    [SerializeField] private Toggle skipToggle, overideToggle;
    [SerializeField] private LevelLoader levelLoader;

    private static readonly string AutoPlayString = "Auto Play";
    private int autoPlayInt;

    private void Start()
    {
        autoPlayInt = PlayerPrefs.GetInt(AutoPlayString);
        if (autoPlayInt == 1)
        {
            skipToggle.isOn = true;
            overideToggle.isOn = true;
            levelLoader.PressedPlay = true;
        }
        else
        {
            skipToggle.isOn = false;
            overideToggle.isOn = false;
            levelLoader.PressedPlay = false;
        }
    }
    public void AutoPlayCheckLoadScreen()
    {
        if (skipToggle.isOn)
        {
            PlayerPrefs.SetInt(AutoPlayString, 1);
            levelLoader.PressedPlay = true;
            overideToggle.isOn = true;
        }
        else if (!skipToggle.isOn)
        {
            PlayerPrefs.SetInt(AutoPlayString, 0);
            levelLoader.PressedPlay = false;
            overideToggle.isOn = false;
        }
        PlayerPrefs.Save();
    }
    public void AutoPlayCheckSetting()
    {
        if (overideToggle.isOn)
        {
            PlayerPrefs.SetInt(AutoPlayString, 1);
            levelLoader.PressedPlay = true;
            skipToggle.isOn = true;
        }
        else if (!overideToggle.isOn)
        {
            PlayerPrefs.SetInt(AutoPlayString, 0);
            levelLoader.PressedPlay = false;
            skipToggle.isOn = false;
        }
        PlayerPrefs.Save();
    }
    public void ButtonPlay()
    {
        levelLoader.PressedPlay = true;
    }
}
