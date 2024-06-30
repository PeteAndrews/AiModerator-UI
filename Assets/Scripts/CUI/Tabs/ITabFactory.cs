using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITabFactory
{
    GameObject CreateTab(GameObject tabPrefab, Transform position);
}

public class FactCheckTabFactory : ITabFactory
{
    public GameObject CreateTab(GameObject tabPrefab, Transform position)
    {
        GameObject tabInstance = GameObject.Instantiate(tabPrefab, position);
        Tab tab = tabInstance.AddComponent<FactCheckTab>();
        return tabInstance;
    }
}
public class PolarityTabFactory : ITabFactory
{
    public GameObject CreateTab(GameObject tabPrefab, Transform position)
    {
        GameObject tabInstance = GameObject.Instantiate(tabPrefab, position);
        Tab tab = tabInstance.AddComponent<PolarityTab>();
        return tabInstance;
    }
}
public class MoreInfoTabFactory : ITabFactory
{
    public GameObject CreateTab(GameObject tabPrefab, Transform position)
    {
        GameObject tabInstance = GameObject.Instantiate(tabPrefab, position);
        Tab tab = tabInstance.AddComponent<MoreInfoTab>();
        return tabInstance;
    }
}
public class ManifestoTabFactory : ITabFactory
{
    public GameObject CreateTab(GameObject tabPrefab, Transform position)
    {
        GameObject tabInstance = GameObject.Instantiate(tabPrefab, position);
        Tab tab = tabInstance.AddComponent<ManifestoTab>();
        return tabInstance;
    }
}
public class OpinionTabFactory : ITabFactory
{
    public GameObject CreateTab(GameObject tabPrefab, Transform position)
    {
        GameObject tabInstance = GameObject.Instantiate(tabPrefab, position);
        Tab tab = tabInstance.AddComponent<OpinionTab>();
        return tabInstance;
    }
}
public class FollowUpFactory : ITabFactory
{
    public GameObject CreateTab(GameObject tabPrefab, Transform position)
    {
        GameObject tabInstance = GameObject.Instantiate(tabPrefab, position);
        Tab tab = tabInstance.AddComponent<FollowUpTab>();
        return tabInstance;
    }
}
public class ContinueTabFactory : ITabFactory
{
    public GameObject CreateTab(GameObject tabPrefab, Transform position)
    {
        GameObject tabInstance = GameObject.Instantiate(tabPrefab, position);
        Tab tab = tabInstance.AddComponent<ContinueTab>();
        return tabInstance;
    }
}
