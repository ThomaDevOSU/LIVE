using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single dialogue entry, including the dialogue lines and optional choices.
/// </summary>
public class DialogueEntry
{
    /// <summary>
    /// Array of sentences or dialogue lines for this entry.
    /// </summary>
    public string[] sentences;

    // /// <summary>
    // /// Indicates whether this dialogue entry has player choices. This is an offline mode feature.
    // /// </summary>
    // public bool hasChoices;

    // /// <summary>
    // /// Array of possible player choices for this dialogue entry. This is an offline mode feature.
    // /// </summary>
    // public DialogueChoice[] choices;
}

// Uncomment and extend as needed for dialogue choices.
// /// <summary>
// /// Represents a player choice in a dialogue, including the text and the next dialogue entry. This is an offline mode feature.
// /// </summary>
// public class DialogueChoice
// {
//     /// <summary>
//     /// Text for the player's choice.
//     /// </summary>
//     public string choiceText;

//     /// <summary>
//     /// The next dialogue entry that follows this choice.
//     /// </summary>
//     public DialogueEntry nextDialogue;
// }
