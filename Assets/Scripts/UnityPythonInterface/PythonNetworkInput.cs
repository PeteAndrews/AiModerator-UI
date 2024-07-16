using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using NetMQ;
using System;
using AsyncIO;
using Newtonsoft.Json;
using System.Linq;
using NetMQ.Sockets;
using TMPro;

[Serializable]
public class PythonServerDataContainer
{
    public string eventName;
    public string eventData;
    public string articleNames;
    public string functionName;
    public bool isHyperText=false;
    public List<byte[]> imageData;
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

    public delegate void GptEventHandler(string eventName, string text, string functionName, string articleName, bool isHyperText);
    public event GptEventHandler OnGptEvent;
    public delegate void MoreInfoResponseHandler(string eventName, string data);
    public event MoreInfoResponseHandler OnMoreInfoResponseEvent;
    public delegate void SetEventHandler();
    public event SetEventHandler OnSetEvent;
    public delegate void ImageReceivedHandler(List<Texture2D> texture);
    public event ImageReceivedHandler OnImageReceieved; 

    public RetrievePushData()
    {
        thread = new Thread(ThreadFunction);
    }
    private void ThreadFunction(object callbackObject)
    {
        Action<PythonServerDataContainer> callback = callbackObject as Action<PythonServerDataContainer>;
        if (callback == null)
        {
            throw new InvalidOperationException("Callback must be an Action<PythonServerDataContainer>.");
        }

        using (var socket = new PullSocket())
        {
            //socket.Connect(f"tcp://{10.0.0.7}:{8888}");
            //socket.Connect("tcp://localhost:5555");
            socket.Connect($"tcp://{NetworkSettings.Instance.serverIP}:{NetworkSettings.Instance.pushPort}");
            try
            {
                while (streaming)
                {
                    var msg = new NetMQMessage();
                    if (socket.TryReceiveMultipartMessage(ref msg) && msg.Count() > 0) 
                    {
                        var input = msg[0].ConvertToString(); 
                        PythonServerDataContainer data = JsonConvert.DeserializeObject<PythonServerDataContainer>(input);
                        if (msg.Count() > 1)
                        {
                            List<byte[]> images = new List<byte[]>();
                            for (int i = 1; i < msg.Count(); i++)
                            {
                                images.Add(msg[i].Buffer);
                            }
                            data.imageData = images; 
                        }
                        else
                        {
                            data.imageData = new List<byte[]>(); 
                        }

                        HandleInputEvent(data);
                        callback(data);
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

    private List<Texture2D> HandleBytesToTexture(List<byte[]> images)
    {
        List<Texture2D> textures = new List<Texture2D>();
        foreach (byte[] image in images)
        {
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(image); 
            textures.Add(texture);
        }
        return textures;
    }

    private void HandleInputEvent(PythonServerDataContainer data)
    {

        switch (data.eventName)
        {
            case "More Info Response Event":
                MainThreadDispatcher.Enqueue(() => OnMoreInfoResponseEvent?.Invoke(data.eventName, data.eventData));
                break;
            case "Set Response Event":
                MainThreadDispatcher.Enqueue(() => OnSetEvent?.Invoke());
                break;
            case "Gpt Response Event":
                MainThreadDispatcher.Enqueue(() => OnGptEvent?.Invoke(data.eventName, data.eventData, data.functionName, data.articleNames, data.isHyperText));
                break;
            case "Image Retrieval Event":
                MainThreadDispatcher.Enqueue(() =>
                {
                    List<Texture2D> textures = HandleBytesToTexture(data.imageData);
                    OnImageReceieved?.Invoke(textures); 
                });
                break;
            default:
                MainThreadDispatcher.Enqueue(() => OnGptEvent?.Invoke(data.eventName, data.eventData, data.functionName, data.articleNames, data.isHyperText));
                break;
        }
    }
    public void Start(Action<PythonServerDataContainer> callback)
    {
        if (callback == null) throw new ArgumentNullException(nameof(callback));

        streaming = true;
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

    void Start()
    {
        ForceDotNet.Force();
        retrieval = RetrievePushData.Instance;
        retrieval.Start((PythonServerDataContainer data) => runOnMainThread.Enqueue(() =>
        {
            Debug.Log($"Received event: {data.eventName}, with data: {data.eventData}");
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
