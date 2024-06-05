using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Text;

public class CuiManager : MonoBehaviour
{
    public static CuiManager Instance { get; private set; }
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
    //public TabConfig cuiConfig; 
    //public Transform cuiParent;
    //public Transform buttonsParent;
    //private Dictionary<TabButtonPair, TabInfo> aliveTabInfos = new Dictionary<TabButtonPair, TabInfo>();
    //public GameObject activeTab;
    //private CuiScrollView activeChat;
    //private float cumulativeButtonWidth = 0f;
    //[SerializeField] private RectTransform buttonsParentRect;
    // Use a dictionary to keep track of instantiated tabs and buttons

    [SerializeField] TabManager tabManager;
    [SerializeField] ChatManager chatManager;
    [SerializeField] OptionButtonManager optionButtonManager;

    private void OnEnable()
    {
        RetrievePushData.Instance.OnOptionsEvent += HandlePythonOptionsInput;
        RetrievePushData.Instance.OnGptEvent += HandlePythonGptInput;
        RetrievePushData.Instance.OnMoreInfoResponseEvent += HandleMoreInfoResponse;
    }
    private void OnDisable()
    {
        RetrievePushData.Instance.OnOptionsEvent -= HandlePythonOptionsInput;
        RetrievePushData.Instance.OnGptEvent -= HandlePythonGptInput;
        RetrievePushData.Instance.OnMoreInfoResponseEvent -= HandleMoreInfoResponse;
    }


    private void Start()
    {
        tabManager.Initialize();
        chatManager.Initialize(tabManager);
        optionButtonManager.Initialize(tabManager);
    }

    private void HandlePythonGptInput(string eventName, string text, string functionName, string articleName, bool isHyperText)
    {
        //chatManager.UpdateChat(eventName, text, functionName, articleName, isHyperText);
        chatManager.UpdateActiveChat(text, functionName, true, isHyperText);
    }

    private void HandlePythonOptionsInput(string eventName, string[] tabNames)
    {
        tabManager.OnOptionsEvent(tabNames);
    }
    private void HandleMoreInfoResponse(string eventName, string text)
    {
        //chatManager.MoreInfoToHyperLinks(text);
        chatManager.AddInteractiveHyperLinkMessage(text, "more info");
    }
    public void HandleManifestoActivation()
    {
        //summarize the event infomation and give the user option of consulting the manifesto
        tabManager.activeChat.AddNonInteractiveMessage(DateTime.Now.ToString("HH:mm:ss"), DialogueEventLoader.Instance.currentEventData.ResponseViewpoint, true);
        chatManager.ManifestoActivationMessage();
    }
}
