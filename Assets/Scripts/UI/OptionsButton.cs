using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; 

public class OptionsButton : MonoBehaviour
{

    public delegate void ButtonClickHandler(string buttonName);
    public event ButtonClickHandler OnButtonClicked;

    // Static dictionary shared across all instances
    private static Dictionary<string, string> buttonNameFormat = new Dictionary<string, string>()
    {
        {"optionsButton-factCheck(Clone)", "fact check"},
        {"optionsButton-polarity(Clone)", "polarity"},
        {"optionsButton-moreInfo(Clone)", "more info"},
        {"optionsButton-opinion(Clone)", "opinion"},
        {"optionsButton-manifesto(Clone)", "manifesto"},
        {"optionsButton-react(Clone)", "react"},
        {"chat-button-react(Clone)", "react"},
        {"chat-button-follow-up(Clone)", "follow up"},
        {"chat-button-my-opinion(Clone)", "my opinion"},
    };

    void Start()
    {
        var button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => {
                string formattedName = GetFormattedName(gameObject.name);
                OnButtonClicked?.Invoke(formattedName);
            });
        }
        else
        {
            Debug.LogError("Button component not found on the object!");
        }
    }
    private static string GetFormattedName(string originalName)
    {
        if (buttonNameFormat.TryGetValue(originalName, out string formattedName))
        {
            return formattedName;
        }
        else
        {
            return originalName;
        }
    }
    public void DestroyButton()
    {
        Destroy(gameObject);
    }
}