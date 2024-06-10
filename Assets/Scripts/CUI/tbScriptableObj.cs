using UnityEngine;

[CreateAssetMenu(fileName = "TabConfig", menuName = "Configuration/TabConfig")]
public class TabConfig : ScriptableObject
{
    public TabButtonPair[] tabs;
}

[System.Serializable]
public class TabButtonPair
{
    public string tabName;
    public GameObject tabPrefab;
    public GameObject buttonPrefab;
    public GameObject tabInstance; // To keep a runtime reference of the instantiated tab
    public GameObject buttonInstance; // To keep a runtime reference of the instantiated button
}

