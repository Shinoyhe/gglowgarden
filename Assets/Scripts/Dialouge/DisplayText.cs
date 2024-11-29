using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayText : MonoBehaviour
{

    public TMP_Text nameText;
    public TMP_Text dialogueText;

    public void setupDialogueText(string text)
    {
        dialogueText.text = text;
    }
    public void setupNameText(string name)
    {
        nameText.text = name;
    }
}
