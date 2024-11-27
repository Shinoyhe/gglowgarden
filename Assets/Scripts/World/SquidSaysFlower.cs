using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidSaysFlower : MonoBehaviour, IInteractable
{
    [SerializeField] FlowerColor flowerColor = FlowerColor.R;
    [SerializeField] SquidSays squidSaysManager;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Interact(){
        squidSaysManager.GetFlower(flowerColor);
    }
}
