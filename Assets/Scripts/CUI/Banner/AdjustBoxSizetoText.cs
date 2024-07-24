using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdjustBoxSizetoText : MonoBehaviour
{
    private RectTransform background;
    private void Start()
    {
        background = GetComponent<RectTransform>();
    }
    public IEnumerator UpdateTextBackgroundSize(TextMeshProUGUI text)
    {
        Debug.Log("UpdateTextBackgroundSize started");
        yield return new WaitForEndOfFrame();
        AdjustBackgroundSize(text);
        Debug.Log("Background adjusted");

    }
    private void AdjustBackgroundSize(TextMeshProUGUI text) 
    {
        Debug.Log($"Adjust Background Size, preferredWidth: {text.GetPreferredValues().x}");
        // Calculate the preferred width for the current text
        float preferredWidth = text.GetPreferredValues().x;
        float padding = 20f;  // Add some padding around the text

        // Set the size of the background
        background.sizeDelta = new Vector2(preferredWidth + padding, background.sizeDelta.y);
    }
}
