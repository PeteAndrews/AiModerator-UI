using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using NetMQ;
using NetMQ.Sockets;

public class RequestEventData
{
    public string EventName { get; set; }
    public string Option { get; set; }  
    public string Mode { get; set; }
    public string Party { get; set; }
    public string Choice { get; set; }
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
public class UnityClientSender : MonoBehaviour
{
    private static UnityClientSender _instance;
    public static UnityClientSender Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("UnityClientSender");
                _instance = go.AddComponent<UnityClientSender>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private bool isWaitingForResponse = false;
    public string mode;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SubscribeEvents();
        Debug.Log("Unity client Start Set Mode: " + mode);
    }

    private void SubscribeEvents()
    {
        DialogueEventLoader.Instance.OnSendEventData += HandleEvent;
        RetrievePushData.Instance.OnSetEvent += OnSetResponseReceived;
    }

    public void SendEventRequest(RequestEventData requestEventData)
    {
        requestEventData.Mode = mode;
        StartCoroutine(HandleEventRequest(requestEventData));
    }

    private IEnumerator HandleEventRequest(RequestEventData requestEventData)
    {
        yield return SendEventAndWaitForResponse("Set Event", requestEventData.Option);
        SendEvent(requestEventData.ToJson());
    }

    private IEnumerator SendEventAndWaitForResponse(string eventName, string option)
    {
        RequestEventData data = new RequestEventData { EventName = eventName, Option = option };
        SendEvent(data.ToJson());
        isWaitingForResponse = true;
        while (isWaitingForResponse)
            yield return null;
    }

    public void OnSetResponseReceived()
    {
        isWaitingForResponse = false;
    }

    private void SendEvent(string jsonData)
    {
        using (var requestSocket = new RequestSocket(">tcp://localhost:5556"))
        {
            requestSocket.SendFrame(jsonData);
            Debug.Log("Message sent: " + jsonData);
            string message = requestSocket.ReceiveFrameString();
            Debug.Log("Message received: " + message);
        }
    }

    private void HandleEvent(EventData eventData)
    {
        AsyncIO.ForceDotNet.Force();
        SendEvent(JsonConvert.SerializeObject(eventData));
    }

    void OnDestroy()
    {
        if (DialogueEventLoader.Instance != null)
        {
            DialogueEventLoader.Instance.OnSendEventData -= HandleEvent;
        }
        NetMQConfig.Cleanup();
    }
}