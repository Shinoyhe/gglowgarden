using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestCollision : MonoBehaviour
{
    enum Tester {TELEPORTER, DIALGOUER, WALLBREAKER};
    [SerializeField] Tester type = Tester.WALLBREAKER;
    [SerializeField] List<WallBreaker> walls2Break;
    [SerializeField] GameObject testDialogue;
    [SerializeField] float dialogueWait = 3;
    
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
        StartCoroutine(DisplayDialogue());
    }
    
    IEnumerator DisplayDialogue(){
        testDialogue.SetActive(true);
        yield return new WaitForSeconds(dialogueWait);
        testDialogue.SetActive(true);
    }
    
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            switch (type)
            {
                case Tester.TELEPORTER:
                    Teleport();
                    Destroy(gameObject);
                    break;
                case Tester.DIALGOUER:
                    Dialogue();
                    break;
                default:
                    foreach (var wall in walls2Break){
                        // Destroy(wall.gameObject);
                        wall.BreakWall();
                    }
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
