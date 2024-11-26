using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TempUI : MonoBehaviour
{
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
        if (core.UI.Submit.triggered && Time.timeScale == 0f){
            CloseText();
        }
    }
    
    void Pause(){
        Time.timeScale = 0f;
        core.Player.Disable();
        core.UI.Enable();
    }
    
    void Unpause(){
        Time.timeScale = 1f;
        core.UI.Disable();
        core.Player.Enable();
    }
    
    public void DisplayText(string text){
        Pause();
        textDisplay.SetActive(true);
        tempText.text = text;
    }
    
    public void CloseText(){
        Unpause();
        textDisplay.SetActive(false);
    }
    
    public void SetUITextDisplay(string text){
        tempUIText.text = text;
    }
    
    public void AddSphere(){
        
    }
}
