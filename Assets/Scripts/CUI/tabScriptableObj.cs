using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TabsConfig", menuName = "Configuration/TabsConfig")]
public class TabsConfig : ScriptableObject
{
    public Tabs[] tabs;
}
[System.Serializable]
public class Tabs
{
    public string tabName;
    public string shortName;
    public string candidateName;
    public GameObject tabPrefab;
    public string positionName;
    public GameObject tabInstance; // To keep a runtime reference of the instantiated tab
    public ITabStrategy tabStrategy;

    public void Initialize(Transform position, bool isActive)
    {
        tabInstance = GameObject.Instantiate(tabPrefab, position);
        tabInstance.active = isActive;
        AssignStrategy();
    }
    public void AssignStrategy()
    {
        switch (tabName)
        {
            case "tab-factCheck-trump": tabStrategy = tabInstance.AddComponent<FactCheckTabStrategy>(); break;
            case "tab-manifesto-trump": tabStrategy = tabInstance.AddComponent<ManifestoTabStrategy>(); break;
            case "tab-polarity-trump": tabStrategy = tabInstance.AddComponent<PolarityTabStrategy>(); break;
            case "tab-moreInfo-trump": tabStrategy = tabInstance.AddComponent<MoreInfoTabStrategy>(); break;
            case "tab-factCheck-biden": tabStrategy = tabInstance.AddComponent<FactCheckTabStrategy>(); break;
            case "tab-manifesto-biden": tabStrategy = tabInstance.AddComponent<ManifestoTabStrategy>(); break;
            case "tab-polarity-biden": tabStrategy = tabInstance.AddComponent<PolarityTabStrategy>(); break;
            case "tab-moreInfo-biden": tabStrategy = tabInstance.AddComponent<MoreInfoTabStrategy>(); break;
            default: Debug.LogWarning("Strategy not found for: " + tabName); break;
        }
    }
}

