using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GglowOrbFragment : MonoBehaviour, IInteractable
{
    MazeManager _mazeManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _mazeManager = GameObject.FindWithTag("MazeManager").GetComponent<MazeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Interact(){
        _mazeManager.GetFragment();
        Destroy(gameObject);
    }
}
