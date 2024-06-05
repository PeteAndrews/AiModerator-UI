using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;
using System;
using System.Drawing.Printing;

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
            case "fact check":
                return ConvertByNumbers(input, ref functionName);
            default:
                return ConvertByCommas(input, ref functionName);
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
    private TabManager tabManager;
    private List<string> validOptions = new List<string> { "fact check", "polarity", "more info", "continue, manifesto" };
    private HyperLinkConverter hyperLinkConverter = new HyperLinkConverter();
    public void Initialize(TabManager tabManager)
    {
        this.tabManager = tabManager;
    }
    /*
    public static string ConvertToHyperlinks(string input, bool splitAtCommas)
    {
        StringBuilder stringBuilder = new StringBuilder();
        int linkId = 0;  // This will keep track of the link ID incrementally

        // Define the pattern for splitting at numbered points or commas based on the splitAtCommas flag
        string pattern = splitAtCommas ? "," : @"(?<=\n)(?=\d+\.)|(?<=\.)\s*(?=\d+\.)";

        // Split the input based on the specified boolean parameter using Regex for numbers
        string[] elements = Regex.Split(input, pattern);

        foreach (string element in elements)
        {
            string trimmedElement = element.Trim();  // Trim to remove any leading/trailing whitespaces

            if (string.IsNullOrEmpty(trimmedElement))
                continue;  // Skip empty strings that might result from multiple delimiters

            // Append each element as a hyperlink
            stringBuilder.Append($"<link=\"ID{linkId}\">{trimmedElement}</link>");

            if (splitAtCommas && trimmedElement.EndsWith(","))
                stringBuilder.Append(" and ");  // This adds 'and' between links, adjust as needed for grammar.
            else if (!splitAtCommas)
                stringBuilder.AppendLine();  // Add a newline if the element is from a split at a number

            linkId++;  // Increment link ID for each element
        }

        stringBuilder.Append(" for more info."); // Appends this text after all links
        return stringBuilder.ToString();
    }*/
    /*
    public void MoreInfoToHyperLinks(string text)
    {
        //update chat with hyperlinks - try to generalise as this will be used for follow up requests
        text = ConvertToHyperlinks(text, true);
        tabManager.activeChat.AddHyperLinkMessage(DateTime.Now.ToString("HH:mm:ss"), text, "more info");
    }*/
    public void AddInteractiveHyperLinkMessage(string text, string functionName)
    {
        text = hyperLinkConverter.Convert(text, functionName);
        tabManager.activeChat.AddHyperLinkMessage(TimeHelper.CurrentTime, text, functionName);
    }
    /*
    public void UpdateChat(string eventName, string text, string functionName, string articleName, bool isHyperText)
    {
        // Handle the event

        if (eventName == tabManager.activeChat.name)
        {
            UpdateActiveChat(DateTime.Now.ToString("HH:mm:ss"), text, functionName, false, isHyperText);
        }
        else
        {
            UpdateActiveChat(DateTime.Now.ToString("HH:mm:ss"), text, functionName, true, isHyperText);

        }
        if (validOptions.Contains(functionName))
        {
            SelectInteractionOptionMessage(functionName);
        }
    }*/
    public void UpdateActiveChat(string text, string functionName, bool scrollBottom, bool isHyperText)
    {
        if (tabManager?.activeChat == null)
        {
            Debug.LogError("Active chat is not set.");
            return;
        }

        if (isHyperText)
        {
            AddInteractiveHyperLinkMessage(text, functionName);
        }
        else
        {
            tabManager.activeChat.AddNonInteractiveMessage(TimeHelper.CurrentTime, text, scrollBottom);
        }

        if (validOptions.Contains(functionName))
        {
            AddSelectInteractionOptionMessage(functionName);
        }
    }
    private void AddSelectInteractionOptionMessage(string funcName)
    {

        tabManager.activeChat.AddInteractionOptionMessage(TimeHelper.CurrentTime, null);
    }
    public void ManifestoActivationMessage()
    {
        if(tabManager.activeChat != null)
        {
            tabManager.activeChat.AddManifestoActivationMessage(TimeHelper.CurrentTime);
        }
        else
        {
            Debug.LogError("No Active Chat found in ChatManager::ManifestoActivationMessage");
        }
    }
}
