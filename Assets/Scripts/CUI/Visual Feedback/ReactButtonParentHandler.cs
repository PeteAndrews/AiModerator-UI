using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReactButtonParentHandler : MonoBehaviour
{
    private Button button;
    private EmojiBar emojiBar;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        emojiBar = GetComponentInParent<EmojiBar>();

    }

    private void OnButtonClick()
    {
        string id = button.name;
        Transform transform = button.transform;
        emojiBar.InstantiateSecondaryButtons(id);
        Destroy(button.gameObject);
    }
    void OnDestroy()
    {
        if (button != null) {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
