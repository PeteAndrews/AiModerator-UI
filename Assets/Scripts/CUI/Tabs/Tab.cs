using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tab : MonoBehaviour
{
    public string tabName;
    public string shortName;
    public string candidateName;
    public GameObject tabPrefab;
    public string positionName;
    public Transform tabPosition;
    public GameObject tabInstance;
    private ITabState currentState;
    private List<ITabState> transitionSequence;
    private int currentStateIndex = -1;
    public bool IsReadyForNextState = false;

    public bool HasReceivedServerResponse { get; set; }
    public string serverResponse { get; set; }
    public bool UserHasInteracted { get; set; }


    private void OnEnable()
    {
        RetrievePushData.Instance.OnGptEvent += ReceiveDataFromServer;
        RetrievePushData.Instance.OnMoreInfoResponseEvent += RecieveKeywordsFromServer;
    }
    private void OnDisable()
    {
        RetrievePushData.Instance.OnGptEvent -= ReceiveDataFromServer;
        RetrievePushData.Instance.OnMoreInfoResponseEvent -= RecieveKeywordsFromServer;
    }
    public void SetStateSequence(List<ITabState> sequence)
    {
        transitionSequence = sequence;
        TransitionToNextState();
    }

    public void TransitionToNextState()
    {
        currentStateIndex++;
        if (currentStateIndex < transitionSequence.Count)
        {
            SetState(transitionSequence[currentStateIndex]);
        }
        else
        {
        }
    }

    public void SetState(ITabState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    void Update()
    {
        currentState?.UpdateState(this);
    }
    public void EnableAdvancedFeatures()
    {
        UserController.Instance.SetTouchInteractionEnabled(true);
    }

    public void EnableBasicFeatures()
    {
        UserController.Instance.SetTouchInteractionEnabled(false);
    }
    public void RequestDataFromServer()
    {
    }
    private void RecieveKeywordsFromServer(string eventName, string keywords)
    {
        serverResponse = keywords;
        HasReceivedServerResponse = true;
    }
    private void ReceiveDataFromServer(string eventName, string text, string functionName, string articleName, bool isHyperText)
    {
        serverResponse = text;
        HasReceivedServerResponse = true;
    }

}

public class FactCheckTab : Tab
{
    void Start()
    {
        List<ITabState> states = new List<ITabState>
        {
            new ActivationState(new FactCheckActivation(), previousResponse : null),
            new WaitServerState(),
            new PublishState(new FactCheckPublishBehavior()),
            new WaitUserInteractionState(),
            new TerminateState()
        };
        SetStateSequence(states);
    }
}
public class PolarityTab : Tab
{
    void Start()
    {
        List<ITabState> states = new List<ITabState>
        {
            new ActivationState(new PolarityActivationBehaviour(), previousResponse : null),
            new WaitServerState(),
            new PublishState(new PolarityPublishBehaviour()),
            new WaitUserInteractionState(),
            new TerminateState()
        };
        SetStateSequence(states);
    }
}
public class MoreInfoTab : Tab 
{
    void Start()
    {
        List<ITabState> states = new List<ITabState>
        {
            new ActivationState(new MoreInfoActivationBehaviour(), previousResponse: null),
            new WaitServerState(),
            new PublishState(new MoreInfoPublishKeywordsBehaviour()),
            new WaitUserSelectionState(),
            new WaitServerState(),
            new PublishState(new MoreInfoPublishBehaviour()),
            new WaitUserInteractionState(),
            new TerminateState()
        };
        SetStateSequence(states);
    }
}
public class ManifestoTab : Tab
{
    void Start()
    {
        List<ITabState> states = new List<ITabState>
        {
            new ActivationState(new ManifestoActivationBehaviour(), previousResponse : null),
            new WaitServerState(),
            new PublishState(new ManifestoPublishBehaviour()),
            new WaitUserInteractionState(),
            new TerminateState()
        };
        SetStateSequence(states);
    }
}
public class OpinionTab : Tab
{
    void Start()
    {
        List<ITabState> states = new List<ITabState>
        {
            new ActivationState(new OpinionActivationBehaviour(), previousResponse:null),
            new WaitServerState(),
            new PublishState(new OpinionPublishBehaviour()),
            new WaitUserSelectionState(),
            new TerminateState() 
        };
        SetStateSequence(states);
    }
}
public class FollowUpTab : Tab
{
    void Start()
    {
        List<ITabState> states = new List<ITabState>
        {
            new ActivationState(new FollowUpActivationBehaviour(), previousResponse:null),
            new WaitServerState(),
            new PublishState(new FollowUpPublishBehaviour()),
            new WaitUserSelectionState(),
            new WaitServerState(),
            new PublishState(new FollowUpContinuedPublishBehaviour()),
            new WaitUserInteractionState(),
            new TerminateState()
        };
        SetStateSequence(states);
    }
}
public class ContinueTab : Tab
{
    void Start()
    {
        string previousResponse = SharedDataService.Instance.GetResponse(tabName);
        SharedDataService.Instance.ClearResponse(tabName);

        List<ITabState> states = new List<ITabState>
        {
            new ActivationState(new ContinueActivationBehaviour(), previousResponse),
            new WaitServerState(),
            new PublishState(new ContinuePublishBehaviour()),
            new WaitUserInteractionState(),
            new TerminateState()
        };
        SetStateSequence(states);
    }
}
