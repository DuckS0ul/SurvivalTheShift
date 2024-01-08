using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using static SaveManager;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; set; }

    public Button backBTN;

    public Slider musicSlider;
    public GameObject musicValue;

    public Slider effectsSlider;
    public GameObject effectsValue;

    public Slider masterSlider;
    public GameObject masterValue;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }



    private void Start()
    {
        backBTN.onClick.AddListener(() =>
        {
            SaveManager.Instance.SaveVolumeSettings(musicSlider.value, effectsSlider.value, masterSlider.value);
        });

        StartCoroutine(LoadAndApplySettings());



    }


    private IEnumerator LoadAndApplySettings()
    {
        LoadAndSetVolume();
        yield return new WaitForSeconds(0.1f);
    }


    private void LoadAndSetVolume()
    {
        VolumeSettings volumeSettings = SaveManager.Instance.LoadVolumeSettings();
        if (volumeSettings != null)
        {
            musicSlider.value = volumeSettings.music;
            effectsSlider.value = volumeSettings.effects;
            masterSlider.value = volumeSettings.master;
            print("Volume Settings are Loaded");
        }
        else
        {
            print("Volume Settings could not be loaded.");
        }
       
    }



    private void Update()
    {
        musicValue.GetComponent<TextMeshProUGUI>().text = "" + (musicSlider.value) + "";
        effectsValue.GetComponent<TextMeshProUGUI>().text = "" + (effectsSlider.value) + "";
        masterValue.GetComponent<TextMeshProUGUI>().text = "" + (masterSlider.value) + "";
    }

}
