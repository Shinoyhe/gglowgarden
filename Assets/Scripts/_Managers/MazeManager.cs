using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeManager : MonoBehaviour, IInteractable
{
    [Header("gglow Orb Fragments")]
    [SerializeField] List<GameObject> gglowOrbFragments;
    [SerializeField] List<GameObject> collectibleFlowers;
    [SerializeField, ReadOnly] int numgglowFragments = 0;
    [SerializeField] GameObject gglowOrb;
    
    [Header("Shrinking")]
    [SerializeField] float shrinkTime = 2;
    [SerializeField] Transform shrinkEnterLocationTransform;
    Vector3 shrinkEnterLocation => shrinkEnterLocationTransform.position;
    [SerializeField] GameObject mazeCamera;
    
    [Header("Popups")]
    [SerializeField] GameObject earlyPopup;
    [SerializeField] GameObject winPopup, gglowFragmentPopup;
    
    [Header("Squids")]
    [SerializeField] GameObject startSquid;
    [SerializeField] GameObject endSquid;
    
    int _maxgGlowFragments => gglowOrbFragments.Count;
    
    Player player;
    UIManager uiManager;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        DisableMaze();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GetFragment(){
        Instantiate(gglowFragmentPopup).GetComponent<ChatPopup>().popupLineToUse = numgglowFragments;
        numgglowFragments++;
        // uiManager.DisplayText(fragmentText);
        uiManager.SetUITextDisplay($"gglow Orbs:\n{numgglowFragments}/{_maxgGlowFragments}");
        SoundManager.Instance.PlaygglowOrbSFX();
    }
    
    public void EnterMaze(){
        StartCoroutine(StartMaze());
    }
    
    public void ExitMaze(){
        startSquid.SetActive(false);
        endSquid.SetActive(true);
        gglowOrb.SetActive(true);
        StartCoroutine(EndMaze());
    }
    
    public void Interact(){
        if(numgglowFragments>=_maxgGlowFragments){
            GameObject popup = Instantiate(winPopup);
            popup.GetComponent<MultiLinePopup>().callback.AddListener(ExitMaze);
        }
        else {
            Instantiate(earlyPopup);
        }
    }
    
    void DisableMaze(){
        gameObject.layer = 0;
        foreach(GameObject gO in gglowOrbFragments){
            gO.layer = 0;
        }
        foreach(GameObject gO in collectibleFlowers){
            gO.layer = 0;
        }
    }
    
    void EnableMaze(){
        gameObject.layer = 7;
        foreach(GameObject gO in gglowOrbFragments){
            gO.layer = 7;
        }
        foreach(GameObject gO in collectibleFlowers){
            gO.layer = 7;
        }
    }
    
    IEnumerator StartMaze(){
        player.action.Disable();
        player.ToggleMovement(false);
        player.tiny = true;
        mazeCamera.SetActive(true);
        Vector3 initialPosition = player.transform.position;
        Vector3 initialScale = player.transform.localScale;
        Vector3 scaleGoal = Vector3.one * player.tinyMult;
        float shrinkingTime = 0f;
        while (shrinkingTime < 1f){
            shrinkingTime += Time.deltaTime/shrinkTime;
            player.transform.localScale = Vector3.Lerp(initialScale, scaleGoal, shrinkingTime);
            player.transform.position = Vector3.Lerp(initialPosition, shrinkEnterLocation, shrinkingTime);
            yield return null;
        }
        uiManager.SetUITextDisplay($"gglow Orbs:\n{numgglowFragments}/{_maxgGlowFragments}");
        player.ToggleMovement(true);
        player.action.Enable();
        EnableMaze();
    }
    
    IEnumerator EndMaze(){
        player.action.Disable();
        player.ToggleMovement(false);
        DisableMaze();
        mazeCamera.SetActive(false);
        Vector3 initialPosition = player.transform.position;
        Vector3 posGoal = player.transform.position+Vector3.up;
        Vector3 initialScale = player.transform.localScale;
        Vector3 scaleGoal = Vector3.one;
        float shrinkingTime = 0f;
        while (shrinkingTime < 1f){
            shrinkingTime += Time.deltaTime/shrinkTime;
            player.transform.localScale = Vector3.Lerp(initialScale, scaleGoal, shrinkingTime);
            player.transform.position = Vector3.Lerp(initialPosition, posGoal, shrinkingTime);
            yield return null;
        }
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        player.tiny = false;
        uiManager.SetUITextDisplay("");
        player.ToggleMovement(true);
        player.action.Enable();
    }
}
