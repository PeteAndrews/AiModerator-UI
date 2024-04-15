using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using NetMQ;
using NetMQ.Sockets;

public class UnityClientSender : MonoBehaviour
{
    public UnityClientSender Instance { get; private set; }
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
    }
    private void HandleSendEventData(EventData eventData)
    {
        //Serialise the eventData object to JSON
        string jsonData = JsonConvert.SerializeObject(eventData);
        AsyncIO.ForceDotNet.Force();
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
    void OnDestroy()
    {
        if (DialogueEventLoader.Instance != null)
        {
            DialogueEventLoader.Instance.OnSendEventData -= HandleSendEventData;
        }
        NetMQConfig.Cleanup();

    }
}
