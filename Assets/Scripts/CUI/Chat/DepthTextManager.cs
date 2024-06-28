using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthTextManager : MonoBehaviour
{
    private Dictionary<DetailLevel, string> currentText = new Dictionary<DetailLevel, string>();
    private float zoomFactor = 0.0f;

    

    public enum DetailLevel
    {
        Brief, Moderate, Detailed
    }
    public void SetDepthText(string fullText)
    {
        string[] paragraphs = fullText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        if (paragraphs.Length >= 3)
        {
            currentText[DetailLevel.Brief] = CleanText(paragraphs[0]);
            currentText[DetailLevel.Moderate] = CleanText(paragraphs[1]);
            currentText[DetailLevel.Detailed] = CleanText(paragraphs[2]);
        }
        CuiManager.Instance.PublishToChat(currentText[DetailLevel.Brief], false, functionName:null);
    }

    private string CleanText(string text)
    {
        int firstLetterIndex = text.IndexOfAny(new char[] { '.', ':' });
        if (firstLetterIndex != -1 && text.Length > firstLetterIndex + 1)
        {
            char nextChar = text[firstLetterIndex + 1];
            if (nextChar == ' ' || nextChar == ':') 
            {
                text = text.Substring(firstLetterIndex + 2).Trim();
            }
        }
        return text;
    }

    public void UpdateTextDepthLevel(float factor)
    {
        if (factor < 0.33)
            PublishText(currentText[DetailLevel.Brief].ToString());
        else if (factor < 0.66)
            PublishText(currentText[DetailLevel.Moderate].ToString());
        else
            PublishText(currentText[DetailLevel.Detailed].ToString());
    }
    private void PublishText(string text)
    {
        CuiManager.Instance.PublishToChat(text, false, functionName:null);
    }

}
