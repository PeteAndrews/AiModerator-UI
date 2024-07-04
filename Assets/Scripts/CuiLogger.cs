using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CuiLogger : MonoBehaviour
{
    // Path to save the text file
    private StreamWriter fileWriter;
    private string filePath;
    public string saveFolder;
    public string saveName;
    void Start()
    {
        string directoryPath = Path.Combine(Application.dataPath, saveFolder);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(directoryPath, saveName);

        fileWriter = new StreamWriter(filePath, true);
        RetrievePushData.Instance.OnGptEvent += HandleNewData;
        DialogueEventLoader.Instance.OnSendEventData += HandleDialogueEvent;
    }
    private void WriteToFile(string data)
    {
        fileWriter.WriteLine(data);
        fileWriter.Flush();
    }
    private void HandleDialogueEvent(EventData eventData)
    {
        WriteToFile($"{eventData.Summary}\n\n");
    }
    private void HandleNewData(string eventName, string eventData, string functionName, string articleNames, bool isHyperText)
    {
        string data = $"{functionName}: {eventData}\n\n";
        fileWriter.WriteLine(data);
    }

    void OnDestroy()
    {
        RetrievePushData.Instance.OnGptEvent -= HandleNewData;
        DialogueEventLoader.Instance.OnSendEventData -= HandleDialogueEvent;
        if (fileWriter != null)
        {
            fileWriter.Close();
        }
    }
}
