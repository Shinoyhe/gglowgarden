using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ChatPopup : ChatHelper
{
    [Header("Outside Links")]
    public UnityEvent callback;

    [Header("Dependencies")]
    public Player player;
    public GameObject dialoguePrefab;

    [Header("Set Popup Reference")]
    public TextAsset popupText;
    public int popupLineToUse = 0;

    [Space]
    [Space]
    public Conversation allPopups;

    // Dialouge Instances
    private DisplayText dialougesDisplayText;
    private GameObject dialoguePrefabInstance = null;

    // Trackers
    private int skipCount = 0;

    // Input
    CoreInput action;
    string lastConvo;
    private InputAction interactAction;

    void OnValidate()
    {
        if (popupText == null) return;
        if (lastConvo == popupText.text) return;
        if (Application.isPlaying == true) return;

        allPopups.lines = popupText.text.Split('\n');
        lastConvo = popupText.text;
    }

    // Setup our Input
    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
        }

        if (player.inConversation) { 
            Destroy(gameObject);
            return;
        }

        action = player.action;
        interactAction = action.Player.Interact;

        // Freeze player
        player.ToggleMovement(false);
        player.inConversation = true;

        // Setup interact UI
        // interactAction.Enable();

        // Create UI
        createDialogueUI();

        // Show popup
        displayLine();
    }

    public void Update()
    {
        bool pressedInteract = interactAction.WasPressedThisFrame() && interactAction.ReadValue<float>() == 1;

        if (!pressedInteract) return;

        if (dialougesDisplayText.finishedTypingText)
        {
            skipCount = 0;
            endChat();
            return;
        }

        skipCount += 1;
        if (skipCount != 2) return;

        dialougesDisplayText.skipText();
        skipCount = 0;
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

    private void displayLine()
    {
        if (popupLineToUse >= allPopups.lines.Length)
        {
            Debug.LogError("Trying to use popup number outside of given popup list!");
        }
        string currentTextLine = allPopups.lines[popupLineToUse];

        // Filter tags
        Tags currentTags = getTags(currentTextLine);
        string currentTextNoTags = removeTags(currentTextLine);

        // Setup our dialouge
        dialougesDisplayText.setupDialogueText(currentTextNoTags, currentTags);
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
        // interactAction.Disable();

        // Trigger any callbacks
        callback.Invoke();
        
        StartCoroutine(WaitThenDestroy());
    }
    
    IEnumerator WaitThenDestroy(){
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    
    public void CreatePopups(){
        allPopups.lines = popupText.text.Split('\n');
        lastConvo = popupText.text;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ChatPopup))]
public class ChatPopupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Create"))
        {
            ChatPopup temp = (ChatPopup)target;
            temp.CreatePopups();
        }
    }
}
#endif