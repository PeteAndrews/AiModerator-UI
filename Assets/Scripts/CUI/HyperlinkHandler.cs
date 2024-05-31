using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HyperlinkHandler : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text textComponent;
    public string Mode { get; set; }

    private Dictionary<string, string> linkFunctionMap = new Dictionary<string, string>()
    {
        {"Share your opinion", "opinion"},
        {"Find out more", "follow up"},
    };

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        Mode = "Default"; 

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on text!");
        if (!enabled) return;  // If the handler is disabled, do not process the click.

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = textComponent.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();
            string linkText = linkInfo.GetLinkText();

            Debug.Log($"Clicked on link ID: {linkId}, Text: '{linkText}'");
            ExecuteLinkAction(linkId, linkText);
            enabled = false;  // Disable this handler to prevent further clicks.
        }
    }
    private void ExecuteLinkAction(string linkId, string linkText)
    {
        // Determine what happens based on the linkId
        //options will be followUp, opinion, moreInfo
        //after first select of opinion, need a converse then another opinion
        //AddHyperLinkMessage in CuiManager will need to set a property that is used in
        //the switch function below to determine what to do
        switch (Mode)
        {
            case "select interaction option":
                RaiseGenericEvent(linkText);
                break;
            case "opinion":
                RaiseOpinionEvent(linkText);
                break;
            case "more info":
                RaiseMoreInfoEvent(linkText);
                break;
            case "follow up":
                RaiseMoreFollowUp(linkText);
                break;
            default:
                RaiseGenericEvent(linkText);
                break;
        }
        //follow up will be a generic event but needs different name
        //follow up second interaction...
    }

    private void RaiseMoreInfoEvent(string linkText)
    {
        // Define actions based on linkId
        Debug.Log("Clicked on link ID: " + linkText);
        // Example: switch statement to handle different link IDs
        UnityClientSender.Instance.MoreInfoEvent(linkText);
    }
    private void RaiseOpinionEvent(string linkText)
    {
        Debug.Log("Clicked on link ID: " + linkText + "  Raising Opinion Event");
    }
    private void RaiseMoreFollowUp(string linkText)
    {
        Debug.Log("Clicked on link ID: " + linkText + "  Raising Continue Event");
        UnityClientSender.Instance.SendEventContinueInteraction("Continue Event", linkText);
    }
    private void RaiseGenericEvent(string linkText)
    {
        UnityClientSender.Instance.SendEventNoResponse("Select Event", linkFunctionMap[linkText]); 
    }
}
