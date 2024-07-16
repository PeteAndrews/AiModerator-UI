using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventDisplay : MonoBehaviour
{
    public TextMeshPro text;
    private void OnEnable()
    {
        DialogueEventLoader.Instance.OnSendEventData += HandleEvent;
    }
    private void OnDisable()
    {
        DialogueEventLoader.Instance.OnSendEventData -= HandleEvent;
    }
    private void HandleEvent(EventData eventData)
    {
        text.text = eventData.ResponseTopic;
    }
}
