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
using System.Collections;

public class CuiManager : MonoBehaviour
{
    public static CuiManager Instance { get; private set; }
    public UserController userController;
    public HeadlineBanner headlineBanner;
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
    [SerializeField] DepthTextManager depthTextManager;
    public string mode;
    private EventData currentEventData;
    public event Action<string> OnMoreInfoSelected;
    public event Action<string> OnOpinionSelected;
    public event Action<string> OnManifestoSelected;
    public event Action<string> OnFollowUpSelected;
    private bool haveImage = false;
    private List<Texture2D> bannerImages;

    private void OnEnable()
    {
        DialogueEventLoader.Instance.OnSendEventData += HandleDialogueEvent;
        RetrievePushData.Instance.OnImageReceieved += HandleImageEvent;
        UserController.Instance.OnSwipeEvent += HandleSwipeEvent;
        UserController.Instance.OnDoubleTapEvent += HandleDoubleTapEvent;
        UserController.Instance.OnSingleTapEvent += HandleSingleTapEvent;
        UserController.Instance.OnPinchZoomEvent += HandlePinchZoomEvent;
    }
    private void OnDisable()
    {
        DialogueEventLoader.Instance.OnSendEventData -= HandleDialogueEvent;
        RetrievePushData.Instance.OnImageReceieved -= HandleImageEvent;
        UserController.Instance.OnSwipeEvent -= HandleSwipeEvent;
        UserController.Instance.OnDoubleTapEvent -= HandleDoubleTapEvent;
        UserController.Instance.OnSingleTapEvent -= HandleSingleTapEvent;
        UserController.Instance.OnPinchZoomEvent -= HandlePinchZoomEvent;
    }

    private void Start()
    {
        UnityClientSender.Instance.mode = mode;

    }
    private void HandleImageEvent(List<Texture2D> texture2Ds)
    {
        haveImage = true;
        bannerImages = texture2Ds;
    }
    private void HandleDialogueEvent(EventData eventData)
    {
        StartCoroutine(HandleDialogueEventCoroutine(eventData));
    }

    private IEnumerator HandleDialogueEventCoroutine(EventData eventData)
    {
        currentEventData = eventData;
        UnityClientSender.Instance.SendEventRequest(new RequestEventData
        {
            EventName = "Image Request Event",
            Option = eventData.Keywords,
        });
        yield return new WaitUntil(() => haveImage);
        headlineBanner.ActivateBanner(bannerImages, currentEventData.Summary);
        functionButtonManager.ActivateFunctionButtons(eventData.Candidate);
        bannerImages = null;
        haveImage = false;
    }

    public void RaiseUserSelectEvent(string text, string id)
    {
        switch (id)
        {
            case "more info":
                OnMoreInfoSelected?.Invoke(text);
                break;
            case "opinion":
                OnOpinionSelected?.Invoke(text);
                break;
            case "manifesto":
                OnManifestoSelected?.Invoke(text);
                break;
            case "follow up":
                OnFollowUpSelected?.Invoke(text);
                break;
        }

    }
    public void PublishToChat(string text, bool isInteractive, string functionName = null)
    {
        if (isInteractive)
        {
            chatManager.AddInteractiveMessage(text, functionName);
        }
        else
        {
            chatManager.AddNonInteractiveMessage(text);
        }
    }
    public void PublishDepthText(string text)
    {
        depthTextManager.SetDepthText(text);
    }
    public void PublishManifestoMessage()
    {
        chatManager.AddManifestoSelectMessage();
    }
    private void HandleSwipeEvent(string direction)
    {
        if (direction == "up")
        {
            TabManager.Instance.HandleReactTab();
        }
        else if (direction == "down")
        {
            TabManager.Instance.HandleOpinionTab();
        }
    }
    private void HandleDoubleTapEvent()
    {
        TabManager.Instance.HandleFollowUpTab();
    }
    private void HandleSingleTapEvent()
    {
        DeactivateFunctionButton();
        TabManager.Instance.DestroyActiveTab();
    }
    private void HandlePinchZoomEvent(float factor)
    {
        depthTextManager.UpdateTextDepthLevel(factor);
    }
    public void DeactivateFunctionButton()
    {
        functionButtonManager.DeactivateActiveFunctionButton();
    }

}
