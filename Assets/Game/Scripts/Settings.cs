using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour {
    
    //display
    [SerializeField] private Dropdown resolutionsDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    
    //audio
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private AudioMixer musicAudioMixer;
    
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private AudioMixer sfxAudioMixer;
    
    //controls
    [SerializeField] private Slider xCamSensitivitySlider;
    [SerializeField] private Slider yCamSensitivitySlider;
    
    private Resolution[] resolutions;
    
    private CinemachineFreeLook freeLookCamera;
    float defaultCameraSensitivity = 0.25f;

    private void Awake(){
        freeLookCamera = FindObjectOfType<CinemachineFreeLook>();
        
        InitializeResolutions();
        LoadOptions();
    }

    private void Start(){
        LoadAudio();
    }
    
    private void InitializeResolutions(){
        resolutions = Screen.resolutions;
        
        resolutionsDropdown.ClearOptions();

        List<string> resOptions = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++){
            string resOption = resolutions[i].width + " x " + resolutions[i].height;
            resOptions.Add(resOption);

            Resolution cr = Screen.currentResolution;
            if(resolutions[i].width == cr.width && resolutions[i].height == cr.height)
                currentResolutionIndex = i;
        }
        
        resolutionsDropdown.AddOptions(resOptions);

        int savedResolution = PlayerPrefs.GetInt("resolution", -1);
        if(savedResolution != -1){
            currentResolutionIndex = savedResolution;
            SetResolution(savedResolution);
        }
        
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
    }
    
    private void LoadOptions(){
        //fullscreen
        bool fullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        Screen.fullScreen = fullscreen;
        fullscreenToggle.isOn = fullscreen;
        
        //camera sensitivity
        float xCamSensitivity = PlayerPrefs.GetFloat("cam sensitivity x", defaultCameraSensitivity);
        xCamSensitivitySlider.value = xCamSensitivity;
            
        if(freeLookCamera != null)
            freeLookCamera.m_XAxis.m_MaxSpeed = xCamSensitivity * (300f / defaultCameraSensitivity);
            
        float yCamSensitivity = PlayerPrefs.GetFloat("cam sensitivity y", defaultCameraSensitivity);
        yCamSensitivitySlider.value = yCamSensitivity;
            
        if(freeLookCamera != null)
            freeLookCamera.m_YAxis.m_MaxSpeed = yCamSensitivity * (4f / defaultCameraSensitivity);
    }
    
    private void LoadAudio(){
        //music volume
        float musicVolume = PlayerPrefs.GetFloat("music volume", 0.6f);
        musicVolumeSlider.value = musicVolume;
        
        musicAudioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        
        //sfx volume
        float sfxVolume = PlayerPrefs.GetFloat("sfx volume", 1f);
        sfxVolumeSlider.value = sfxVolume;
        
        sfxAudioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
    }
    
    public void SetResolution(int resolutionIndex){
        Resolution res = resolutionIndex < resolutions.Length ? resolutions[resolutionIndex] : resolutions[0];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        
        PlayerPrefs.SetInt("resolution", resolutionIndex);
    }

    public void SetFullscreen(bool fullscreen){
        Screen.fullScreen = fullscreen;
        
        PlayerPrefs.SetInt("fullscreen", fullscreen ? 1 : 0);
    }
    
    public void SetMusicAudio(float value){
        musicAudioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20); 
        PlayerPrefs.SetFloat("music volume", value);
    }
    public void SetSfxAudio(float value){
        sfxAudioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20); 
        PlayerPrefs.SetFloat("sfx volume", value);
    }
    
    public void SetCameraSensitivityX(float x){
        if(freeLookCamera != null)
            freeLookCamera.m_XAxis.m_MaxSpeed = x * (300 / defaultCameraSensitivity);
        
        PlayerPrefs.SetFloat("cam sensitivity x", x);
    }
    
    public void SetCameraSensitivityY(float y){
        if(freeLookCamera != null)
            freeLookCamera.m_YAxis.m_MaxSpeed = y * (4 / defaultCameraSensitivity);
        
        PlayerPrefs.SetFloat("cam sensitivity y", y);
    }
}
