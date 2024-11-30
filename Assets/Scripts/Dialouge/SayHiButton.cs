using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SayHiButton : MonoBehaviour, IInteractable
{
    public string text;

    public void Interact()
    {
        Debug.Log(text);
    }
}
