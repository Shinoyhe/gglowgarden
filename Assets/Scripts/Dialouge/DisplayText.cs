using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayText : MonoBehaviour
{
    [Header("Dependencies")]
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("Audio Clips")]
    public AudioClip[] talkingClips;
    public AudioClip punctuation;

    [Header("Speaking Settings")]
    public float minPitch = 0.8f;
    public float maxPitch = 0.9f;
    public float textSpeed = 0.05f;


    private int charactersForThisLine = 0;
    private AudioSource source;
    [HideInInspector]
    public bool finishedTypingText = false;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void setupDialogueText(string text)
    {
        // Set Text
        dialogueText.text = text;
        charactersForThisLine = text.Length;

        // Reset Trackers
        finishedTypingText = false;
        dialogueText.maxVisibleCharacters = 0;

        // Start Printing
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

        if(dialogueText.maxVisibleCharacters >= charactersForThisLine)
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
        if (source.isPlaying)
        {
            source.Stop();
        }

        source.pitch = Random.Range(minPitch, maxPitch);

        if (char.IsLetter(currentCharacter))
        {
            int audioIndex = currentCharacter % talkingClips.Length;
            source.PlayOneShot(talkingClips[audioIndex]);
        }
        else
        {
            source.PlayOneShot(punctuation);
        }
    }
}
