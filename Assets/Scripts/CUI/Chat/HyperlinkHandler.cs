using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class HyperlinkHandler : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text textComponent;
    private Dictionary<string, ILinkAction> linkActionMap;




    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        //Mode = "Default";
        linkActionMap = new Dictionary<string, ILinkAction>
        {
            { "opinion", new OpinionAction() },
            { "moreInfo", new MoreInfoAction() },
            { "more info", new MoreInfoAction() },
            { "followUp", new FollowUpAction() },
            { "manifesto", new ManifestoAction() },
            { "select interaction option", new SelectInteractionOption()}
        };

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on text!");
        if (!enabled) return;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = textComponent.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();

            if (linkActionMap.TryGetValue(linkId, out ILinkAction action))
            {
                action.ExecuteAction(linkInfo.GetLinkText());
            }
            enabled = false;
        }
    }

}
