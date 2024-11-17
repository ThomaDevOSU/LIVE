using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
    private string apiKey = "API-KEY";
    private string api_url = "https://api.openai.com/v1/chat/completions";

    public string playerInput, prompt, response, request, playerData, NPCData;

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
    public string GeneratePrompt(string playerInput)
    {
        string prompt = "You are a baker named Bob. Your responses should be simple and not contain any text except the response itself. Use no emojis." +
            " If asked a question a normal baker would not know feign ignorance" +
            " Respond to the following prompt without breaking character and keep it concise: " + playerInput; // Hardcoded prompt for now.
        return prompt;
    }

    /// <summary>
    /// Sends a request to the GPT API and processes the response.
    /// </summary>
    /// <param name="playerInput">The input provided by the player.</param>
    /// <returns>An IEnumerator for coroutine handling. This cannot be used like a typical return value and allows async behavior.</returns>
    public IEnumerator apiCall(string playerInput)
    {
        prompt = GeneratePrompt(playerInput);
        messages.Add(new Message { role = "user", content = prompt }); // Add the player's prompt to the message list.

        Headers headers = new Headers
        {
            Authorization = "Bearer " + apiKey,
            ContentType = "application/json"
        };

        RequestBody requestBody = new RequestBody
        {
            model = "gpt-4o",
            messages = messages.ToArray(),
            max_completion_tokens = 40
        };

        string jsonBody = JsonUtility.ToJson(requestBody);
        UnityWebRequest request = new UnityWebRequest(api_url, "POST");
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
            messages.Add(new Message { role = "system", content = "Response: " + response });
        }
    }

    /// <summary>
    /// Parses the JSON response from the GPT API to extract the content.
    /// </summary>
    /// <param name="jsonResponse">The raw JSON response from the GPT API.</param>
    /// <returns>The extracted content from the response, or null if parsing fails.</returns>
    public string ParseResponse(string jsonResponse)
    {
        string pattern = "\"content\":\\s*\"([^\"]*)\"";
        Match match = Regex.Match(jsonResponse, pattern);
        if (match.Success)
        {
            return match.Groups[1].Value;
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
    /// Represents the headers required for the GPT API request.
    /// </summary>
    [System.Serializable]
    public class Headers
    {
        /// <summary>
        /// The authorization token for the API.
        /// </summary>
        public string Authorization;

        /// <summary>
        /// The content type of the request.
        /// </summary>
        public string ContentType;
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
