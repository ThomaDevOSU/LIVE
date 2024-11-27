using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

/// <summary>
/// Handles communication with the GPT API to generate responses based on player input.
/// </summary>
public class GPTService : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the GPTService.
    /// </summary>
    public static GPTService Instance;

    /// <summary>
    /// API key for authenticating requests to the GPT API. This needs to be defined on the GPTService game object. We need a better way to store this.
    /// </summary>
    private readonly string apiKey = "API-KEY";
    private readonly string api_url = "https://api.openai.com/v1/chat/completions";

    public string playerInput, prompt, response, request;
    private PlayerData playerData;
    private NPC NPCData;
    private Task taskData;

    /// <summary>
    /// List of messages exchanged with the GPT API to maintain conversation context.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Ensures only one instance of GPTService exists and persists across scenes. Singleton pattern.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Generates a prompt for the GPT API based on the player's input. Currently only supports input but will later take Player and NPC data.
    /// </summary>
    /// <param name="playerInput">The input provided by the player.</param>
    /// <returns>A formatted prompt string for the GPT API.</returns>
    public string GeneratePrompt(string playerInput, PlayerData playerData, NPC NPCData, Task taskData)
    {
        string prompt = $@"
    NPC Information:
    - Name: {NPCData.Name}
    - Profession: {NPCData.Job}
    - Location: {NPCData.CurrentLocation}
    - Personality Traits: {string.Join(", ", NPCData.Personality)} 
    - Description: {NPCData.Description}
    Context:
    - Previous Interactions: {string.Join(", ", messages.Select(m => m.content))}
    Game State:
    - Player Name: {playerData.name}
    - Language Level: Advanced
    - Current Task: {taskData.TaskDescription}
    - Task Answer: {taskData.TaskAnswer}
    - Time of Day: Afternoon
    - Game Language: {playerData.language}
    Player Input: {playerInput}
    Instructions:
    Respond concisely as {NPCData.Name} in {playerData.language}.
    When responding with nondialogue use * * as action tags. Example: *picks up the money*. 
    Do not use any formatting your response should be plain text and only the dialogue itself. Do not use the 'Response:' to being any response.
    Use a tone that matches the personality traits, remain consistent with previous interactions, and ensure relevance to: {taskData.TaskDescription}";

        return prompt;
    }

    /// <summary>
    /// Sends a request to the GPT API and processes the response.
    /// </summary>
    /// <param name="playerInput">The input provided by the player.</param>
    /// <returns>An IEnumerator for coroutine handling. This cannot be used like a typical return value and allows async behavior.</returns>
    public IEnumerator ApiCall(string playerInput)
    {
        if (messages.Count == 0)
        {
            Debug.Log("No messages found, generating prompt...");
            playerData = RetrievePlayerData();
            NPCData = RetrieveNPCData();

            // ** This is temporary until TaskManager is updated to use NPC names.
            taskData = RetrieveTask(NPCData.Job, NPCData.CurrentLocation);

            prompt = GeneratePrompt(playerInput, playerData, NPCData, taskData);
            messages.Add(new Message { role = "system", content = prompt }); // Add the player's prompt to the message list.
        }
        else
        {
            Debug.Log("Messages found, adding player input...");
            messages.Add(new Message { role = "user", content = playerInput });
        }

        foreach (var message in messages)
        {
            Debug.Log(message.role);
            Debug.Log(message.content);
        }

        RequestBody requestBody = new()
        {
            model = "gpt-4o-mini",
            messages = messages.ToArray(),
            max_completion_tokens = 100
        };

        string jsonBody = JsonUtility.ToJson(requestBody);
        UnityWebRequest request = new(api_url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            response = request.downloadHandler.text;
            response = ParseResponse(response); // Parse the response content.
            messages.Add(new Message { role = "assistant", content = "Response: " + response });
        }
    }

    private PlayerData RetrievePlayerData()
    {
        playerData = GameManager.Instance.CurrentPlayerData;
        return playerData;
    }

    private NPC RetrieveNPCData()
    {
        NPCData = NPCManager.Instance.GetCurrentDialogueNPC();
        return NPCData;
    }

    private Task RetrieveTask(string npcName, string location)
    {
        // Right now npcName is really the job until TaskManager is updated with NPC names.
        taskData = TaskManager.Instance.SubjectTask(npcName, location);
        if (taskData == null)
        {
            // This is stricly for testing purposes. Better TaskManager implementation needed.
            Debug.Log("Task not found for current NPC/Location");
            taskData = new Task
            {
                TaskSubject = "Bread",
                TaskNPC = NPCData.Name,
                TaskLocation = NPCData.CurrentLocation,
                TaskAnswer = "Can I buy a loaf of bread?",
                TaskDifficulty = 1,
                T_TaskDescription = "Buy Bread",
                T_TaskSubject = "Bread",
                T_TaskNPC = NPCData.Name,
                T_TaskLocation = NPCData.CurrentLocation,
                T_TaskAnswer = "Can I buy a loaf of bread?",
                IsCompleted = false,
                IsCustom = false
            };
        }
        else
        {
            Debug.Log("Task found");
            taskData.printData();
        }
        return taskData;
    }

    /// <summary>
    /// Parses the JSON response from the GPT API to extract the content.
    /// Also removes the "Response: " prefix from the content because the AI will not stop adding it.
    /// </summary>
    /// <param name="jsonResponse">The raw JSON response from the GPT API.</param>
    /// <returns>The extracted content from the response, or null if parsing fails.</returns>
    public string ParseResponse(string jsonResponse)
    {
        string pattern = "\"content\":\\s*\"([^\"]*)\"";
        Match match = Regex.Match(jsonResponse, pattern);
        if (match.Success)
        {
            // The AI will not stop adding "Response: " so we need to remove it. Can't prompt engineer this out 100% of the time.
            string content = match.Groups[1].Value;
            if (content.StartsWith("Response: "))
            {
                content = content["Response: ".Length..];
            }
            return content;
        }
        else
        {
            Debug.LogError("Failed to parse response");
            return null;
        }
    }

    /// <summary>
    /// Represents a message in the GPT API conversation, including the role and content.
    /// </summary>
    [System.Serializable]
    public class Message
    {
        /// <summary>
        /// The role of the message (e.g., "user" for player input or "system" for API responses).
        /// </summary>
        public string role;

        /// <summary>
        /// The content of the message.
        /// </summary>
        public string content;
    }

    /// <summary>
    /// Represents the body of the GPT API request.
    /// </summary>
    [System.Serializable]
    public class RequestBody
    {
        /// <summary>
        /// The GPT model to use for the completion request.
        /// </summary>
        public string model;

        /// <summary>
        /// The list of messages to provide context for the conversation. The current player input is the last element.
        /// </summary>
        public Message[] messages;

        /// <summary>
        /// The maximum number of tokens to generate in the response.
        /// </summary>
        public int max_completion_tokens;
    }
}
