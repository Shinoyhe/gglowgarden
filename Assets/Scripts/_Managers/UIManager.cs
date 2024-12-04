using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] GameObject PauseMenu;
    [SerializeField] TextMeshProUGUI flowerText;
    // [SerializeField] GameObject keyboardControls;
    // [SerializeField] GameObject gamepadControls;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    
    [Header("Flower Collectables")]
    [SerializeField] GameObject flowerPopup;
    [SerializeField, ReadOnly] int flowersCollected = 0;
    
    [Header("Temp Text")]
    [SerializeField] GameObject textDisplay;
    [SerializeField] TextMeshProUGUI tempText;
    [SerializeField] TextMeshProUGUI uiText;
    
    SoundManager soundManager => SoundManager.Instance;
    CoreInput core;
    string flowerString = "Flowers Collected: ";
    
    // Start is called before the first frame update
    void Awake()
    {
        core = GameObject.FindWithTag("Player").GetComponent<Player>().action;
        
        masterSlider.onValueChanged.AddListener(ChangeMasterVolume);
        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
        
        Resume();
    }

    // Update is called once per frame
    void Update()
    {
        if ((core.UI.Pause.triggered || core.UI.Cancel.triggered) && Time.timeScale == 0f){
            Resume();
        }
        else if (core.Player.Pause.triggered){
            Pause();
        }
    }
    
    private void OnDestroy() {
        masterSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
    }
    
    void ChangeMasterVolume(float volume){
        soundManager.masterVolume = volume;
    }
    
    void ChangeMusicVolume(float volume){
        soundManager.musicVolume = volume;
    }
    
    void ChangeSFXVolume(float volume){
        soundManager.sfxVolume = volume;
    }
    
    void Pause(){
        PauseMenu.SetActive(true);
        flowerText.text = flowerString+flowersCollected;
        Stop();
    }
    
    void Stop(){
        Time.timeScale = 0f;
        core.Player.Disable();
        core.UI.Enable();
    }
    
    public void Resume(){
        Time.timeScale = 1f;
        core.UI.Disable();
        core.Player.Enable();
        CloseUI();
    }
    
    void CloseUI(){
        CloseText();
        PauseMenu.SetActive(false);
    }
    
    public void DisplayText(string text){
        Stop();
        textDisplay.SetActive(true);
        tempText.text = text;
    }
    
    public void CloseText(){
        textDisplay.SetActive(false);
    }
    
    public void SetUITextDisplay(string text){
        uiText.text = text;
    }
    
    public void AddFlower(){
        Instantiate(flowerPopup);
        flowersCollected++;
    }
}
