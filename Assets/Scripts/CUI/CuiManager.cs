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
{    public static CuiManager Instance { get; private set; }
    public UserController userController;
    public HeadlineBanner headlineBanner;
    private float adjustmentY = 28f;
    private float adjustmentX = 20f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
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
   // public string mode;
    private EventData currentEventData;
    public event Action<string> OnMoreInfoSelected;
    public event Action<string> OnOpinionSelected;
    public event Action<string> OnManifestoSelected;
    public event Action<string> OnFollowUpSelected;
    public event Action<string, string, Transform> InstantiateReactFeedback;
    public event Action<string, Transform> InstantiatePollFeedback;
    private bool haveImage = false;
    private List<Texture2D> bannerImages;
    private List<string> bannerNames;
    public GameObject waitIconPrefab;
    private GameObject currentWaitIcon;
    public GameObject bidenPollPrefab;
    public GameObject trumpPollPrefab;
    private GameObject currentPoll;
    public string pollSelection;


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
    private void HandleImageEvent(List<Texture2D> texture2Ds, List<string> imageNames)
    {
        haveImage = true;
        bannerImages = texture2Ds;
        bannerNames = imageNames;
    }
    private void HandleDialogueEvent(EventData eventData)
    {
        StartCoroutine(HandleDialogueEventCoroutine(eventData));
    }

    private IEnumerator HandleDialogueEventCoroutine(EventData eventData)
    {
        currentEventData = eventData;
        eventData.EventName = "Image Request Event";
        UnityClientSender.Instance.ImageRequestAndDataSet(eventData);
        yield return new WaitUntil(() => haveImage);
        headlineBanner.ActivateBanner(bannerImages, bannerNames, currentEventData.Summary);
        functionButtonManager.ActivateFunctionButtons(eventData.Candidate);

        bannerImages = null;
        bannerNames = null;
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
        if (isInteractive && functionName!="opinion")
        {
            chatManager.AddInteractiveMessage(text, functionName);
        }
        else if (isInteractive && functionName == "opinion")
        {
            chatManager.AddInteractivePollMessage(text);
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
        Debug.Log("HandleSingleTap event called and destroying th tab");
        TabManager.Instance.DestroyActiveTab();
        DeactivateFunctionButton();
    }
    private void HandlePinchZoomEvent(float factor)
    {
        depthTextManager.UpdateTextDepthLevel(factor);
    }
    public void DeactivateFunctionButton()
    {
        functionButtonManager.DeactivateActiveFunctionButton();
    }
    public void InstantiateWaitAnimation()
    {
        CuiMessage cuiMessage = TabManager.Instance.activeTab.tabInstance.GetComponent<CuiMessage>();
        if (currentWaitIcon != null)
        {
            Destroy(currentWaitIcon);
        }

        currentWaitIcon = Instantiate(waitIconPrefab, cuiMessage.mainText.transform);

        // Get RectTransform of the instantiated icon and the mainText
        RectTransform iconRect = currentWaitIcon.GetComponent<RectTransform>();
        RectTransform textRect = cuiMessage.mainText.GetComponent<RectTransform>();

        iconRect.anchoredPosition = new Vector2((-textRect.rect.width / 2)+adjustmentX, iconRect.anchoredPosition.y + adjustmentY);


    }
    public void DestroyWaitAnimation()
    {
        if (currentWaitIcon != null)
        {
            Destroy(currentWaitIcon);
            currentWaitIcon = null;
        }
    }
    public void InstantiatePoll()
    {
        name = TabManager.Instance.activeTab.candidateName;
        CuiMessage cuiMessage = TabManager.Instance.activeTab.tabInstance.GetComponent<CuiMessage>();
        PublishToChat("\n\n\n\n\n\n\n\n\n\n", false);
        GameObject poll = null;  // Declare 'currentPoll' outside the if blocks to increase its scope

        if (name == "Biden")
        {
            poll = Instantiate(bidenPollPrefab, cuiMessage.mainText.transform);

        }
        else if (name == "Trump")
        {
            poll = Instantiate(trumpPollPrefab, cuiMessage.mainText.transform);
        }
        Debug.Log("Current Poll: " + (currentPoll == null ? "null" : currentPoll.name));
        OpinionPoll opinionPoll = poll.GetComponent<OpinionPoll>();
        if (opinionPoll == null)
        {
            Debug.LogError("OpinionPoll component not found on " + currentPoll.name);
        }
        opinionPoll.textHeading.text = chatManager.opinionPollData.HeaderText;
        string candidate = chatManager.opinionPollData.Options.GetValueOrDefault(pollSelection.Trim());
        currentPoll = poll;
        //Instantiate Visual Feedback
        InstantiatePollFeedback?.Invoke(candidate, cuiMessage.mainText.transform);
    }
    public void InstantiateReact(string id, string label, Transform transform)
    {
        name = TabManager.Instance.activeTab.candidateName;
        InstantiateReactFeedback?.Invoke(id, label, transform);
    }
    public void DestroyReactGameObject()
    {
        Debug.Log("Callback: DestroyReactGameObject Complete");
    }
    public void DestroyPoll()
    {
        if (currentPoll != null)
        {
            Destroy(currentPoll);
            currentPoll = null;
        }
        pollSelection = null;
    }
    public void ToggleFunctionButtonsInteraction(bool isInteractive)
    {
        functionButtonManager.ToggleFunctionButtonsInteractions(isInteractive);
    }

}
