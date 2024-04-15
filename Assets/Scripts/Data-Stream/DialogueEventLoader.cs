using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class EventData
{
    public string EventName = "System Event";
    public int Iteration;
    public string Time;
    public float SimilarityScore;
    public bool IsSimilar;
    public string ResponseTopic;
    public string ResponseViewpoint;
    public string ResponseAction;
    public string CounterArgument;
    public string Defense;
    public string Attack;
    public string Blame;
    public string PersonalAttack;
    public string Deflection;
    public string EventWords;
    public string Keywords;
    public string SearchQueries;
}

[System.Serializable]
public class EventList
{
    public List<EventData> events;
}

public class DialogueEventLoader : MonoBehaviour
{
    public static DialogueEventLoader Instance { get; private set; }
    public delegate void SendEventDataHandler(EventData eventData);
    public event SendEventDataHandler OnSendEventData;
    [SerializeField] protected float eventCheckInterval = 0.5f;
    public string dialogueEventPath;
    private Dictionary<float, EventData> eventsDictionary = new Dictionary<float, EventData>();
    private List<float> sortedEventTimes;
   
    protected GlobalTimer globalTimer;


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
        globalTimer = GlobalTimer._instance;
        if (globalTimer == null)
        {
            Debug.LogError("globalTimer is null in DialogueEventLoader Awake()");
        }

    }

    void Start()
    {
        // get gloabl timer from same gameobject
        //globalTimer = GetComponent<GlobalTimer>();
        LoadEventData();
        sortedEventTimes = eventsDictionary.Keys.OrderBy(t => t).ToList();
        StartCoroutine(CheckEventsCoroutine());

    }

    private IEnumerator CheckEventsCoroutine()
    {
        while (true) // Infinite loop to continuously check for events
        {
            if (sortedEventTimes.Count > 0 && globalTimer.Time >= sortedEventTimes[0])
            {
                float eventTime = sortedEventTimes[0];
                if (eventsDictionary.TryGetValue(eventTime, out EventData eventData))
                {
                    Debug.Log("Raised Event: " + eventData.ResponseTopic);
                    // raise event here to notify socket server to send data to python server
                    OnSendEventData?.Invoke(eventData);

                    // Remove the event after processing
                    eventsDictionary.Remove(eventTime);
                    sortedEventTimes.RemoveAt(0);
                }
            }

            // Wait for a specified interval before checking again
            yield return new WaitForSeconds(eventCheckInterval); // Check every 0.5 seconds, adjust as needed
        }
    }
    private void LoadEventData()
    {
        // Load the data from the file
        string filePath = Path.Combine(Application.dataPath, dialogueEventPath);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            EventList loadedData = JsonUtility.FromJson<EventList>("{\"events\":" + dataAsJson + "}");
            foreach (var eventData in loadedData.events)
            {
                float time = ConvertTimeStringToSeconds(eventData.Time);
                eventsDictionary.Add(time, eventData);
            }
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }
    }

    // Example method to retrieve data by timestamp
    private EventData GetEventData(float timestamp)
    {
        if (eventsDictionary.ContainsKey(timestamp))
        {
            return eventsDictionary[timestamp];
        }
        return null; // Or handle the case where the timestamp isn't found
    }
    private float ConvertTimeStringToSeconds(string timeStr)
    {
        /* 
         ** The time string is in the format "HH:MM:SS,MS" where:
         ** HH is hours
         ** MM is minutes
         ** SS is seconds
         * MS is milliseconds
         ** 
         ** This method will convert the time string to total seconds
         **/
        // Split the string into components
        string[] parts = timeStr.Split(':', ',');
        if (parts.Length < 3) return -1; // Basic validation

        // Parse the parts into integers
        int hours = int.Parse(parts[0]);
        int minutes = int.Parse(parts[1]);
        int seconds = int.Parse(parts[2]);
        int milliseconds = int.Parse(parts[3]);

        // Calculate total seconds
        float totalSeconds = (hours * 3600) + (minutes * 60) + seconds + (milliseconds / 1000f);
        return totalSeconds;
    }
}
