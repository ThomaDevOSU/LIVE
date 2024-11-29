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
    /// API key for authenticating requests to the GPT API.
    /// </summary>
    private readonly string apiKey = "API-KEY";
    private readonly string api_url = "https://api.openai.com/v1/chat/completions";

    public string playerInput, prompt, response, request;
    private PlayerData playerData;
    private NPC NPCData;
    private Task taskData;
    private List<Message> messages;

    /// <summary>
    /// Ensures only one instance of GPTService exists and persists across scenes.
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
    /// Generates a prompt for the GPT API.
    /// </summary>
    /// <param name="playerInput">The input provided by the player.</param>
    /// <param name="playerData">The data of the player.</param>
    /// <param name="NPCData">The data of the NPC.</param>
    /// <param name="taskData">The data of the task.</param>
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
    - Previous Interactions: {string.Join(", ", NPCData.messages.Select(m => m.content))}
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
    /// <param name="NPCData">The data of the NPC.</param>
    /// <returns>An IEnumerator for coroutine handling.</returns>
    public IEnumerator ApiCall(string playerInput, NPC NPCData)
    {
        NPCData.printData();
        if (NPCData.messages.Count == 0)
        {
            Debug.Log("No NPCData.messages found, generating prompt...");
            RetrievePlayerData();
            RetrieveTask(NPCData.Job, NPCData.CurrentLocation);

            prompt = GeneratePrompt(playerInput, playerData, NPCData, taskData);
            NPCData.messages.Add(new Message { role = "system", content = prompt });
        }
        else
        {
            Debug.Log("Messages found, adding player input...");
            NPCData.messages.Add(new Message { role = "user", content = playerInput });
        }

        foreach (var message in NPCData.messages)
        {
            Debug.Log(message.role);
            Debug.Log(message.content);
        }

        RequestBody requestBody = new()
        {
            model = "gpt-4o",
            messages = NPCData.messages.ToArray(),
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
            response = ParseResponse(response);
            NPCData.messages.Add(new Message { role = "assistant", content = "Response: " + response });
        }
    }

    private void RetrievePlayerData()
    {
        playerData = GameManager.Instance.CurrentPlayerData;
    }

    private void RetrieveTask(string npcName, string location)
    {
        taskData = TaskManager.Instance.SubjectTask(npcName, location);
        if (taskData == null)
        {
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
    }

    /// <summary>
    /// Parses the JSON response from the GPT API to extract the content.
    /// This method is necessary because the response often includes irrelevant text that cannot be prompted away.
    /// </summary>
    /// <param name="jsonResponse">The raw JSON response from the GPT API.</param>
    /// <returns>The extracted content from the response, or null if parsing fails.</returns>
    public string ParseResponse(string jsonResponse)
    {
        string pattern = "\"content\":\\s*\"([^\"]*)\"";
        Match match = Regex.Match(jsonResponse, pattern);
        if (match.Success)
        {
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
        /// The list of messages to provide context for the conversation.
        /// </summary>
        public Message[] messages;

        /// <summary>
        /// The maximum number of tokens to generate in the response.
        /// </summary>
        public int max_completion_tokens;
    }
}
