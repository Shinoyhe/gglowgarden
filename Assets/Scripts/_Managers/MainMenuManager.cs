using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] float endCameraTimeMult = 1.5f;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject mainMenuCamera;
    [SerializeField] GameObject startDialogue;
    
    float _defaultBlendTime;
    
    CoreInput core;
    CinemachineBrain brain;
    Animator animator;
    EventSystem system;
    
    // Start is called before the first frame update
    void Start()
    {
        core = GameObject.FindWithTag("Player").GetComponent<Player>().action;
        core.Player.Disable();
        core.UI.Enable();
        
        brain = Camera.main.GetComponent<CinemachineBrain>();
        _defaultBlendTime = brain.m_DefaultBlend.m_Time;
        
        animator = GetComponent<Animator>();
        
        system = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // private void OnDisable()
    // {
    //     core.Player.Disable();
    //     core.UI.Disable();
    // }
    
    public void StartGame(){
        StartCoroutine(EnterGame());
        // SceneManager.LoadScene(1);
    }
    
    IEnumerator EnterGame(){
        mainMenuCamera.SetActive(false);
        system.enabled = false;
        core.UI.Disable();
        float temp = brain.m_DefaultBlend.m_Time;
        animator.speed = (temp == 0) ? 0 : 1/temp;
        animator.SetTrigger("Fade In");
        yield return new WaitForSeconds(temp);
        system.enabled = true;
        core.Player.Enable();
        mainMenu.SetActive(false);
        Instantiate(startDialogue);
    }
    
    public void Quit(){
        Application.Quit();
    }
    
    public void MainMenu(){
        SceneManager.LoadScene(0);
    }
    
    public void ReturnToMainMenu(){
        StartCoroutine(ExitGame());
    }
    
    IEnumerator ExitGame(){
        brain.m_DefaultBlend.m_Time = _defaultBlendTime*endCameraTimeMult;
        mainMenuCamera.SetActive(true);
        system.enabled = false;
        core.Player.Disable();
        float temp = brain.m_DefaultBlend.m_Time;
        animator.speed = (temp == 0) ? 0 : 1/temp;
        mainMenu.SetActive(true);
        animator.SetTrigger("Fade Out");
        yield return new WaitForSeconds(temp);
        system.enabled = true;
        core.UI.Enable();
        SceneManager.LoadScene(0);
    }
}
