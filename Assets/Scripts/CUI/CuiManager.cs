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

    [SerializeField] TabManager tabManager;
    [SerializeField] ChatManager chatManager;
    [SerializeField] FunctionButtonManager functionButtonManager;
    private EventData currentEventData;

    private void OnEnable()
    {
        //RetrievePushData.Instance.OnOptionsEvent += HandlePythonOptionsInput;
        RetrievePushData.Instance.OnGptEvent += HandlePythonGptInput;
        RetrievePushData.Instance.OnMoreInfoResponseEvent += HandleMoreInfoKeywordResponse;
        DialogueEventLoader.Instance.OnSendEventData += HandleDialogueEvent;

    }
    private void OnDisable()
    {
        //RetrievePushData.Instance.OnOptionsEvent -= HandlePythonOptionsInput;
        RetrievePushData.Instance.OnGptEvent -= HandlePythonGptInput;
        RetrievePushData.Instance.OnMoreInfoResponseEvent -= HandleMoreInfoKeywordResponse;
        DialogueEventLoader.Instance.OnSendEventData -= HandleDialogueEvent;

    }

    private void Start()
    {
        //tabManager.Initialize();
        chatManager.Initialize(tabManager);
        //optionButtonManager.Initialize(tabManager);
    }

    private void HandleDialogueEvent(EventData eventData)
    {
        //store event data
        currentEventData = eventData;
        //set active buttons                                    --NEXT
        functionButtonManager.ActivateFunctionButtons(eventData.Candidate);
        //Add event summary to banner.
        //throw new NotImplementedException();

    }
    public void HandleFunctionButtonEvent(string eventName, string candidateName)
    {
        Debug.Log("CuiManager Notified, Notifying TabManager");
        if (eventName != "more info")
        {
            tabManager.SwitchTab(eventName, candidateName, true);
        }
        else
        {
            UnityClientSender.Instance.SendMoreInfoRequest();
        }

        //throw new NotImplementedException();
    }
    private void HandlePythonGptInput(string eventName, string text, string functionName, string articleName, bool isHyperText)
    {
        //chatManager.UpdateChat(eventName, text, functionName, articleName, isHyperText);
        //spawna and activate tab
        //want to check is the tab under the same name already exists
        bool tabExists = tabManager.CheckTabExists(functionName);
        if (!tabExists)
        {
            tabManager.SwitchTab(functionName, currentEventData.Candidate, true);
        }
        chatManager.UpdateActiveChat(text, functionName, true, isHyperText);
    }
    public void HandleManifestoActivation()
    {
        //summarize the event infomation and give the user option of consulting the manifesto
        chatManager.AddNonInteractiveMessage(DialogueEventLoader.Instance.currentEventData.ResponseViewpoint);
        chatManager.AddManifestoSelectMessage();
    }

    public void HandleAction(string actionType, string linkText)
    {
        switch (actionType)
        {
            case "manifesto": UnityClientSender.Instance.SendManifestoEvent(linkText);break;
            case "more info": UnityClientSender.Instance.MoreInfoEvent(linkText); break;
            case "opinion": UnityClientSender.Instance.SendEventNoResponse("Select Event", actionType);break;
            case "continue":  UnityClientSender.Instance.SendEventContinueInteraction("Continue Event", linkText);break;
        }
        tabManager.DestroyActiveTab();
    }
    public void HandleDestroyActiveTab(string tabName, string candidateName)
    {
        functionButtonManager.DeactivateFunctionButton(tabName, candidateName);
    }
    public void HandleMoreInfoKeywordResponse(string eventName, string data)
    {
        //Being called constantly
        tabManager.SwitchTab("more info", currentEventData.Candidate, true);
        chatManager.UpdateActiveChat(data, "more info", true, true);
    }
    /*
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
    }*/
}
