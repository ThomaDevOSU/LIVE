using System.Collections.Generic;
using UnityEngine;

public class DialogueEntry
{
    public string[] sentences;  // Dialogue lines.
    public bool hasChoices;

    public DialogueChoice[] choices;              // Optional player choices.
}

public class DialogueChoice
{
    public string choiceText;
    public DialogueEntry nextDialogue;  // Where the choice leads.
}