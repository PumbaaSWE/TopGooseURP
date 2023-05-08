using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioOptions : MonoBehaviour
{
    private static float masterVolume = 1;
    private static float effectsVolume = 1;
    private static float musicVolume = 1;
    private static readonly string masterString = "MasterVolumePref";
    private static readonly string effectsString = "EffectsVolumePref";
    private static readonly string musicString = "MusicVolumePref";
    private static readonly string firstPlay = "FirstPlay";
    private int firstPlayInt;
    private bool InGame = false;

    [Header("Percent text")]
    [SerializeField] private TextMeshProUGUI masterTxt;
    [SerializeField] private TextMeshProUGUI effectsTxt;
    [SerializeField] private TextMeshProUGUI musicTxt;

    [Space]
    [Header("Mixers")]
    [SerializeField] private AudioMixerGroup masterMixer;
    [SerializeField] private AudioMixerGroup effectsMixer;
    [SerializeField] private AudioMixerGroup musicMixer;

    [Space]
    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider effectsSlider;
    [SerializeField] private Slider musicSlider;


    [Space]
    [Header("Extra")]
    [SerializeField] private Button saveButton;
    [SerializeField] private GameObject save;

    public void OnMasterSliderChange(float value)
    {
        masterVolume = value;
        masterMixer.audioMixer.SetFloat("Master", Mathf.Log(value) * 20);
        masterTxt.text = Mathf.Round(value * 100).ToString() + "%";
    }
    public void OnEffectsSliderChange(float value)
    {
        effectsVolume = value;
        effectsMixer.audioMixer.SetFloat("Effects", Mathf.Log(value) * 20);
        effectsTxt.text = Mathf.Round(value * 100).ToString() + "%";
    }
    public void OnMusicSliderChange(float value)
    {
        musicVolume = value;
        musicMixer.audioMixer.SetFloat("Music", Mathf.Log(value) * 20);
        musicTxt.text = Mathf.Round(value * 100).ToString() + "%";
    }

    private void Awake()
    {
        if (InGame)
        {
            LoadSoundSettings();
        }
        saveButton.onClick.AddListener(() =>
        {
            SaveSoundSettings();
            PlayerPrefs.Save();
            StartCoroutine(ShowMessage(2));
            
        });
    }
    void Start()
    {
        firstPlayInt = PlayerPrefs.GetInt(firstPlay);

        if (firstPlayInt == 0)
        {
            DefaultSettings();
            SaveSoundSettings();
            PlayerPrefs.SetInt(firstPlay, -1);
            PlayerPrefs.Save();
            InGame = true;
        }
        else
        {
            LoadSoundSettings();
            InGame = true;
        }
    }

    private void DefaultSettings()
    {
        masterVolume = 1;
        effectsVolume = 1;
        musicVolume = 1;
        masterSlider.value = masterVolume;
        effectsSlider.value = effectsVolume;
        musicSlider.value = musicVolume;
        masterMixer.audioMixer.SetFloat("Music", Mathf.Log(masterVolume) * 20);
        effectsMixer.audioMixer.SetFloat("Music", Mathf.Log(effectsVolume) * 20);
        musicMixer.audioMixer.SetFloat("Music", Mathf.Log(musicVolume) * 20);
    }
    private void SaveSoundSettings()
    {
        PlayerPrefs.SetFloat(masterString, masterVolume);
        PlayerPrefs.SetFloat(effectsString, effectsVolume);
        PlayerPrefs.SetFloat(musicString, musicVolume);
    }
    private void LoadSoundSettings()
    {
        masterVolume = PlayerPrefs.GetFloat(masterString);
        masterSlider.value = masterVolume;
        masterMixer.audioMixer.SetFloat("Music", Mathf.Log(masterVolume) * 20);

        effectsVolume = PlayerPrefs.GetFloat(effectsString);
        effectsSlider.value = effectsVolume;
        effectsMixer.audioMixer.SetFloat("Music", Mathf.Log(effectsVolume) * 20);

        musicVolume = PlayerPrefs.GetFloat(musicString);
        musicSlider.value = musicVolume;
        musicMixer.audioMixer.SetFloat("Music", Mathf.Log(musicVolume) * 20);
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveSoundSettings();
            PlayerPrefs.Save();
        }
    }
    IEnumerator ShowMessage(float delay)
    {
        save.SetActive(true);
        yield return new WaitForSeconds(delay);
        save.SetActive(false);
    }
}
