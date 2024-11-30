using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject keyboardControls;
    [SerializeField] GameObject gamepadControls;
    
    [Header("Temp Text")]
    [SerializeField] GameObject textDisplay;
    [SerializeField] TextMeshProUGUI tempText;
    [SerializeField] TextMeshProUGUI tempUIText;
    
    CoreInput core;
    
    // Start is called before the first frame update
    void Start()
    {
        core = GameObject.FindWithTag("Player").GetComponent<Player>().action;
    }

    // Update is called once per frame
    void Update()
    {
        if ((core.UI.Pause.triggered || core.UI.Cancel.triggered || core.UI.Submit.triggered) && Time.timeScale == 0f){
            Resume();
        }
        else if (core.Player.Pause.triggered){
            Pause();
        }
    }
    
    void Pause(){
        PauseMenu.SetActive(true);
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
        tempUIText.text = text;
    }
    
    public void AddSphere(){
        
    }
}
