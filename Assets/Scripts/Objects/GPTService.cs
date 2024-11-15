using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GPTService
{
    public string apiKey = "APIKEY";
    public string api_url = "https://api.openai.com/v1/chat/completions";
    public string playerInput;
    public string prompt, response, request, playerData, NPCData;
    public List<Message> messages = new List<Message>();

    public string GeneratePrompt() // This will be passed playerInput, playerData, and NPCData in the future.
    {
        string prompt = "You are an NPC named Bob the Baker and you should respond to the players input accordingly. Respond with concise realistic messages. Do not break character.";
        return prompt;
    }

    public IEnumerator apiCall()
    {
        Debug.Log("Starting API Call");
        prompt = GeneratePrompt();
        messages.Add(new Message { role = "user", content = prompt });
        Debug.Log("Sending prompt to GPT: " + prompt);

        Headers headers = new Headers
        {
            Authorization = "Bearer " + apiKey,
            ContentType = "application/json"
        };

        RequestBody requestBody = new RequestBody
        {
            model = "gpt-4o",
            messages = messages.ToArray(),
            max_tokens = 50
        };

        // Convert the headers and request body to JSON
        string jsonHeaders = JsonUtility.ToJson(headers);
        Debug.Log("Headers: " + jsonHeaders);
        string jsonBody = JsonUtility.ToJson(requestBody);
        Debug.Log("Request Body: " + jsonBody);
        UnityWebRequest request = new UnityWebRequest(api_url, "POST");
        // Convert the body to a byte array.
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        request.SetRequestHeader("Content-Type", "application/json");

        // Recieves four responses. ????
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            response = request.downloadHandler.text;
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
        // Optional parameters coming soon.
        public int max_tokens; // The maximum number of tokens to generate. Requests can use up to 4096 tokens.
    }
}

