using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SharedDataService
{
    public static SharedDataService Instance { get; } = new SharedDataService();

    private Dictionary<string, string> responses = new Dictionary<string, string>();

    public void SetResponse(string key, string response)
    {
        responses[key] = response;
    }

    public string GetResponse(string key)
    {
        responses.TryGetValue(key, out var response);
        return response;
    }

    public void ClearResponse(string key)
    {
        if (responses.ContainsKey(key))
        {
            responses.Remove(key);
        }
    }
}


public interface IActivationBehavior
{
    void Activate(Tab context, string previousResponse=null);
}
public interface IPublishBehaviour
{
    void Publish(Tab context);
}
public interface ITabState
{
    void EnterState(Tab context);
    void ExitState(Tab context);
    void UpdateState(Tab context);
    void HandleTransition(Tab context);
}
public class FactCheckActivation : IActivationBehavior
{
    public void Activate(Tab context, string previousResponse)
    {
        context.EnableBasicFeatures();
        RequestEventData requestEventData = new RequestEventData { EventName = "Select Event", Option = "fact check" };
        UnityClientSender.Instance.SendEventRequest(requestEventData);

    }
}
public class PolarityActivationBehaviour : IActivationBehavior
{
    public void Activate(Tab context, string previousResponse)
    {
        context.EnableBasicFeatures();
        RequestEventData requestEventData = new RequestEventData { EventName = "Select Event", Option = "polarity" };
        UnityClientSender.Instance.SendEventRequest(requestEventData);

    }
}
public class MoreInfoActivationBehaviour : IActivationBehavior
{
    public void Activate(Tab context, string previousResponse)
    {
        context.EnableBasicFeatures();
        RequestEventData requestEventData = new RequestEventData { EventName = "More Info Request Event", Option = "more info" };
        UnityClientSender.Instance.SendEventRequest(requestEventData);
    }
}
public class ManifestoActivationBehaviour : IActivationBehavior
{
    public void Activate(Tab context, string previousResponse)
    {
        context.EnableBasicFeatures();
        CuiManager.Instance.PublishManifestoMessage();

    }
}
public class OpinionActivationBehaviour : IActivationBehavior
{
    public void Activate(Tab context, string previousResponse)
    {
        context.EnableBasicFeatures();
        RequestEventData requestEventData = new RequestEventData { EventName = "Select Event", Option = "opinion" };
        UnityClientSender.Instance.SendEventRequest(requestEventData);

    }
}
public class FollowUpActivationBehaviour : IActivationBehavior
{
    public void Activate(Tab context, string previousResponse)
    {
        context.EnableBasicFeatures();
        RequestEventData requestEventData = new RequestEventData { EventName = "Select Event", Option = "follow up" };
        UnityClientSender.Instance.SendEventRequest(requestEventData);
    }
}
public class ContinueActivationBehaviour : IActivationBehavior
{
    public void Activate(Tab context, string previousResponse)
    {
        context.EnableBasicFeatures();
        RequestEventData requestEventData = new RequestEventData { EventName = "Continue Event", Choice = previousResponse};
        UnityClientSender.Instance.SendEventRequest(requestEventData);
    }
}

public class FactCheckPublishBehavior : IPublishBehaviour
{
    public void Publish(Tab context)
    {
        CuiManager.Instance.PublishDepthText(context.serverResponse);
    }
}
public class PolarityPublishBehaviour : IPublishBehaviour
{
    public void Publish(Tab context)
    {
        CuiManager.Instance.PublishDepthText(context.serverResponse);
    }
}
public class MoreInfoPublishBehaviour : IPublishBehaviour
{
    public void Publish(Tab context)
    {
        CuiManager.Instance.PublishDepthText(context.serverResponse);
    }
}
public class MoreInfoPublishKeywordsBehaviour : IPublishBehaviour
{
    public void Publish(Tab context)
    {
        CuiManager.Instance.PublishToChat(context.serverResponse, true, functionName : "more info");
    }
}
public class ManifestoPublishBehaviour : IPublishBehaviour
{
    public void Publish(Tab context)
    {
        CuiManager.Instance.PublishDepthText(context.serverResponse);
    }
}
public class OpinionPublishBehaviour : IPublishBehaviour
{
    public void Publish(Tab context)
    {
        CuiManager.Instance.PublishToChat(context.serverResponse, true, functionName: "opinion");
    }
}
public class FollowUpPublishBehaviour : IPublishBehaviour
{
    public void Publish(Tab context)
    {
        CuiManager.Instance.PublishToChat(context.serverResponse, true, functionName: "follow up");
    }
}
public class FollowUpContinuedPublishBehaviour : IPublishBehaviour
{
    public void Publish(Tab context)
    {
        CuiManager.Instance.PublishDepthText(context.serverResponse);
    }
}
public class ContinuePublishBehaviour : IPublishBehaviour
{
    public void Publish(Tab context)
    {
        CuiManager.Instance.PublishToChat(context.serverResponse, false, functionName: null);
    }
}
public class ActivationState : ITabState
{
    private IActivationBehavior activationBehavior;
    private string previousResponse;


    public ActivationState(IActivationBehavior activationBehavior, string previousResponse = null)
    {
        this.activationBehavior = activationBehavior;
        this.previousResponse = previousResponse;
    }

    public void EnterState(Tab context)
    {
        activationBehavior.Activate(context, previousResponse);
        context.IsReadyForNextState = true;
    }

    public void ExitState(Tab context)
    {
        context.IsReadyForNextState = false;
    }

    public void HandleTransition(Tab context)
    {
    }

    public void UpdateState(Tab context)
    {
        if (context.IsReadyForNextState)
        {
            context.TransitionToNextState();
        }
    }
}

public class PublishState : ITabState
{
    private IPublishBehaviour publishBehavior;

    public PublishState(IPublishBehaviour publishBehavior)
    {
        this.publishBehavior = publishBehavior;
    }
    public void EnterState(Tab context)
    {
        publishBehavior.Publish(context);
        context.IsReadyForNextState = true;
    }

    public void ExitState(Tab context)
    {
        context.IsReadyForNextState = false;
    }

    public void UpdateState(Tab context)
    {
        if (context.IsReadyForNextState)
        {
            context.TransitionToNextState();
        }
    }

    public void HandleTransition(Tab context)
    {
        context.SetState(new ActivationState(new MoreInfoActivationBehaviour()));
    }
}







public class WaitServerState : ITabState
{
    public void EnterState(Tab context)
    {
    }

    public void ExitState(Tab context)
    {
        context.HasReceivedServerResponse = false; 
    }

    public void UpdateState(Tab context)
    {
        if (context.HasReceivedServerResponse)
        {
            context.TransitionToNextState();
        }
    }
    public void HandleTransition(Tab context)
    {

    }
}







public class WaitUserSelectionState : ITabState
{
    private bool userHasSelected = false;

    public void EnterState(Tab context)
    {
        CuiManager.Instance.OnMoreInfoSelected += HandleMoreInfoSelected;
        CuiManager.Instance.OnManifestoSelected += HandleManifestoSelected;
        CuiManager.Instance.OnOpinionSelected += HandleOpinionSelected;
        CuiManager.Instance.OnFollowUpSelected += HandleFollowUpSelected;
    }

    public void ExitState(Tab context)
    {
        CuiManager.Instance.OnMoreInfoSelected -= HandleMoreInfoSelected;
        CuiManager.Instance.OnManifestoSelected -= HandleManifestoSelected;
        CuiManager.Instance.OnOpinionSelected -= HandleOpinionSelected;
        CuiManager.Instance.OnFollowUpSelected -= HandleFollowUpSelected;   

        userHasSelected = false;
    }

    public void UpdateState(Tab context)
    {
        if (userHasSelected)
        {
            context.TransitionToNextState();
        }
    }

    private void HandleMoreInfoSelected(string linkText)
    {
        userHasSelected = true;
        RequestEventData requestEventData = new RequestEventData { EventName = "More Info Event", Option = "more info", Choice = linkText };
        UnityClientSender.Instance.SendEventRequest(requestEventData);
    }

    private void HandleOpinionSelected(string linkText)
    {
        userHasSelected = true;
        throw new NotImplementedException();
        // Instantiate poll
    }

    private void HandleManifestoSelected(string linkText)
    {
        userHasSelected = true;
        RequestEventData requestEventData = new RequestEventData { EventName = "Select Event", Option = "manifesto", Choice = linkText };
        UnityClientSender.Instance.SendEventRequest(requestEventData);
    }
    private void HandleFollowUpSelected(string linkText)
    {
        userHasSelected = true;
        RequestEventData requestEventData = new RequestEventData { EventName = "Continue Event", Option = "continue", Choice = linkText };
        UnityClientSender.Instance.SendEventRequest(requestEventData);
    }
    public void HandleTransition(Tab context)
    {
    }
}







public class WaitUserInteractionState : ITabState
{
    public void EnterState(Tab context)
    {
        context.EnableAdvancedFeatures();
    }

    public void ExitState(Tab context)
    {
        context.EnableBasicFeatures();
    }

    public void HandleTransition(Tab context)
    {
    }

    public void UpdateState(Tab context)
    {
        if (context.UserHasInteracted)
        {
            context.TransitionToNextState();
        }
    }
}






public class TerminateState : ITabState
{
    private string nextTabName; 

    public TerminateState(string nextTabName = null)
    {
        this.nextTabName = nextTabName;
    }

    public void EnterState(Tab context)
    {
        if (!string.IsNullOrEmpty(nextTabName))
        {
            SharedDataService.Instance.SetResponse(nextTabName, context.serverResponse);
            TabManager.Instance.ActivateTab(nextTabName, context.candidateName);
        }

        GameObject.Destroy(context.gameObject); 
    }

    public void ExitState(Tab context)
    {
        // Deactivate Function Button
        CuiManager.Instance.DeactivateFunctionButton();
    }

    public void UpdateState(Tab context)
    {
    }

    public void HandleTransition(Tab context)
    {
    }
}


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
            new WaitUserSelectionState(),
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
