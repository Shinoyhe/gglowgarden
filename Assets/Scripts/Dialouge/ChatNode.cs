using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Conversation
{
    public string[] lines;
    public int currentLine;
}

public class ChatNode : MonoBehaviour, IInteractable
{
    public Conversation currentChat;

    public void Interact()
    {
        nextLine();
    }

    public void nextLine()
    {
        // Pre Condition: Must have line of text to display
        if (currentChat.lines.Length == 0) return;

        displayLine();

        // Try to end conversation
        if (currentChat.currentLine >= currentChat.lines.Length)
        {
            endChat();
        }
    }

    private void displayLine()
    {
        Debug.Log(currentChat.lines[currentChat.currentLine]);
        currentChat.currentLine += 1;
    }

    public void endChat()
    {
        currentChat.currentLine = 0;
    }
}
