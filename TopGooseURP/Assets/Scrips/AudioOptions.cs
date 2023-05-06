using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AudioOptions : MonoBehaviour
{
    private static float masterVolume = 1;
    private static float effectsVolume = 1;
    private static float musicVolume = 1;


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

    public void OnMasterSliderChange(float value)
    {
        masterVolume = value;
        masterMixer.audioMixer.SetFloat("Master", Mathf.Log(value)*20);
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

    void Awake()
    {
        masterSlider.value = masterVolume;
        effectsSlider.value = effectsVolume;
        musicSlider.value = musicVolume;
        masterMixer.audioMixer.SetFloat("Music", Mathf.Log(masterVolume) * 20);
        effectsMixer.audioMixer.SetFloat("Music", Mathf.Log(effectsVolume) * 20);
        musicMixer.audioMixer.SetFloat("Music", Mathf.Log(musicVolume) * 20);
    }    
}
