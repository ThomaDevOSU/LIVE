using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string language; // What language are we trying to learn?
    public string name; // What is the characters name?
    public Gender gender; // Gender choice, 0 == female, 1 == male... Defaults to male
    public int day; // What day are we on?
    public int currency; // How much money do we have?
    public int score; // How many tasks have we completed?
    public int playerSprite; // What character model is being used?
}

public enum Gender { female, male };
