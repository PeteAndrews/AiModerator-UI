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

public class RetrievePushData
{
    private readonly Thread thread;
    private bool streaming;
    private NetworkOutput bboxDetails;
    private NetworkOutput colourDetails;
    private NetworkOutput hDetails;
    private List<string> objectIds;

    public RetrievePushData()
    {
        thread = new Thread((object callback) =>
        {
            using (var socket = new NetMQ.Sockets.PullSocket())
            {
                socket.Connect("tcp://localhost:5505");
                while (streaming)
                {
                    byte[] byteImage = socket.ReceiveFrameBytes();
                    string netOut_details = socket.ReceiveFrameString();
                    byte[] bboxes = socket.ReceiveFrameBytes();
                    string ids = socket.ReceiveFrameString();
                    int frame = Int32.Parse(socket.ReceiveFrameString());
                    string colour_details = socket.ReceiveFrameString();
                    byte[] colours = socket.ReceiveFrameBytes();
                    string H_details = socket.ReceiveFrameString();
                    byte[] H = socket.ReceiveFrameBytes();

                    bboxDetails = JsonConvert.DeserializeObject<NetworkOutput>(netOut_details);
                    colourDetails = JsonConvert.DeserializeObject<NetworkOutput>(colour_details);
                    hDetails = JsonConvert.DeserializeObject<NetworkOutput>(H_details);

                    objectIds = JsonConvert.DeserializeObject<List<String>>(ids);
                    int[] bbint = ByteToInt(bboxes, bboxDetails.dimensions[0], bboxDetails.dimensions[1]);
                    int[][] bbOut = TransformBboxes(bbint);
                    int[] coloursOut = ByteToInt(colours, colourDetails.dimensions[0], colourDetails.dimensions[1]);
                    float[] Homography = ByteToFloat(H, hDetails.dimensions[0], hDetails.dimensions[1]);
                    
                    ((Action<byte[], int[][], List<string>, int[], float[], int>)callback)(byteImage, bbOut, objectIds, coloursOut, Homography, frame);
                }
            }
        });
    }
    private int[] ByteToInt(byte[] input, int width, int height)
    {
        int[] output = new int[input.Length];
        Buffer.BlockCopy(input, 0, output, 0, input.Length);
        return output[0..(width * height)];
    }
    private float[] ByteToFloat(byte[] input, int width, int height)
    {
        var output = new float[input.Length/4];
        Buffer.BlockCopy(input, 0, output, 0, input.Length);
        return output;

    }
    private int[][] TransformBboxes(int[] bboxes, int length = 4)
    {
        int i = 0;
        int[][] transformed_bbs = bboxes.GroupBy(b=> i++ / 4).Select(n => n.ToArray()).ToArray();
        return transformed_bbs;
    }
    public void Start(Action<byte[], int[][], List<string>, int[], float[], int> callback)
    {
        streaming = true;
        thread.Start(callback);
    }
    public void Stop()
    {
        streaming = false;
        thread.Join();  
    }

}


public class SocketClient : MonoBehaviour
{
    private readonly System.Collections.Concurrent.ConcurrentQueue<Action> runOnMainThread = new System.Collections.Concurrent.ConcurrentQueue<Action>();
    private RetrievePushData retrieval;
    public Texture2D mainTexture;
    [SerializeField] protected Renderer mainRenderer;
    [SerializeField] protected Renderer birdseyeRenderer;
    public NetworkOutput networkOut;
    [SerializeField] public List<String> objectIds;
    [SerializeField] public int[][] boundingBoxes;
    [SerializeField] public int[] colours;
    public int f;
    public float[] H;
    // Start is called before the first frame update
    private void OnEnable()
    {
       

    }
    private void OnDisable()
    {
       
    }
    void Start()
    {
        mainTexture = new Texture2D(2, 2);
        mainRenderer.material.SetTexture("_MainTex", mainTexture);
        ForceDotNet.Force();
        retrieval = new RetrievePushData();
        retrieval.Start((byte[] byteImage, int[][]bboxes, List<string> objIds, int[] coloursOut,    float[] Homography, int frame)=> runOnMainThread.Enqueue(()=>
        {
            mainTexture.LoadImage(byteImage);
            objectIds = objIds;
            boundingBoxes = bboxes;
            colours = coloursOut;
            H = Homography;
            f = frame;
        }));
    }

    void Update()
    {
        if (!runOnMainThread.IsEmpty)
        {
            Action action;
            while(runOnMainThread.TryDequeue(out action))
            {

                action.Invoke();
            }
        }

    }
    private void OnDestroy()
    {
        retrieval.Stop();
        NetMQConfig.Cleanup();
    }

}
