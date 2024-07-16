using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class HyperlinkHandler : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on Text");
        if (!enabled) return;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = textComponent.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();
            CuiManager.Instance.RaiseUserSelectEvent(linkInfo.GetLinkText(), linkId);
            enabled = false;
        }
    }

}
