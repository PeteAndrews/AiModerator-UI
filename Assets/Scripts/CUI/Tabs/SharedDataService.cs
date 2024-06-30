using System.Collections.Generic;

public class SharedDataService
{
    public static SharedDataService Instance { get; } = new SharedDataService();

    private Dictionary<string, string> responses = new Dictionary<string, string>();

    public void SetResponse(string key, string response)
    {
        responses[key] = response;
    }

    public string GetResponse(string key)
    {
        responses.TryGetValue(key, out var response);
        return response;
    }

    public void ClearResponse(string key)
    {
        if (responses.ContainsKey(key))
        {
            responses.Remove(key);
        }
    }
}

