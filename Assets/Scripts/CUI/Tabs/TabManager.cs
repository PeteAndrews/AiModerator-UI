using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.WSA;

[System.Serializable]
public class TabPosition
{
    public string position;
    public Transform transform;
}
public class TabManager: MonoBehaviour
{
    public static TabManager Instance { get; private set; }
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
        tabFactories = new Dictionary<string, ITabFactory>()
        {
            { "fact check", new FactCheckTabFactory() },
            { "polarity", new PolarityTabFactory() },
            { "manifesto", new ManifestoTabFactory() },
            { "more info", new MoreInfoTabFactory() },
            { "opinion", new OpinionTabFactory() },
            { "follow up", new FollowUpFactory() },
            { "continue", new ContinueTabFactory()}
        };
    }
    private Dictionary<string, ITabFactory> tabFactories;
    public TabsConfig tabsConfig;
    public Tab activeTab;
    public List<TabPosition> tabPositionsTrump;
    public List<TabPosition> tabPositionsBiden;
    private Dictionary<string, List<TabPosition>> tabPositions;
    public Dictionary<string, string> mapTabs = new Dictionary<string, string>()
    {
        {"fact check Trump", "tab-factCheck-trump" },
        {"polarity Trump", "tab-polarity-trump" },
        {"more info Trump", "tab-moreInfo-trump" },
        {"manifesto Trump", "tab-manifesto-trump" },
        {"opinion Trump", "tab-opinion-trump" },
        {"follow up Trump", "tab-followup-trump" },
        {"continue Trump", "tab-continue-trump" },
        {"depth Trump", "tab-depth-trump" },
        {"fact check Biden", "tab-factCheck-biden" },
        {"polarity Biden", "tab-polarity-biden" },
        {"more info Biden", "tab-moreInfo-biden" },
        {"manifesto Biden", "tab-manifesto-biden" },
        {"opinion Biden", "tab-opinion-biden" },
        {"follow up Biden", "tab-followup-biden" },
        {"continue Biden", "tab-continue-biden" },
        {"depth Biden", "tab-depth-biden" },
    };
    public Transform bidenTabAnchorPosition;
    public Transform trumpTabAnchorPosition;
    private Dictionary<string, Transform> instTabPositions;

    private void Start()
    {
        tabPositions = new Dictionary<string, List<TabPosition>>()
        {
            { "Trump", tabPositionsTrump },
            { "Biden", tabPositionsBiden }
        };
        instTabPositions = new Dictionary<string, Transform>()
        {
            {"Trump", trumpTabAnchorPosition },
            {"Biden", bidenTabAnchorPosition }
        };
    }
    private Transform FindTabPosition(string candidateName, string positionName)
    {
        return tabPositions[candidateName]?.FirstOrDefault(p => p.position == positionName)?.transform;
    }
    public bool CheckTabExists(string functionName)
    {
        return activeTab != null ? activeTab.shortName == functionName : false;
    }
    private void InitializeTab(GameObject tabInstance, Transform position, string candidateName, string tabType, bool isActive)
    {

        Tab tab = tabInstance.GetComponent<Tab>();
        tab.tabPosition = position;
        tab.candidateName = candidateName;
        tab.shortName = tabType;
        tab.tabInstance = tabInstance;
        activeTab = tab;
    }
    public void ActivateTab(string tabType, string candidateName)
    {
        string fullName = tabType + " " + candidateName;
        string tabName = mapTabs.GetValueOrDefault(fullName);
        var tabDetails = System.Array.Find(tabsConfig.tabMetaData, t => t.tabName == tabName);
        if (tabFactories.TryGetValue(tabType, out var factory))
        {
            Transform position = instTabPositions[candidateName];
            GameObject tabInstance = factory.CreateTab(tabDetails.tabPrefab, position);
            InitializeTab(tabInstance, position, candidateName, tabType, isActive: true);
        }
    }
    public void SwitchTab(string eventName, string candidateName)
    {
        if (activeTab != null)
        {
            DestroyActiveTab();
        }
        ActivateTab(eventName, candidateName);
    }
    public void HandleOpinionTab()
    {
        activeTab.UserHasInteracted = true;
        string candidateName = activeTab.candidateName;
        SwitchTab("opinion", candidateName);
    }
    public void HandleReactTab()
    {
       throw new NotImplementedException();
    }
    public void HandleFollowUpTab()
    {
        activeTab.UserHasInteracted=true;
        string candidate = activeTab.candidateName;
        SwitchTab("follow up", candidate);
    }
    public void HandleDepthTab(float factor)
    {

    }
    public void DestroyActiveTab()
    {
        Destroy(activeTab.gameObject);
        activeTab = null;
    }
}
