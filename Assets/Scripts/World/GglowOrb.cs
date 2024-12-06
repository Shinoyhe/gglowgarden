using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GglowOrb : MonoBehaviour, IInteractable
{
    bool obtained = false;
    ChatNode speech;
    
    // Start is called before the first frame update
    void Start()
    {
        speech = GetComponent<ChatNode>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator AfterInteract(){
        GetComponent<MeshRenderer>().enabled = false;
        speech.enabled = true;
        yield return null;
        speech.Interact();
    }
    
    public void Interact(){
        if (!obtained){
            obtained = true;
            GameObject.FindWithTag("Player").GetComponent<Player>().EatOrb(() => StartCoroutine(AfterInteract()));
        }
    }
}
