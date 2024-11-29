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
    public Player player;

    public Conversation currentChat;
    public GameObject dialoguePrefab;
    private GameObject dialoguePrefabInstance = null;

    CoreInput action;
    private DisplayText dialougesDisplayText;
    private bool conversationStarted = false;
    private InputAction interactAction;

    private void Awake()
    {
        // Input
        action = new CoreInput();
        interactAction = action.Player.Interact;
    }

    public void Update()
    {
        bool pressedInteract = interactAction.WasPressedThisFrame() && interactAction.ReadValue<float>() == 1;

        if (pressedInteract && conversationStarted)
        {
            nextLine();
        }
    }

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
