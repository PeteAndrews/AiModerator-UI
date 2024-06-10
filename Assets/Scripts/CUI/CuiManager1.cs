/*
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Text;

public class CuiManager1 : MonoBehaviour
{
    public static CuiManager1 Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public TabConfig cuiConfig; 
    public Transform cuiParent;
    public Transform buttonsParent;
    //private Dictionary<TabButtonPair, TabInfo> aliveTabInfos = new Dictionary<TabButtonPair, TabInfo>();
    public GameObject activeTab;
    private CuiScrollView activeChat;
    private float cumulativeButtonWidth = 0f;
    [SerializeField] private RectTransform buttonsParentRect;
    private List<string> validOptions = new List<string> { "fact check", "polarity", "more info", "continue, manifesto" };



    public static Dictionary<string, string> mapTabs = new Dictionary<string, string>()
    {
        {"fact check", "cuiTab-factCheck" },
        {"polarity", "cuiTab-polarity" },
        {"more info", "cuiTab-moreInfo" },
        {"manifesto", "cuiTab-manifesto" }
    };
    // Use a dictionary to keep track of instantiated tabs and buttons

    private void OnEnable()
    {
        RetrievePushData.Instance.OnOptionsEvent += HandlePythonOptionsInput;
        RetrievePushData.Instance.OnGptEvent += HandlePythonGptInput;
        RetrievePushData.Instance.OnMoreInfoResponseEvent += HandleMoreInfoResponse;
    }
    private void OnDisable()
    {
        RetrievePushData.Instance.OnOptionsEvent -= HandlePythonOptionsInput;
        RetrievePushData.Instance.OnGptEvent -= HandlePythonGptInput;
        RetrievePushData.Instance.OnMoreInfoResponseEvent -= HandleMoreInfoResponse;
    }

    private void Start()
    {
        InitializeTabsAndButtons();
    }

    void PrintChildNames(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Debug.Log(child.name);
            PrintChildNames(child); // Recursively print all children
        }
    }
    private string GetGameObjectPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
    private void InitializeTabsAndButtons() {
        foreach (var pair in cuiConfig.tabs)
        {
            GameObject tab = Instantiate(pair.tabPrefab, cuiParent);
            tab.SetActive(false);

            GameObject button = Instantiate(pair.buttonPrefab, buttonsParent);
            Button btnComponent = button.GetComponent<Button>();
            if (btnComponent != null)
            {
                btnComponent.onClick.AddListener(() => ToggleTab(pair.tabInstance));
            }
            button.SetActive(false);
        }
    }

    private void ToggleTab(GameObject tab)
    {
        // Deactivate all tabs
        foreach (var pair in cuiConfig.tabs)
        {
            if (pair.tabInstance != null)
                pair.tabInstance.SetActive(false);
        }

        // Activate the selected tab
        tab.SetActive(true);
        name = GetKeyFromValue(tab.name);
        if(name != null && name != "more info")
        {
            UnityClientSender.Instance.ReceiveButtonName(name);
        }
        else if (name != null && name == "more info")
        {
            UnityClientSender.Instance.SendMoreInfoRequest();
        }
        else
        {
            Debug.LogError("Tab name not found in dictionary or is more info event");
        }
        activeTab = tab;
        activeChat = activeTab.GetComponentInChildren<CuiScrollView>();

    }

    public string GetKeyFromValue(string value)
    {
        // Remove "(Clone)" from the value
        string cleanValue = value.Replace("(Clone)", "").Trim();

        // Search the dictionary for the clean value
        foreach (var pair in mapTabs)
        {
            if (pair.Value == cleanValue)
            {
                // If the value matches, return the corresponding key
                return pair.Key;
            }
        }

        // If no matching value was found, return null
        return null;
    }
    private GameObject InstantiateTabAndButtons(TabButtonPair tbp)
    {
        // Instantiate tab and store reference
        GameObject tab = Instantiate(tbp.tabPrefab, cuiParent);
        tbp.tabInstance = tab;
        tab.SetActive(false); // Tabs start deactivated

        // Instantiate button and calculate position
        GameObject button = Instantiate(tbp.buttonPrefab, buttonsParent);
        RectTransform btnRect = button.GetComponent<RectTransform>();

        // Ensure the RectTransform for button has been driven completely before placement
        LayoutRebuilder.ForceRebuildLayoutImmediate(btnRect);

        // Positioning first button at the right edge and moving leftward for subsequent buttons
        float buttonWidth = btnRect.rect.width;
        float initialRightPosition = buttonsParent.GetComponent<RectTransform>().rect.width - cumulativeButtonWidth - buttonWidth;
        btnRect.anchoredPosition = new Vector2(initialRightPosition, btnRect.anchoredPosition.y);

        // Update cumulative width for next button placement
        cumulativeButtonWidth += buttonWidth;

        Button btnComponent = button.GetComponent<Button>();
        if (btnComponent != null)
        {
            btnComponent.onClick.AddListener(() => ToggleTab(tab));
        }

        tbp.buttonInstance = button;
        button.SetActive(false);

        return tab;
    }
    private void CleanTabsAndButtons()
    {
        // Clean up inactive tabs
        foreach (Transform child in cuiParent)
        {
            if (!child.gameObject.activeSelf)
            {
                Destroy(child.gameObject);
            }
        }

        // Clean up buttons whose associated tabs are not active
        foreach (Transform child in buttonsParent)
        {
            TabButtonPair pair = cuiConfig.tabs.FirstOrDefault(p => p.buttonInstance == child.gameObject);
            if (pair != null && !pair.tabInstance.activeSelf)
            {
                Destroy(child.gameObject);
            }
        }
    }
    //if tab is active, dont destory, keep active but destroy inactive
    public void OnOptionsEvent(string[] tabNames)
    {
        // Clear existing tabs and buttons
        try
        {
            CleanTabsAndButtons();
            foreach (string name in tabNames)
            {
                var excludedNames = new List<string> { "react", "opinion", "follow up" };
                if (excludedNames.Contains(name))
                {
                    continue;
                }
                string tabName = mapTabs[name];
                var tabButtonPair = System.Array.Find(cuiConfig.tabs, pair => pair.tabName == tabName);
                if (tabButtonPair != null)
                {
                    GameObject tab = InstantiateTabAndButtons(tabButtonPair);
                    //    SetAliveTabsDict(tabButtonPair, tab);
                }
            }
        }
        catch (Exception ex)
        {
            // Handle the exception
            Debug.LogError("An error occurred: " + ex.Message);
        }

    }
    public void ActivateTabButtons()//will need to add a argument to define the active tab button and the inactive tab buttons
    {
        foreach(var pair in cuiConfig.tabs)
        {
            if (pair.buttonInstance != null)
            {
                pair.buttonInstance.SetActive(true);
            }
        }
    }
    public void ActivateTab(string tabName)
    {
        tabName = mapTabs[tabName];
        var tabButtonPair = System.Array.Find(cuiConfig.tabs, pair => pair.tabName == tabName);//returning null
        if (tabButtonPair != null)
        {
            tabButtonPair.tabInstance.SetActive(true);
            activeTab = tabButtonPair.tabInstance;
            activeChat = activeTab.GetComponentInChildren<CuiScrollView>();
        }
        ActivateTabButtons();
    }
    private void HandlePythonOptionsInput(string eventName, string[] tabNames)
    {
        Debug.Log($"Received event: {eventName}, with data: {tabNames}");
        OnOptionsEvent(tabNames);
    }
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
    }
    public void UpdateActiveTabChat(string time, string text, string functionName, bool scrollBottom, bool isHyperText)
    {
        if (activeChat == null)
        {
            Debug.LogError("ChatScrollView component not found in the active tab.");
            return;
        }
        if (isHyperText)
        {
            bool splitAtCommas = !text.Contains("1."); 
            text = ConvertToHyperlinks(text, splitAtCommas);
            activeChat.AddHyperLinkMessage(time, text, functionName);
        }
        else
        {
            activeChat.AddNonInteractiveMessage(time, text, scrollBottom);
        }
    }

    private void HandlePythonGptInput(string eventName, string text, string functionName, string articleName, bool isHyperText)
    {
        // Handle the event

        if (eventName == activeTab.name)
        {
            UpdateActiveTabChat(DateTime.Now.ToString("HH:mm:ss"), text, functionName, false, isHyperText);
        }
        else
        {
            UpdateActiveTabChat(DateTime.Now.ToString("HH:mm:ss"), text, functionName, true, isHyperText);

        }
        if (validOptions.Contains(functionName))
        {
            SelectInteractionOption(functionName);
        }
        /*
        Debug.Log($"Received event: {functionName}, with data: {text}");
        switch (functionName)
        {
            case "fact check":
                SelectInteractionOption(functionName);
                break;

            case "polarity":
                SelectInteractionOption(functionName);
                break;

            case "more info":
                SelectInteractionOption(functionName);
                break;
            case "continue":
                SelectInteractionOption(functionName);
                break;

        
    }
    public void HandleOptionButtonOnClick(string buttonName)
    {
        switch(buttonName)
        {
            case "fact check":
                UnityClientSender.Instance.ReceiveButtonName(buttonName);
                break;

            case "polarity":
                UnityClientSender.Instance.ReceiveButtonName(buttonName);
                break;

            case "more info":
                //make request to python server for the information
                UnityClientSender.Instance.SendMoreInfoRequest();
                break;
        }
        
    }
    private void HandleMoreInfoResponse(string eventName, string text)
    {
        //update chat with hyperlinks - try to generalise as this will be used for follow up requests
        text = ConvertToHyperlinks(text, true);
        activeChat.AddHyperLinkMessage(DateTime.Now.ToString("HH:mm:ss"), text, "more info");
    }

    private void SelectInteractionOption(string funcName)
    {

        activeChat.AddInteractionOptionMessage(DateTime.Now.ToString("HH:mm:ss"), null);
    }
    
    private void OnPolarityEvent(string funcName)
    {
        activeChat.AddInteractionOptionMessage(DateTime.Now.ToString("HH:mm:ss"), null);

        Debug.Log("CuiManager::OnPolarityEvent");

    }
    private void OnMoreInfoEvent(string funcName)
    {
        activeChat.AddInteractionOptionMessage(DateTime.Now.ToString("HH:mm:ss"), null);

    }
}
*/
