using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[System.Serializable]
public struct Conversation
{
    public string[] lines;
    [HideInInspector]
    public int currentLine;
}

public class ChatNode : ChatHelper, IInteractable
{
    [Header("Outside Links")]
    public UnityEvent callback;

    [Header("Dependencies")]
    public Player player;
    public CameraSwapper cameraController;
    public GameObject dialoguePrefab;

    [Header("Conversation Text")]
    public TextAsset conversation;

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
    string lastConvo;
    private InputAction interactAction;

    void OnValidate()
    {
        if (conversation == null) return;
        if (lastConvo == conversation.text) return;
        if (Application.isPlaying == true) return;

        Debug.Log("Performed update to Chat Text");

        currentChat.lines = conversation.text.Split('\n');
        lastConvo = conversation.text;
    }

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

        if (dialougesDisplayText.finishedTypingText)
        {
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
        if (conversationStarted || player.inConversation) return;

        // Freeze player
        player.ToggleMovement(false);
        player.inConversation = true;

        // Setup interact UI
        interactAction.Enable();

        // Create UI
        createDialogueUI();

        // Move Camera
        cameraController.startConversation(this.gameObject.transform,player.gameObject.transform);

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

        // Filter tags
        Tags currentTags = getTags(currentTextLine);
        string currentTextNoTags = removeTags(currentTextLine);

        // Setup our dialouge
        dialougesDisplayText.setupDialogueText(currentTextNoTags, currentTags);

        // Express any emotions!
        switch (currentTags.emotion)
        {
            case 1:
                // Surpised!
                Debug.Log("Suprised!");
                break;
        }

        // Increase line count
        currentChat.currentLine += 1;
    }


    public void endChat()
    {
        // UnFreeze player
        player.ToggleMovement(true);
        player.inConversation = false;

        // Destroy UI Element
        if (dialoguePrefabInstance != null)
        {
            Destroy(dialoguePrefabInstance);
        }

        // Stop interact
        interactAction.Disable();

        // Move Camera Back
        cameraController.endConversation();

        // Reset trackers
        currentChat.currentLine = 0;
        conversationStarted = false;

        // Trigger any callbacks
        callback.Invoke();
    }
}
