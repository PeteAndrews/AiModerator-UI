using System;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class FrameJsonLoader : MonoBehaviour
{
    
    public string jsonFolderPath = "Assets/data/track_data";

    void Update()
    {
    }
    
    
    public FrameData LoadFrameData(int frameNumber)
    {
        string frameNumberString = frameNumber.ToString("D4");
        string jsonFilePath = Path.Combine(jsonFolderPath, "frame_" + frameNumberString + ".json");
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            FrameData frameData = JsonConvert.DeserializeObject<FrameData>(json);
            return frameData;
        }
        
        return null;
        
    }
}