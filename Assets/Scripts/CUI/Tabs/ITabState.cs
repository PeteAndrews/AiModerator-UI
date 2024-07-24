using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        RequestEventData requestEventData = new RequestEventData { EventName = "Select Event", Option = "manifesto" };
        UnityClientSender.Instance.SendEventRequest(requestEventData);

        //CuiManager.Instance.PublishManifestoMessage();

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
        Debug.Log("Opinion Publish Behaviour");

        CuiManager.Instance.PublishToChat(context.serverResponse, true, functionName: "opinion");
    }
}
public class FollowUpPublishBehaviour : IPublishBehaviour
{
    public void Publish(Tab context)
    {
        Debug.Log("Follow Up Publish Behaviour");
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
        CuiManager.Instance.ToggleFunctionButtonsInteraction(false);
        CuiManager.Instance.PublishToChat("", false);
        CuiManager.Instance.InstantiateWaitAnimation();
    }

    public void ExitState(Tab context)
    {
        context.HasReceivedServerResponse = false;
        CuiManager.Instance.PublishToChat("", false);
        CuiManager.Instance.DestroyWaitAnimation();
        CuiManager.Instance.ToggleFunctionButtonsInteraction(true);

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

public class OnPollEventState : ITabState
{
    private Tab context; // Keep a reference to the context if needed in the HandleInteractionEvent

    public void EnterState(Tab context)
    {
        this.context = context; // Store the context
        context.inPollEvent = true;
        CuiManager.Instance.InstantiatePoll();
        UserController.Instance.OnDoubleTapEvent += HandleInteractionEvent;
        UserController.Instance.OnSingleTapEvent += HandleInteractionEvent;
        UserController.Instance.OnPinchZoomEvent += HandleInteractionEvent;
        UserController.Instance.OnSwipeEvent += HandleInteractionEvent;
        context.EnableAdvancedFeatures();

    }

    public void ExitState(Tab context)
    {
        // Ensure that the event is unregistered to avoid memory leaks and unintended behavior
        UserController.Instance.OnDoubleTapEvent -= HandleInteractionEvent;
        UserController.Instance.OnSingleTapEvent -= HandleInteractionEvent;
        UserController.Instance.OnPinchZoomEvent -= HandleInteractionEvent;
        UserController.Instance.OnSwipeEvent -= HandleInteractionEvent;
        context.inPollEvent = false;
        CuiManager.Instance.DestroyPoll();
        CuiManager.Instance.DeactivateFunctionButton();
        context.EnableBasicFeatures();
    }

    public void HandleTransition(Tab context)
    {
        throw new NotImplementedException();
    }

    public void UpdateState(Tab context)
    {
        if (!context.inPollEvent)
        {
            context.TransitionToNextState();
        }
    }
    private void HandleInteractionEvent()
    {
        SetInteractionComplete();
    }

    private void HandleInteractionEvent(float num)
    {
        SetInteractionComplete();
    }

    private void HandleInteractionEvent(string dir)
    {
        SetInteractionComplete();
    }

    private void SetInteractionComplete()
    {
        if (context != null)
            context.inPollEvent = false;
    }
}

public class WaitUserSelectionState : ITabState
{
    private bool userHasSelected = false;

    public void EnterState(Tab context)
    {
        CuiManager.Instance.OnMoreInfoSelected += HandleMoreInfoSelected;
        //CuiManager.Instance.OnManifestoSelected += HandleManifestoSelected;
        CuiManager.Instance.OnOpinionSelected += HandleOpinionSelected;
        CuiManager.Instance.OnFollowUpSelected += HandleFollowUpSelected;
    }

    public void ExitState(Tab context)
    {
        CuiManager.Instance.OnMoreInfoSelected -= HandleMoreInfoSelected;
        //CuiManager.Instance.OnManifestoSelected -= HandleManifestoSelected;
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
        CuiManager.Instance.pollSelection = linkText;
        //STORE THE TEXT WITH THE LINKED NAME, MAY

        //throw new NotImplementedException();
        // Instantiate poll
    }
    /*
    private void HandleManifestoSelected(string linkText)
    {
        userHasSelected = true;
        RequestEventData requestEventData = new RequestEventData { EventName = "Select Event", Option = "manifesto", Choice = linkText };
        UnityClientSender.Instance.SendEventRequest(requestEventData);
    }*/
    private void HandleFollowUpSelected(string linkText)
    {
        userHasSelected = true;
        RequestEventData requestEventData = new RequestEventData { EventName = "Continue Event", Option = "continue", Choice = linkText };
        UnityClientSender.Instance.SendEventRequest(requestEventData);
        Debug.Log("Follow Up Selected and Moving to Continue Event");
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
        TabManager.Instance.activeTab = null;
        Debug.Log("Terminating Tab");
        //CuiManager.Instance.DeactivateFunctionButton();

    }

    public void ExitState(Tab context)
    {
        // Deactivate Function Button
    }

    public void UpdateState(Tab context)
    {
    }

    public void HandleTransition(Tab context)
    {
    }
}
