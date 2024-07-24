using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpinionPoll : MonoBehaviour
{
    private List<Transform> barFills = new List<Transform>();
    private List<Transform> barBacks = new List<Transform>();
    public TextMeshProUGUI textHeading;

    void Start()
    {
        Transform[] childComponents = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in childComponents)
        {
            if (child.name == "bar-fill")
            {
                barFills.Add(child);
            }
            else if (child.name == "bar-back")
            {
                barBacks.Add(child);
            }
        }
        RandomlyDistributeWidths();
    }
    private void RandomlyDistributeWidths()
    {
        int totalWidth = 450;
        int[] widths = new int[barFills.Count];
        int totalAssigned = 0;

        for (int i = 0; i < barFills.Count - 1; i++)
        {
            widths[i] = Random.Range(0, totalWidth - totalAssigned);
            totalAssigned += widths[i];
        }

        widths[barFills.Count - 1] = totalWidth - totalAssigned;

        for (int i = 0; i < barFills.Count; i++)
        {
            RectTransform rectTransform = barFills[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(widths[i], rectTransform.sizeDelta.y);
            }
        }
    }

}
