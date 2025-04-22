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
    private readonly string apiKey = "";
    private readonly string api_url = "https://api.openai.com/v1/chat/completions";

    public string playerInput, prompt, response, request, taskResponse;
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
    public string GenerateDialoguePrompt(string playerInput, PlayerData playerData, NPC NPCData, Task taskData)
    {
        taskData ??= new Task
        {
            TaskDifficulty = 0,
            TaskDescription = "No task available",
            TaskSubject = "N/A",
            TaskNPC = "N/A",
            TaskLocation = "N/A",
            TaskAnswer = "N/A",

            T_TaskDescription = "No task available",
            T_TaskSubject = "N/A",
            T_TaskNPC = "N/A",
            T_TaskLocation = "N/A",
            T_TaskAnswer = "N/A",
            IsCompleted = false,
            IsCustom = false
        };
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
    Do not use any formatting your response should be plain text and only the dialogue itself. Do not use the text 'Response:' to format the response.
    Use a tone that matches the personality traits, remain consistent with previous interactions, and subtely maintain relevance to: {taskData.TaskDescription}";

        return prompt;
    }

    /// <summary>
    /// Sends a request to the GPT API for dialogue and processes the response.
    /// </summary>
    /// <param name="playerInput">The input provided by the player.</param>
    /// <param name="NPCData">The data of the NPC.</param>
    /// <returns>An IEnumerator for coroutine handling.</returns>
    public IEnumerator DialogueApiCall(string playerInput, NPC NPCData)
    {
        if (NPCData.messages.Count == 1)
        {
            RetrievePlayerData();
            RetrieveTask(NPCData.Job, NPCData.CurrentLocation);

            prompt = GenerateDialoguePrompt(playerInput, playerData, NPCData, taskData);
            NPCData.messages.Insert(0, new Message { role = "system", content = prompt });
            NPCData.messages.Add(new Message { role = "user", content = playerInput });
        }
        else
        {
            NPCData.messages.Add(new Message { role = "user", content = playerInput });
        }

        DialogueRequestBody requestBody = new()
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
            response = ParseDialogueResponse(response);
            NPCData.messages.Add(new Message { role = "assistant", content = "Response: " + response });
        }
    }

    public string GenerateTaskPrompt(string playerInput, PlayerData playerData, NPC NPCData, Task taskData)
    {
        taskData ??= new Task
        {
            TaskDifficulty = 0,
            TaskDescription = "No task available",
            TaskSubject = "N/A",
            TaskNPC = "N/A",
            TaskLocation = "N/A",
            TaskAnswer = "N/A",

            T_TaskDescription = "No task available",
            T_TaskSubject = "N/A",
            T_TaskNPC = "N/A",
            T_TaskLocation = "N/A",
            T_TaskAnswer = "N/A",
            IsCompleted = false,
            IsCustom = false
        };
        string prompt = $@"
        NPC and Task Context:
        - Name: {NPCData.Name}
        - Profession: {NPCData.Job}
        - Location: {NPCData.CurrentLocation}
        - Current Task: {taskData.TaskDescription}
        - Task Answer: {taskData.TaskAnswer}
        - Time of Day: Afternoon
        - Player Input: {playerInput}
        Instructions:
        Has the player successfully completed the task based on the task description, NPC's occupation, location, and whether their input loosely matches the intended meaning of the task answer? 
        Respond with ""true"" or ""false"" only.
        ";
        return prompt;
    }

    /// <summary>
    /// Sends a request to the GPT API for dialogue and processes the response.
    /// </summary>
    /// <param name="playerInput">The input provided by the player.</param>
    /// <param name="NPCData">The data of the NPC.</param>
    /// <returns>An IEnumerator for coroutine handling.</returns>
    public IEnumerator TaskApiCall(string playerInput, NPC NPCData)
    {
        RetrievePlayerData();
        RetrieveTask(NPCData.Job, NPCData.CurrentLocation);
        prompt = GenerateTaskPrompt(playerInput, playerData, NPCData, taskData);
        var message = new Message { role = "system", content = prompt };
        TaskRequestBody requestBody = new()
        {
            model = "gpt-4o",
            messages = new Message[] { message },
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
            Debug.LogError($"Error: {request.error}");
            Debug.LogError($"Response Code: {request.responseCode}");
            Debug.LogError($"Response: {request.downloadHandler.text}");
        }
        else
        {
            taskResponse = request.downloadHandler.text;
            taskResponse = ParseTaskResponse(taskResponse);
        }
    }

    /// <summary>
    /// Sends a request to the GPT API to summarize all previous dialogue interactions in the NPC's message list.
    /// </summary>
    /// <param name="NPCData">The data of the NPC whose messages need to be summarized.</param>
    /// <returns>An IEnumerator for coroutine handling.</returns>
    public IEnumerator SummarizeMessagesApiCall(NPC NPCData)
    {
        // Generate the prompt for summarizing messages
        string prompt = $@"
    Conversation History:
    {string.Join("\n", NPCData.messages.Select(m => $"{m.role}: {m.content}"))}
    Instructions:
    Summarize the above conversation history into a single concise paragraph. 
    The summary should cover all important details and ensure no critical information is lost. 
    The response should be plain text and should not include any formatting or additional instructions.";

        // Create the request body
        var message = new Message { role = "system", content = prompt };
        DialogueRequestBody requestBody = new()
        {
            model = "gpt-4o",
            messages = new Message[] { message },
            max_completion_tokens = 150
        };

        // Serialize the request body to JSON
        string jsonBody = JsonUtility.ToJson(requestBody);

        // Create the UnityWebRequest
        UnityWebRequest request = new(api_url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {request.error}");
            Debug.LogError($"Response Code: {request.responseCode}");
            Debug.LogError($"Response: {request.downloadHandler.text}");
        }
        else
        {
            string summaryResponse = request.downloadHandler.text;
            string summary = ParseDialogueResponse(summaryResponse);

            if (!string.IsNullOrEmpty(summary))
            {
                Debug.Log($"Conversation Summary: {summary}");
                // right now this is just added into the NPC immediately. We will see if this is the best going forward
                NPCData.messages.Add(new Message { role = "system", content = summary });
            }
            else
            {
                Debug.LogError("Failed to generate a conversation summary.");
            }
        }
    }


    /// <summary>
    /// Processes the task response and take call the appropriate method from TaskManager.
    /// </summary>
    public void ProcessTask(string response)
    {
        if (response == null)
        {
            Debug.Log("GPT failed to respond");
        }
        else if (bool.Parse(response) is true)
        {
            Debug.Log("Task completed");
            TaskManager.Instance.CompleteTask(taskData);
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
        }
        else
        {
            taskData.printData();
        }
    }

    /// <summary>
    /// Parses the JSON response from the GPT API to extract the content.
    /// This method is necessary because the response often includes irrelevant text that cannot be prompted away.
    /// </summary>
    /// <param name="jsonResponse">The raw JSON response from the GPT API.</param>
    /// <returns>The extracted content from the response, or null if parsing fails.</returns>
    public string ParseDialogueResponse(string jsonResponse)
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
    /// Parses the JSON response from the GPT API to extract the content.
    /// </summary>
    /// <param name="jsonResponse">The raw JSON response from the GPT API.</param>
    /// <returns>The extracted content from the response, or null if parsing fails.</returns>
    public string ParseTaskResponse(string jsonResponse)
    {
        string pattern = "\"content\":\\s*\"([^\"]*)\"";
        Match match = Regex.Match(jsonResponse, pattern);
        if (match.Success)
        {
            string content = match.Groups[1].Value;
            return content;
        }
        else
        {
            Debug.LogError("Failed to parse response");
            return null;
        }
    }
}

    /// <summary>
    /// Represents the body of the dialogue GPT API request.
    /// </summary>
    [System.Serializable]
    public class DialogueRequestBody
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

    /// <summary> 
    /// Represents the body of the task GPT API request.
    /// </summary>
    [System.Serializable]
    public class TaskRequestBody
    {
        /// <summary>
        /// The GPT model to use for the completion request.
        /// </summary>
        public string model;
        /// <summary>
        /// The prompt for the task completion request.
        /// </summary>
        public Message[] messages;
        /// <summary>
        /// The maximum number of tokens to generate in the response.
        /// </summary>
        public int max_completion_tokens;
    }

