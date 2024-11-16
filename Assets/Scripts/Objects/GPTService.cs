using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GPTService
{
    public string apiKey = "API-KEY";
    public string api_url = "https://api.openai.com/v1/chat/completions";
    public string playerInput;
    public string prompt, response, request, playerData, NPCData;
    public List<Message> messages = new List<Message>();

    public string GeneratePrompt(string playerInput) // This will be passed playerInput, playerData, and NPCData in the future.
    {
        string prompt = "You are a baker named Bob. Respond to the following prompt without breaking character and keep it concise: " + playerInput;
        return prompt;
    }

    public IEnumerator apiCall(string playerInput)
    {
        Debug.Log("Starting API Call");
        prompt = GeneratePrompt(playerInput);
        messages.Add(new Message { role = "user", content = prompt }); // Add the player prompt to the messages list. This should hold all previous player input and GPT responses. Broken!

        Headers headers = new Headers
        {
            Authorization = "Bearer " + apiKey,
            ContentType = "application/json"
        };

        RequestBody requestBody = new RequestBody
        {
            model = "gpt-4o",
            messages = messages.ToArray(),
            max_completion_tokens = 70
        };
        Debug.Log("Request Body: " + requestBody.messages); // This is broken! Yipee!

        // Convert the headers and request body to JSON
        string jsonHeaders = JsonUtility.ToJson(headers);
        string jsonBody = JsonUtility.ToJson(requestBody);
        UnityWebRequest request = new UnityWebRequest(api_url, "POST");
        // Convert the body to a byte array.
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
            // add the response to the messages list
            messages.Add(new Message { role = "system", content = "Response:" + response });
            Debug.Log("Response: " + response);
        }
    }

    [System.Serializable]
    public class Message
    {
        public string role; // The role of the message. Either "user" for a player prompt or "system" for a system prompt.
        public string content;
    }

    [System.Serializable]
    public class Headers
    {
        public string Authorization;
        public string ContentType;
    }

    [System.Serializable]
    public class RequestBody
    {
        public string model; // The model to use for completion. Should usually be "4o". Defaults to "4".
        public Message[] messages; // This includes all player input. The last element is what the NPC will respond to the rest is context.
        // Optional parameters
        public int max_completion_tokens; // The maximum number of tokens to generate.
    }
}

