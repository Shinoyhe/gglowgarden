using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct Conversation
{
    public string[] lines;
    [HideInInspector]
    public int currentLine;
}

public struct Tags
{
    public string name;

    public Tags(string Name = "")
    {
        name = Name;
    }
}


public class ChatNode : MonoBehaviour, IInteractable
{
    [Header("Dependencies")]
    public Player player;
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

        // Filter tags
        Tags currentTags = getTags(currentTextLine);
        string currentTextNoTags = removeTags(currentTextLine);

        // Setup our dialouge
        dialougesDisplayText.setupDialogueText(currentTextNoTags, currentTags);

        // Increase line count
        currentChat.currentLine += 1;
    }

    public Tags getTags(string currentLine)
    {
        Tags newTags = new Tags();

        // Get tags with structure [KEY,var,var] [tags,var,var]
        Regex regex = new Regex(@"\[(.*?)\]");
        MatchCollection matches = regex.Matches(currentLine);

        // For each [KEY,var] match
        for (int i = 0; i < matches.Count; i++)
        {
            // Get each part of match
            string[] tagParts = matches[i].Groups[1].Value.Split(',');
            string tagKey = tagParts[0].ToLower();

            switch (tagKey)
            {
                case "name":
                    if (tagParts.Length != 2) break;
                    newTags.name = removeStartingSpace(tagParts[1]);
                    break;
            }

        }

        return newTags;
    }

    public string removeStartingSpace(string input)
    {
        if (input[0] == ' ')
        {
            input = input.Substring(1);
        }

        return input;
    }

    public string removeTags(string currentLine)
    {
        // Get tags with structure [KEY,var,var] [tags,var,var]
        Regex regex = new Regex(@"\[(.*?)\]");
        MatchCollection matches = regex.Matches(currentLine);

        if (matches.Count == 0) return currentLine;

        // Get the last match
        Match lastMatch = matches[matches.Count - 1];

        // Get the index of the character after the last match
        int startIndex = lastMatch.Index + lastMatch.Length;

        // Extract the remaining substring
        string remainingString = currentLine.Substring(startIndex);

        remainingString = removeStartingSpace(remainingString);

        return remainingString;
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
