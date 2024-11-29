using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct Conversation
{
    public string name;
    public string[] lines;
    public int currentLine;
}

public class ChatNode : MonoBehaviour, IInteractable
{
    [Header("Dependencies")]
    public Player player;
    public GameObject dialoguePrefab;

    [Space]
    [Space]
    public Conversation currentChat;

    // Dialouge Instances
    private DisplayText dialougesDisplayText;
    private GameObject dialoguePrefabInstance = null;

    // Trackers
    private int skipCount = 0;
    private bool conversationStarted = false;

    // Input
    CoreInput action;
    private InputAction interactAction;

    // Setup our Input
    private void Awake()
    {
        action = new CoreInput();
        interactAction = action.Player.Interact;
    }

    public void Update()
    {
        bool pressedInteract = interactAction.WasPressedThisFrame() && interactAction.ReadValue<float>() == 1;

        if (!pressedInteract || !conversationStarted) return;
        
        if (dialougesDisplayText.finishedTypingText) {
            skipCount = 0;
            nextLine();
            return;
        }

        skipCount += 1;
        if (skipCount != 2) return;
            
        dialougesDisplayText.skipText();
        skipCount = 0;
    }

    // Called by player to start conversation
    // Uses its own internal interaction System for the rest of the inputs
    // Just incase the player leaves the range of the player by some command.
    public void Interact()
    {
        if (conversationStarted) return;

        // Freeze player
        player.ToggleMovement(false);

        // Setup interact UI
        interactAction.Enable();

        // Create UI
        createDialogueUI();

        // Start Conversation
        nextLine();
        conversationStarted = true;
    }

    public void createDialogueUI()
    {
        // Destroy any past dialogue UI
        if (dialoguePrefabInstance != null)
        {
            Destroy(dialoguePrefabInstance);
        }

        // Create our new dialogue prefab
        dialoguePrefabInstance = Instantiate(dialoguePrefab);
        dialougesDisplayText = dialoguePrefabInstance.GetComponent<DisplayText>();

        // Pre Condition: Chatnode must have a valid Dialoge Prefab set 
        if (dialougesDisplayText == null)
        {
            Debug.LogError("ChatNodes Dialoge Prefab contains no DisplayText component in the root!");
        }
    }

    public void nextLine()
    {
        // Try to end conversation
        if (currentChat.currentLine >= currentChat.lines.Length)
        {
            endChat();
        }
        else
        {
            displayLine();
        }
    }

    private void displayLine()
    {
        string currentTextLine = currentChat.lines[currentChat.currentLine];

        currentChat.currentLine += 1;

        // Setup our dialouge
        dialougesDisplayText.setupNameText(currentChat.name);
        dialougesDisplayText.setupDialogueText(currentTextLine);
    }

    public void endChat()
    {
        // UnFreeze player
        player.ToggleMovement(true);

        // Destroy UI Element
        if (dialoguePrefabInstance != null)
        {
            Destroy(dialoguePrefabInstance);
        }

        // Stop interact
        interactAction.Disable();

        // Reset trackers
        currentChat.currentLine = 0;
        conversationStarted = false;
    }
}
