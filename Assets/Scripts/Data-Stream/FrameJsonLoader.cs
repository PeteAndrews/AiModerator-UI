using System;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class FrameJsonLoader : MonoBehaviour
{
    
    public string jsonFolderPath = "/track_data/security/";

    public FrameData LoadFrameData(int frameNumber)
    {
       string frameNumberString = frameNumber.ToString("D6");
       //string jsonFilePath = Path.Combine(jsonFolderPath, "frame_" + frameNumberString + ".json");
       string jsonFilePath = Path.Combine(jsonFolderPath, "frame_" + frameNumberString);
       TextAsset jsonData = Resources.Load<TextAsset>(jsonFilePath.TrimStart('/'));
       if (jsonData != null)
       {
           //FrameData frameData = JsonUtility.FromJson<FrameData>(jsonData.text);
           FrameData frameData = JsonConvert.DeserializeObject<FrameData>(jsonData.text);

           return frameData;
       }

       else
       {
            return null;
       }
     }

}
