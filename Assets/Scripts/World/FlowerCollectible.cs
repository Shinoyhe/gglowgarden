using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerCollectible : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Interact(){
        GameObject.FindWithTag("UIManager").GetComponent<UIManager>().AddFlower();
        gameObject.SetActive(false);
    }
}
