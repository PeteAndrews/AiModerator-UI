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
    public static UnityClientSender Instance { get; private set; }
    private void Awake()
    {
        Debug.Log("Awake called on UnityClientSender with instance ID: " + GetInstanceID());
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            Debug.Log("Set as singleton instance: " + GetInstanceID());
        }
        else
        {
            Debug.Log("Destroying duplicate UnityClientSender instance: " + GetInstanceID());
            Destroy(gameObject);
        }
    }

    private bool isWaitingForResponse = false;
    public string mode;

    void Start()
    {
        SubscribeEvents();
        Debug.Log("Unity client Start Set Mode: " + mode);

    }

    private void SubscribeEvents()
    {
        //DialogueEventLoader.Instance.OnSendEventData += HandleEvent;
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
    public void ImageRequestAndDataSet(EventData eventData)
    {
        Debug.Log("Image Request and Data Set called on UnityClientSender with instance ID: " + GetInstanceID());
        SendEvent(eventData.ToJson());
    }
    public void OnSetResponseReceived()
    {
        isWaitingForResponse = false;
    }

    private void SendEvent(string jsonData)
    {
        try
        {
            //using (var requestSocket = new RequestSocket(">tcp://10.0.0.7:8989"))
            //using (var requestSocket = new RequestSocket("tcp://localhost:5556"))
            using (var requestSocket = new RequestSocket($">tcp://{NetworkSettings.Instance.serverIP}:{NetworkSettings.Instance.repPort}"))
            {
                requestSocket.SendFrame(jsonData);
                string message = requestSocket.ReceiveFrameString();
            }
        }
        catch (NetMQException ex)
        {
            Debug.LogError("NetMQException: " + ex);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Exception: " + ex);
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