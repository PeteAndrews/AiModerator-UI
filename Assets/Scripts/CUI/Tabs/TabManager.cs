using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

[System.Serializable]
public class TabPosition
{
    public string position;
    public Transform transform;
}
public interface ITabManager
{
    void Initialize();
    void ToggleTab(GameObject tab);
    void ActivateTab(string tabName);
}
public class TabManager: MonoBehaviour//, ITabManager
{
    public TabsConfig tabsConfig;
    public Tabs activeTab;
    public List<TabPosition> tabPositionsTrump;
    public List<TabPosition> tabPositionsBiden;
    private Dictionary<string, List<TabPosition>> tabPositions;

    public Dictionary<string, string> mapTabs = new Dictionary<string, string>()
    {
        {"fact check Trump", "tab-factCheck-trump" },
        {"polarity Trump", "tab-polarity-trump" },
        {"more info Trump", "tab-moreInfo-trump" },
        {"manifesto Trump", "tab-manifesto-trump" },
        {"fact check Biden", "tab-factCheck-biden" },
        {"polarity Biden", "tab-polarity-biden" },
        {"more info Biden", "tab-moreInfo-biden" },
        {"manifesto Biden", "tab-manifesto-biden" }
    };
    private void Start()
    {
        tabPositions = new Dictionary<string, List<TabPosition>>()
        {
            { "Trump", tabPositionsTrump },
            { "Biden", tabPositionsBiden }
        };
    }
    public bool CheckTabExists(string functionName)
    {
        return activeTab != null ? activeTab.shortName == functionName : false;
    }
    public void Activate(string tabName, string candidateName, bool isActive)
    {
        string fullName = tabName + " " + candidateName;
        tabName = mapTabs[fullName];
        var tab = System.Array.Find(tabsConfig.tabs, pair => pair.tabName == tabName);

        if (tab != null && tabPositions.Count > 0)
        {
            TabPosition tabPosition = tabPositions[candidateName].FirstOrDefault(p => p.position == tab.positionName);
            Transform position = tabPosition?.transform;
            if (position != null)
            {
                tab.Initialize(position, isActive);
                activeTab = tab;
                tab.tabStrategy.Activate();
            }
        }
    }
    public void SwitchTab(string tabName, string candidateName, bool isActive)
    {
        if (activeTab.tabStrategy != null)
        {
            DestroyActiveTab();
        }
        Activate(tabName, candidateName, isActive);
    }
    public void DestroyActiveTab()
    {
        if (activeTab != null)
        {
            CuiManager.Instance.HandleDestroyActiveTab(activeTab.shortName, activeTab.candidateName);
            activeTab.tabStrategy.Deactivate();
            Destroy(activeTab.tabInstance);
            activeTab.tabStrategy = null;
        }
    }
}
