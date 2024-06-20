using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;
using System;
using System.Drawing.Printing;
using System.ServiceModel.Channels;

public static class TimeHelper
{
    public static string CurrentTime => DateTime.Now.ToString("HH:mm:ss");
}
public class HyperLinkConverter
{
    public string Convert(string input, string functionName)
    {
        switch (functionName)
        {
            case "more info":
                return ConvertByCommas(input, ref functionName);
            case "follow up":
                return ConvertByNumbers(input, ref functionName);
            default:
                return ConvertByNumbers(input, ref functionName);
        }
    }
    private string ConvertByCommas(string input, ref string linkId)
    {
        StringBuilder stringBuilder = new StringBuilder();
        string[] elements = input.Split(',');
        Debug.Log(elements.Length);
        foreach (string element in elements)
        {
            string trimmedElement = element.Trim();
            if (string.IsNullOrEmpty(trimmedElement))
                continue;

            stringBuilder.Append($"<link=\"{linkId}\">{trimmedElement}</link>");
            stringBuilder.Append(" and ");
        }

        // Remove the last " and " if it exists
        if (stringBuilder.Length >= 5)
            stringBuilder.Remove(stringBuilder.Length - 5, 5);

        stringBuilder.Append(" for more info.");
        return stringBuilder.ToString();
    }

    private string ConvertByNumbers(string input, ref string linkId)
    {
        StringBuilder stringBuilder = new StringBuilder();
        string pattern = @"(?<=\n)(?=\d+\.)|(?<=\.)\s*(?=\d+\.)";
        string[] elements = Regex.Split(input, pattern);

        foreach (string element in elements)
        {
            string trimmedElement = element.Trim();
            if (string.IsNullOrEmpty(trimmedElement))
                continue;

            stringBuilder.Append($"<link=\"{linkId}\">{trimmedElement}</link>");
            stringBuilder.AppendLine();
        }

        stringBuilder.Append(" for more info.");
        return stringBuilder.ToString();
    }
}
public class ChatManager : MonoBehaviour
{
    private List<string> validOptions = new List<string> { "fact check", "polarity", "more info", "continue, manifesto" };
    private HyperLinkConverter hyperLinkConverter = new HyperLinkConverter();
    private void ResizeTextCollider()
    {
        AdjustColliderToText adjustCollider = TabManager.Instance.activeTab.tabInstance.GetComponent<AdjustColliderToText>();
        StartCoroutine(adjustCollider.UpdateColliderSizeAfterFrame());
    }

    public void AddInteractiveMessage(string text, string functionName)
    {
        if (functionName != null)
        { 
            text = hyperLinkConverter.Convert(text, functionName);
        }
        AddInteractiveHyperLinkMessage(text);
    }

    public void AddInteractiveHyperLinkMessage(string text)
    {

        CuiMessage message = TabManager.Instance.activeTab.tabInstance.GetComponent<CuiMessage>();
        if (message == null || message.timeText == null || message.mainText == null)
        {
            Debug.LogError("MessageUI component or Text components not found in the instantiated prefab.");
            return; 
        }

        message.timeText.text = TimeHelper.CurrentTime;
        message.mainText.richText = true;

        message.mainText.text += string.IsNullOrEmpty(message.mainText.text) ? text : "\n" + text;

        if (message.hyperlinkHandler != null)
        {
            message.hyperlinkHandler.enabled = message.mainText.text.Contains("<link=");
        }
        ResizeTextCollider();
    }
    public void AddNonInteractiveMessage(string mainText)
    {
        CuiMessage messageUI = TabManager.Instance.activeTab.tabInstance.GetComponent<CuiMessage>();

        if (messageUI != null && messageUI.timeText != null && messageUI.mainText != null)
        {
            messageUI.timeText.text = TimeHelper.CurrentTime;
            messageUI.mainText.text = mainText;
        }
        else
        {
            Debug.LogError("MessageUI component or Text components not found in the instantiated prefab.");
        }
        ResizeTextCollider();
    }
    public void AddManifestoSelectMessage()
    {
        string text = "Would you like to consult the <link=\"manifesto\"><color=blue>Republican</color></link> manifesto or <link=\"manifesto\"><color=blue>Democratic</color></link> manifesto?";
        AddInteractiveHyperLinkMessage(text);
    }
}
