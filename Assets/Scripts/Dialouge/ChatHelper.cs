using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
public struct Tags
{
    public string name;

    public Tags(string Name = "")
    {
        name = Name;
    }
}


public class ChatHelper : MonoBehaviour
{
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
}
