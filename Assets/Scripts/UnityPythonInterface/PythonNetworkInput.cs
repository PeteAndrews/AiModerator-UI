using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using NetMQ;
using System;
using AsyncIO;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using NetMQ.Sockets;

[Serializable]
public class PythonServerDataContainer
{
    public string eventName;
    public string eventData;
    public string articleNames;
    public string functionName;
    public bool isHyperText=false;
}

public class RetrievePushData
{
    private static RetrievePushData _instance;
    public static RetrievePushData Instance
    {
        get
        {
            _instance ??= new RetrievePushData();
            return _instance;
        }
    }

    private readonly Thread thread;
    private volatile bool streaming;


//    public delegate void OptionsEventHandler(string eventName, string[] text);
//    public event OptionsEventHandler OnOptionsEvent;
    public delegate void GptEventHandler(string eventName, string text, string functionName, string articleName, bool isHyperText);
    public event GptEventHandler OnGptEvent;
    public delegate void MoreInfoResponseHandler(string eventName, string data);
    public event MoreInfoResponseHandler OnMoreInfoResponseEvent;
    public delegate void SetEventHandler();
    public event SetEventHandler OnSetEvent;
    //will need another event for the gpt events

    public RetrievePushData()
    {
        // This no longer directly takes a callback. The callback is passed when starting the thread.
        thread = new Thread(ThreadFunction);
    }

    private void ThreadFunction(object callbackObject)
    {
        Debug.Log("ThreadFunction started");

        Action<string, string> callback = callbackObject as Action<string, string>;
        if (callback == null)
        {
            throw new InvalidOperationException("Callback must be an Action<string, string>.");
        }

        using (var socket = new PullSocket())
        {
            socket.Connect("tcp://localhost:5555");
            try
            {
                while (streaming)
                {
                    string input = socket.ReceiveFrameString();
                    PythonServerDataContainer data = JsonConvert.DeserializeObject<PythonServerDataContainer>(input);//make sure container can retain information from gpt event as well
                    HandleInputEvent(data);
                    callback(data.eventName, data.eventData);
                    try
                    {
                        callback(data.eventName, data.eventData);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Exception in callback: " + ex.Message);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception in while (streaming) loop: " + ex.Message);
            }
        }

        Debug.Log("ThreadFunction ended");
    }


    private void HandleInputEvent(PythonServerDataContainer data)
    {

        if(data.eventName == "More Info Response Event"){
            MainThreadDispatcher.Enqueue(() => OnMoreInfoResponseEvent?.Invoke(data.eventName, data.eventData));
        }
        else if(data.eventName == "Set Response Event")
        {
            MainThreadDispatcher.Enqueue(() => OnSetEvent?.Invoke());
        }
        else
        {
            MainThreadDispatcher.Enqueue(() => OnGptEvent?.Invoke(data.eventName, data.eventData, data.functionName, data.articleNames, data.isHyperText));
        }
    }
    private string[] splitText(string text)
    {
        string[] splitText = text.Split(',');
        return splitText;
    }
    public void Start(Action<string, string> callback)
    {
        if (callback == null) throw new ArgumentNullException(nameof(callback));

        streaming = true;
        // Start the thread and pass the callback as an argument.
        thread.Start(callback);
    }

    public void Stop()
    {
        streaming = false;
        if (thread.IsAlive)
        {
            thread.Join();
        }
    }
}


public class PythonNetworkInput : MonoBehaviour
{
    private readonly System.Collections.Concurrent.ConcurrentQueue<Action> runOnMainThread = new System.Collections.Concurrent.ConcurrentQueue<Action>();
    private RetrievePushData retrieval;
    public PythonNetworkDataContainer networkOut;
    [SerializeField] public string aim_response;
    // Start is called before the first frame update
    private void OnEnable()
    {


    }
    private void OnDisable()
    {

    }
    void Start()
    {
        ForceDotNet.Force();
        retrieval = RetrievePushData.Instance;
        retrieval.Start((string eventName, string eventData) => runOnMainThread.Enqueue(() =>
        {
            Debug.Log($"Received event: {eventName}, with data: {eventData}");
        }));
    }

    void Update()
    {
        if (!runOnMainThread.IsEmpty)
        {
            Action action;
            while (runOnMainThread.TryDequeue(out action))
            {

                action.Invoke();
            }
        }

    }
    private void OnDestroy()
    {
        Debug.Log("OnDestroy called");

        retrieval.Stop();
        NetMQConfig.Cleanup();
    }

}
