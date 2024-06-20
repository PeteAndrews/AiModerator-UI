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
    public Transform tabPosition;
    public GameObject tabInstance; // To keep a runtime reference of the instantiated tab

    public void Initialize(bool isActive)
    {
        tabInstance = GameObject.Instantiate(tabPrefab, tabPosition);
        tabInstance.active = isActive;
    }

}

