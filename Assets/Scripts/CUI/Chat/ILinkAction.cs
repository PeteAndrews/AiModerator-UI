using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILinkAction
{
    void ExecuteAction(string linkText);
}
public class OpinionAction : ILinkAction
{
    public void ExecuteAction(string linkText)
    {
        Debug.Log($"Opinion Action Triggered: {linkText}");
        UnityClientSender.Instance.SendEventNoResponse("Select Event", "opinion");

    }
}

public class MoreInfoAction : ILinkAction
{
    public void ExecuteAction(string linkText)
    {
        UnityClientSender.Instance.MoreInfoEvent(linkText);
    }
}
public class ManifestoAction : ILinkAction
{
    public void ExecuteAction(string linkText)
    {
        UnityClientSender.Instance.SendManifestoEvent(linkText);
    }
}

public class FollowUpAction : ILinkAction
{
    public void ExecuteAction(string linkText)
    {
        UnityClientSender.Instance.SendEventContinueInteraction("Continue Event", linkText);
    }
}
public class SelectInteractionOption : ILinkAction
{
    private Dictionary<string, string> linkFunctionMap = new Dictionary<string, string>()
    {
        {"Share your opinion", "opinion"},
        {"Find out more", "follow up"},
    };
    public void ExecuteAction(string linkText)
    {
        UnityClientSender.Instance.SendEventNoResponse("Select Event", "follow up");
    }
}