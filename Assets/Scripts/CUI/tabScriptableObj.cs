using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TabsConfig", menuName = "Configuration/TabsConfig")]
public class TabsConfig : ScriptableObject
{
    public TabMetaData[] tabMetaData;
}
[System.Serializable]
public class TabMetaData
{
    public string tabName;
    public string shortName;
    public string candidateName;
    public GameObject tabPrefab;
    public string positionName;
    public Transform tabPosition;
    public GameObject tabInstance; 
}

