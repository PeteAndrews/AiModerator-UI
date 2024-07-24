using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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
    public string Summary;
    public string Candidate;
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
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
    public EventData currentEventData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
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
        LoadEventData();
        sortedEventTimes = eventsDictionary.Keys.OrderBy(t => t).ToList();
        StartCoroutine(CheckEventsCoroutine());
    }

    private IEnumerator CheckEventsCoroutine()
    {
        while (true)
        {
            if (sortedEventTimes.Count > 0 && globalTimer.Time >= sortedEventTimes[0])
            {
                float eventTime = sortedEventTimes[0];
                if (eventsDictionary.TryGetValue(eventTime, out EventData eventData))
                {
                    currentEventData = eventData;
                    OnSendEventData?.Invoke(eventData);

                    eventsDictionary.Remove(eventTime);
                    sortedEventTimes.RemoveAt(0);
                }
            }
            yield return new WaitForSeconds(eventCheckInterval);
        }
    }

    private void LoadEventData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>(dialogueEventPath.TrimStart('/'));
        if (jsonData != null)
        {
            EventList loadedData = JsonUtility.FromJson<EventList>("{\"events\":" + jsonData.text + "}");
            foreach (var eventData in loadedData.events)
            {
                float time = ConvertTimeStringToSeconds(eventData.Time);
                eventsDictionary.Add(time, eventData);
            }
        }
        else
        {
            Debug.LogError("Failed to load JSON data from Resources.");
        }
    }

    private float ConvertTimeStringToSeconds(string timeStr)
    {
        string[] parts = timeStr.Split(':', ',');
        if (parts.Length < 3) return -1;

        int hours = int.Parse(parts[0]);
        int minutes = int.Parse(parts[1]);
        int seconds = int.Parse(parts[2]);
        int milliseconds = parts.Length > 3 ? int.Parse(parts[3]) : 0;

        float totalSeconds = (hours * 3600) + (minutes * 60) + seconds + (milliseconds / 1000f);
        return totalSeconds;
    }
}
