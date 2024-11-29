using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayText : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    private int charactersForThisLine = 0;
    public bool finishedTypingText = false;

    public void setupDialogueText(string text)
    {
        dialogueText.text = text;
        finishedTypingText = false;
        dialogueText.maxVisibleCharacters = 0;
        charactersForThisLine = text.Length;
        StartCoroutine("nextCharacter");
    }
    public void setupNameText(string name)
    {
        nameText.text = name;
    }

    public void skipText()
    {
        dialogueText.maxVisibleCharacters = charactersForThisLine;
        finishedTypingText = true;
    }

    IEnumerator nextCharacter()
    {
        yield return new WaitForSeconds(.1f);
        dialogueText.maxVisibleCharacters += 1;

        if(dialogueText.maxVisibleCharacters >= charactersForThisLine)
        {
            finishedTypingText = true;
        }
        else
        {
            StartCoroutine("nextCharacter");
        }
    }
}
