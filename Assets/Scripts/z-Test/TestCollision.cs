using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestCollision : MonoBehaviour, IInteractable
{
    enum Tester {TELEPORTER, WALLBREAKER, DIALGOUER};
    [SerializeField] Tester type = Tester.WALLBREAKER;
    [SerializeField] List<WallBreaker> walls2Break;
    [SerializeField] GameObject testDialogue;
    [SerializeField] float dialogueWait = 3;
    [SerializeField, TextArea(1,5)] string dialogue = "Oho";
    [SerializeField] TempUI tempUI;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void Teleport(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
    
    void Dialogue(){
        tempUI.DisplayText(dialogue);
    }
    
    IEnumerator DisplayDialogue(){
        testDialogue.SetActive(true);
        yield return new WaitForSeconds(dialogueWait);
        testDialogue.SetActive(false);
    }
    
    public void Interact(){
        Dialogue();
        if (type != Tester.DIALGOUER){
            StartCoroutine(CheckAction());
        }
    }
    
    IEnumerator CheckAction(){
        yield return new WaitForSeconds(0.1f);
        switch (type)
        {
            case Tester.TELEPORTER:
                Teleport();
                break;
            case Tester.WALLBREAKER:
                foreach (var wall in walls2Break){
                    // Destroy(wall.gameObject);
                    wall.BreakWall();
                }
                walls2Break = new List<WallBreaker>();
                break;
        }
    }
    
    // private void OnTriggerEnter(Collider other) {
    //     if(other.tag == "Player"){
    //         switch (type)
    //         {
    //             case Tester.TELEPORTER:
    //                 Teleport();
    //                 Destroy(gameObject);
    //                 break;
    //             case Tester.DIALGOUER:
    //                 Dialogue();
    //                 break;
    //             default:
    //                 foreach (var wall in walls2Break){
    //                     // Destroy(wall.gameObject);
    //                     wall.BreakWall();
    //                 }
    //                 Destroy(gameObject);
    //                 break;
    //         }
    //     }
    // }
}
