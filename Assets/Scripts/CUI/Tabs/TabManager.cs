using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public interface ITabManager
{
    void Initialize();
    void ToggleTab(GameObject tab);
    void ActivateTab(string tabName);
}

public class TabManager : MonoBehaviour, ITabManager
{
    public GameObject activeTab;
    public CuiScrollView activeChat;

    public TabConfig tabConfig;
    public Transform tabsParent;
    public Transform buttonsParent;
    private float cumulativeButtonWidth = 0f;

    public Dictionary<string, string> mapTabs = new Dictionary<string, string>()
    {
        {"fact check", "cuiTab-factCheck" },
        {"polarity", "cuiTab-polarity" },
        {"more info", "cuiTab-moreInfo" },
        {"manifesto", "cuiTab-manifesto" }
    };
    public void ActivateTab(string tabName)
    {
        tabName = mapTabs[tabName];
        var tabButtonPair = System.Array.Find(tabConfig.tabs, pair => pair.tabName == tabName);

        // If tabButtonPair is null, activate tab buttons and return early
        if (tabButtonPair == null)
        {
            ActivateTabButtons();
            return;
        }

        tabButtonPair.tabInstance.SetActive(true);
        activeTab = tabButtonPair.tabInstance;
        activeChat = activeTab.GetComponentInChildren<CuiScrollView>();

        var newStrategy = activeTab.GetComponent<ITabStrategy>();

        // If newStrategy is null, activate tab buttons and return early
        if (newStrategy == null)
        {
            ActivateTabButtons();
            return;
        }

        newStrategy.Activate();
        ActivateTabButtons();
    }
    public void ActivateTabv1(string tabName)
    {
        tabName = mapTabs[tabName];
        var tabButtonPair = System.Array.Find(tabConfig.tabs, pair => pair.tabName == tabName);
        if (tabButtonPair != null)
        {
            tabButtonPair.tabInstance.SetActive(true);
            activeTab = tabButtonPair.tabInstance;
            activeChat = activeTab.GetComponentInChildren<CuiScrollView>();
            var newStrategy = activeTab.GetComponent<ITabStrategy>();
            if (newStrategy != null)
            {
                newStrategy.Activate();
            }
        }
        ActivateTabButtons();
    }
    public void ActivateTabButtons()//will need to add a argument to define the active tab button and the inactive tab buttons
    {
        foreach (var pair in tabConfig.tabs)
        {
            if (pair.buttonInstance != null)
            {
                pair.buttonInstance.SetActive(true);
            }
        }
    }
    public void Initialize()
    {
        foreach (var pair in tabConfig.tabs)
        {
            GameObject tab = Instantiate(pair.tabPrefab, tabsParent);
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
    private GameObject InstantiateTabAndButtons(TabButtonPair tbp)
    {
        // Instantiate tab and store reference
        GameObject tab = Instantiate(tbp.tabPrefab, tabsParent);
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
    public void ToggleTab(GameObject tab)
    {
        if (activeTab != null)
        {
            // Deactivate the currently active tab using its attached strategy
            var activeStrategy = activeTab.GetComponent<ITabStrategy>();
            if (activeStrategy != null)
            {
                activeStrategy.Deactivate();
            }
        }

        // Activate the new tab
        tab.SetActive(true);
        var newStrategy = tab.GetComponent<ITabStrategy>();
        if (newStrategy != null)
        {
            newStrategy.Activate();
        }

        // Update the activeTab reference and possibly the active chat reference
        activeTab = tab;
        activeChat = activeTab.GetComponentInChildren<CuiScrollView>();
    }
    public void CleanTabsAndButtons()
    {
        // Clean up inactive tabs
        foreach (Transform child in tabsParent)
        {
            if (!child.gameObject.activeSelf)
            {
                Destroy(child.gameObject);
            }
        }

        // Clean up buttons whose associated tabs are not active
        foreach (Transform child in buttonsParent)
        {
            TabButtonPair pair = tabConfig.tabs.FirstOrDefault(p => p.buttonInstance == child.gameObject);
            if (pair != null && !pair.tabInstance.activeSelf)
            {
                Destroy(child.gameObject);
            }
        }
    }
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
                var tabButtonPair = System.Array.Find(tabConfig.tabs, pair => pair.tabName == tabName);
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

}
