using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using FMODUnity;

[Serializable]
public struct TextTheme
{
    public Color nameTextColor;
    public Color nameBackgroundColor;
    public Color mainTextColor;
    public Color mainBackgroundColor;
}

public class DisplayText : MonoBehaviour
{
    [Header("Dependencies")]
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image dialogueTextBackground;
    public Image nameBackground;

    [Header("Themes")]
    public TextTheme narrationTheme;
    public TextTheme npcTheme;

    [Header("Audio Clips")]
    public int numTalkingClips = 7;
    public AudioClip[] talkingClips;
    public AudioClip punctuation;

    [Header("Speaking Settings")]
    public float minPitch = 0.8f;
    public float maxPitch = 0.9f;
    public float textSpeed = 0.05f;


    private int charactersForThisLine = 0;
    // private AudioSource source;
    [HideInInspector]
    public bool finishedTypingText = false;
    private Tags currentLineTags;

    private void Awake()
    {
        // source = GetComponent<AudioSource>();
    }

    public void setupDialogueText(string text, Tags currentTags)
    {
        // Set Text
        dialogueText.text = text;
        charactersForThisLine = text.Length;

        // Reset Trackers
        finishedTypingText = false;
        dialogueText.maxVisibleCharacters = 0;

        // Link Tags
        currentLineTags = currentTags;
        ProcessTags();

        // Start Printing
        StartCoroutine("nextCharacter");
    }

    public void ProcessTags()
    {
        // Get Name
        nameText.text = currentLineTags.name;

        bool isNarration = nameText.text == null;

        TextTheme currentTheme = npcTheme;

        if (isNarration)
        {
            currentTheme = narrationTheme;
        }

        nameText.color = currentTheme.nameTextColor;
        nameBackground.color = currentTheme.nameBackgroundColor;

        dialogueText.color = currentTheme.mainTextColor;
        dialogueTextBackground.color = currentTheme.mainBackgroundColor;
    }

    public void skipText()
    {
        dialogueText.maxVisibleCharacters = charactersForThisLine;
        finishedTypingText = true;
    }

    IEnumerator nextCharacter()
    {
        int index = Mathf.Clamp(dialogueText.maxVisibleCharacters, 0, dialogueText.text.Length - 1);

        char currentCharacter = dialogueText.text[index];

        float actualTextSpeed = textSpeed;

        if (currentCharacter == '.')
        {
            actualTextSpeed *= 5;
        }

        yield return new WaitForSeconds(actualTextSpeed);

        // Play sound every other character or if a punctuation
        if (currentCharacter == '.' || dialogueText.maxVisibleCharacters % 2 == 0)
        {
            playTalkSound(currentCharacter);
        }

        // Get next character
        dialogueText.maxVisibleCharacters += 1;

        if (dialogueText.maxVisibleCharacters >= charactersForThisLine)
        {
            finishedTypingText = true;
        }
        else
        {
            StartCoroutine("nextCharacter");
        }
    }

    void playTalkSound(char currentCharacter)
    {
        // if (source.isPlaying)
        // {
        //     source.Stop();
        // }

        float pitch = UnityEngine.Random.Range(minPitch, maxPitch);

        if (char.IsLetter(currentCharacter))
        {
            // int audioIndex = currentCharacter % talkingClips.Length;
            // source.PlayOneShot(talkingClips[audioIndex]);
            SoundManager.Instance.PlayLetterTrack((LetterSFX)(currentCharacter % numTalkingClips), pitch);
        }
        else
        {
            // source.PlayOneShot(punctuation);
            SoundManager.Instance.PlayLetterTrack(LetterSFX.PUNCTUATION, pitch);
        }
    }
}
