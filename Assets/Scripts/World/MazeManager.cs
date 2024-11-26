using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeManager : MonoBehaviour, IInteractable
{
    [SerializeField] int gglowFragments = 0;
    [SerializeField, TextArea(1,5)] string earlyText = "You don't have enough orbs to leave.";
    [SerializeField, TextArea(1,5)] string winText = "You are amazeing!";
    [SerializeField, TextArea(1,5)] string fragmentText = "You got a gglow orb!";
    [SerializeField] TempUI tempUI;
    
    int _maxgGlowFragments = 7;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GetFragment(){
        gglowFragments++;
        tempUI.DisplayText(fragmentText);
        tempUI.SetUITextDisplay($"gglow Orbs:\n{gglowFragments}/{_maxgGlowFragments}");
    }
    
    public void Interact(){
        if(gglowFragments>=_maxgGlowFragments){
            tempUI.DisplayText(winText);
            StartCoroutine(EndGame());
        }
        else {
            tempUI.DisplayText(earlyText);
        }
    }
    
    IEnumerator EndGame(){
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
