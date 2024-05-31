using System.Collections;
using System.Collections.Generic;
using System.ServiceModel.Dispatcher;
using System.Text;
using TMPro;
using UnityEngine;

public class CuiScrollView : MonoBehaviour
{
    [SerializeField] private GameObject messageReceivedPrefab; // Assign in the inspector
    [SerializeField] private Transform contentTransform;       // Assign the Content transform from ScrollView
    [SerializeField] private GameObject userInputMessageO2Prefab;
    [SerializeField] private GameObject userInputMessageO3Prefab;
    [SerializeField] private GameObject userInputFollowUpPrefab;

    public void SetupMessage(string time, string hyperText, string functionName, GameObject prefab, Transform contentTransform)
    {
        GameObject messageInstance = Instantiate(prefab, contentTransform);
        CuiMessage messageUI = messageInstance.GetComponent<CuiMessage>();

        if (messageUI != null && messageUI.timeText != null && messageUI.mainText != null)
        {
            messageUI.timeText.text = time;
            messageUI.mainText.text = hyperText;
            messageUI.mainText.richText = true; // Assume rich text needs to be always enabled

            if (messageUI.hyperlinkHandler != null)
            {
                messageUI.hyperlinkHandler.enabled = hyperText.Contains("<link=");
                messageUI.hyperlinkHandler.Mode = functionName;
                //this is where want to set the property of the event
            }
        }
        else
        {
            Debug.LogError("MessageUI component or Text components not found in the instantiated prefab.");
        }
    }
    public void AddInteractionOptionMessage(string time, string mainText)
    {
        string hyperText = "What do you think? <link=opinion><color=blue>Share your opinion</color></link>. Want more info? <link=followUp><color=blue>Find out more</color></link>.";
        SetupMessage(time, hyperText, "select interaction option", userInputFollowUpPrefab, contentTransform);
    }

    public void AddHyperLinkMessage(string time, string mainText, string functionName)
    {
        //string hyperText = ConvertToHyperlinks(mainText);
        SetupMessage(time, mainText, functionName, messageReceivedPrefab, contentTransform);
    }
    public void AddMessage(string time, string mainText, bool scrollBottom)
    {
        // Instantiate the message prefab under the content panel
        GameObject messageInstance = Instantiate(messageReceivedPrefab, contentTransform);
        // Access the MessageUI component to get pre-cached TMP_Text references
        CuiMessage messageUI = messageInstance.GetComponent<CuiMessage>();

        if (messageUI != null && messageUI.timeText != null && messageUI.mainText != null)
        {
            messageUI.timeText.text = time;
            messageUI.mainText.text = mainText;
        }
        else
        {
            Debug.LogError("MessageUI component or Text components not found in the instantiated prefab.");
        }

        if (scrollBottom)
        {
            // Optional: Automatically scroll to the bottom of the ScrollView when a new message is added
            StartCoroutine(ScrollToBottom());
        }
    }
    public void AddUserInputMessage(string time, string func_name)
    {
        GameObject messageInstance;
        if (func_name == "polarity")
        {
            messageInstance = Instantiate(userInputMessageO2Prefab, contentTransform);

        }
        else
        {
            messageInstance = Instantiate(userInputMessageO3Prefab, contentTransform);
        }
        CuiMessage messageUI = messageInstance.GetComponent<CuiMessage>();

        if (messageUI != null && messageUI.timeText != null && messageUI.mainText != null)
        {
            messageUI.timeText.text = time;        }
        else
        {
            Debug.LogError("MessageUI component or Text components not found in the instantiated prefab.");
        }

        // Optional: Automatically scroll to the bottom of the ScrollView when a new message is added
        StartCoroutine(ScrollToBottom());

    }
    private IEnumerator ScrollToBottom()
    {
        // Wait for end of frame so layout will definitely be updated
        yield return new WaitForEndOfFrame();

        // Assuming ScrollView uses a Unity UI ScrollRect component
        GetComponent<UnityEngine.UI.ScrollRect>().normalizedPosition = new Vector2(0, 0);
    }
    public static string ConvertToHyperlinks(string input)
    {
        string[] elements = input.Split(',');
        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < elements.Length; i++)
        {
            string element = elements[i].Trim();  // Trim to remove any leading/trailing whitespaces
            stringBuilder.Append($"<link=\"ID{i}\">{element}</link>");
            if (i < elements.Length - 1)
            {
                stringBuilder.Append(" and "); // This adds 'and' between links. Adjust as needed for grammar.
            }
        }

        stringBuilder.Append(" for more info."); // Appends this text after all links
        return stringBuilder.ToString();
    }

}