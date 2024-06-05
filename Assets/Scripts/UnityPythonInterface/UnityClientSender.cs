using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using NetMQ;
using NetMQ.Sockets;

public class UnityClientSender : MonoBehaviour
{
    public static UnityClientSender Instance { get; private set; }
    private bool isWaitingForResponse = false;
    public string mode;


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
    // Start is called before the first frame update
    void Start()
    {
        if (DialogueEventLoader.Instance != null)
        {
            DialogueEventLoader.Instance.OnSendEventData += HandleSendEventData;
        }
        if(RetrievePushData.Instance != null)
        {
            RetrievePushData.Instance.OnSetEvent += OnSetResponseReceived;
        }

    }
    public void SendMoreInfoRequest()
    {
        StartCoroutine(HandleInfoButtonClick("more info"));

    }

    public void ReceiveButtonName(string buttonName)
    {
        StartCoroutine(HandleButtonClick(buttonName));

    }
    private IEnumerator HandleInfoButtonClick(string buttonName)
    {
        yield return SendEventAndWaitForResponse("Set Event", buttonName);
        Debug.Log("Send More Info Request");
        var data = new
        {
            EventName = "More Info Request Event"
        };
        string jsonData = JsonConvert.SerializeObject(data);
        SendEvent(jsonData);

    }
    private IEnumerator HandleButtonClick(string buttonName)
    {
        // Send the set event
        yield return SendEventAndWaitForResponse("Set Event", buttonName);
        Debug.Log("Received response for set event, proceeding with select event.");

        var data = new
        {
            EventName = "Select Event",
            Option = buttonName,
        };
        string jsonData = JsonConvert.SerializeObject(data);
        // Once the response is received, send the select event
        SendEvent(jsonData);

    }

    private IEnumerator SendEventAndWaitForResponse(string eventName, string option)
    {
        Debug.Log($"Sending {eventName} and waiting for response...");

        var data = new
        {
            EventName = eventName,
            Option = option
        };
        string jsonData = JsonConvert.SerializeObject(data);
        SendEvent(jsonData);

        isWaitingForResponse = true;

        // Wait until the response is received
        while (isWaitingForResponse)
        {
            yield return null; // Wait for one frame
        }

        Debug.Log($"Response received for {eventName}, continuing...");
    }

    public void OnSetResponseReceived()
    {
        Debug.Log("Set response received, allowing continuation.");
        isWaitingForResponse = false;
    }
    public void MoreInfoEvent(string moreInfoData)
    {
        var data = new
        {
            EventName = "More Info Event",
            Option = "more info",
            Keyword = moreInfoData
        };
        string jsonData = JsonConvert.SerializeObject(data);
        SendEvent(jsonData);
    }

    public void SendEventNoResponse(string eventName, string option)
    {
        var data = new
        {
            EventName = eventName,
            Option = option
        };
        string jsonData = JsonConvert.SerializeObject(data);
        SendEvent(jsonData);
    }
    public void SendEventContinueInteraction(string eventName, string userInput)
    {
        var data = new
        {
            EventName = eventName,
            UserInput = userInput,
            Mode = mode
        };
        string jsonData = JsonConvert.SerializeObject(data);
        SendEvent(jsonData);
    }
    public void SendManifestoEvent(string party)
    {
        var data = new
        {
            EventName = "Manifesto Event",
            UserInput = party,
            Mode = mode
        };
        string jsonData = JsonConvert.SerializeObject(data);
        SendEvent(jsonData);
    }
    private void SendEvent(string data)
    {
        //need to send over event name
        using (var requestSocket = new RequestSocket(">tcp://localhost:5556"))
        {
            requestSocket.SendFrame(data);
            Debug.Log("Message sent: " + data);
            string message = requestSocket.ReceiveFrameString();
            Debug.Log("Message received: " + message);

        }
    }
    private void HandleSendEventData(EventData eventData)//HandleSendSystemEventData
    {
        //Serialise the eventData object to JSON
        string jsonData = JsonConvert.SerializeObject(eventData);
        AsyncIO.ForceDotNet.Force();
        SendEvent(jsonData);

    }



    void OnDestroy()
    {
        if (DialogueEventLoader.Instance != null)
        {
            DialogueEventLoader.Instance.OnSendEventData -= HandleSendEventData;
        }
        NetMQConfig.Cleanup();

    }
}
