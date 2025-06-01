using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Represents an NPC.
/// </summary>
[System.Serializable]
public class NPC
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public Direction movementDirection;
    public float speed;
    public Vector3 velocity;

    public string[] Greeting;
    public bool inDialogue = false;
    public int ID;
    public string Name;
    public string Job;
    public string Description;
    public List<string> Personality;
    public List<ScheduleEntry> Schedule;
    public List<Message> messages;
    public string CurrentLocation { get; set; }
    public Vector2 CurrentCoordinates { get; set; }

    /// <summary>
    /// NavMesh Stuff
    /// </summary
    [SerializeField] Transform target;
    public NavMeshAgent agent;

    /// <summary>
    /// Animation stuff
    /// </summary>
    public Animator animator;


    /// <summary>
    /// Set direction and speed of NPC. This is for animations.
    /// </summary>
    public void UpdateSpeedandDirection()
    {
        velocity = agent.velocity;
        speed = velocity.magnitude;

        animator.SetFloat("Speed", speed);
        Vector3 directionVector = velocity.normalized;

        if (Mathf.Abs(directionVector.x) > Mathf.Abs(directionVector.y))
        {
            movementDirection = directionVector.x > 0 ? Direction.Right : Direction.Left;
            animator.SetBool("Up", false);
            animator.SetBool("Down", false);
            if (movementDirection == Direction.Left)
            {
                animator.SetBool("Right", false);
                animator.SetBool("Left", true);
            } else
            {
                animator.SetBool("Left", false);
                animator.SetBool("Right", true);
            }
            
        }
        else
        {
            animator.SetBool("Right", false);
            animator.SetBool("Left", false);
            movementDirection = directionVector.y > 0 ? Direction.Up : Direction.Down;
            if (movementDirection == Direction.Up)
            {
                animator.SetBool("Down", false);
                animator.SetBool("Up", true);
            } else
            {
                animator.SetBool("Up", false);
                animator.SetBool("Down", true);
            }
        }
    }



    /// <summary>
    /// Prints the NPC's data to the debug log.
    /// </summary>
    public void printData()
    {
        Debug.Log($"Printing data about this NPC...\n" +
                  $"Greeting: {Greeting}\n" +
                  $"Name: {Name}\n" +
                  $"Job: {Job}\n" +
                  $"Description: {Description}\n" +
                  $"Personality: {string.Join(", ", Personality)}\n" +
                  $"Current Location: {CurrentLocation}\n" +
                  $"Current Coordinates: {CurrentCoordinates}\n" +
                  $"Is in Dialogue: {inDialogue}");
        Debug.Log("Printing NPC's messages...");
        foreach (Message message in messages)
        {
            Debug.Log($"Role: {message.role}\n" + $"Content: {message.content}");
        }
    }

    /// <summary>  
    /// Stops the NPC's movement by halting the NavMeshAgent.  
    /// </summary>  
    public void StopNPCMovement()
    {
        if (agent != null && agent.isActiveAndEnabled && inDialogue)
        {
            agent.isStopped = true;
        }
    }

    /// <summary>  
    /// Stops the NPC's movement by halting the NavMeshAgent.  
    /// </summary>  
    public void StartNPCMovement()
    {
        if (agent != null && agent.isActiveAndEnabled && !inDialogue)
        {
            agent.isStopped = false;
        }
    }
}

/// <summary>
/// Represents a schedule entry for an NPC.
/// </summary>
public class ScheduleEntry
{
    public string waypoint;
    public int time;
    public string location;
}

/// <summary>
/// Represents a message in the GPT API conversation.
/// </summary>
[System.Serializable]
public class Message
{
    public string role;    // The role of the message (e.g., "user" or "system").
    public string content; // The content of the message.
}
