using System.Collections.Generic;
using UnityEngine;

public class NPC
{
    public string Greeting;
    public bool inDialogue = false;
    public int ID;
    public string Name;
    public string Job;
    public string Description;
    public List<string> Personality;
    public ScheduleEntry[] Schedule;
    public string CurrentLocation { get; set; }
    public Vector3 CurrentCoordinates { get; set; }
    // It may or may not be necessary to add translated fields here

}

public class ScheduleEntry
{
    public Vector3 Coordinates { get; set; }
    public string Location { get; set; }
    // public string time { get; set; }
}
