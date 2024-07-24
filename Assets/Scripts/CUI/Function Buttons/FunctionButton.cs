using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunctionButton : MonoBehaviour
{
    public delegate void ButtonClickHandler(string buttonName, string candidate);
    public event ButtonClickHandler OnButtonClicked;

    private static Dictionary<string, string> mapCandidateNames = new Dictionary<string, string>()
    {
        {"FunctionButtonsParent-Trump(Clone)", "Trump"},
        {"FunctionButtonsParent-Biden(Clone)", "Biden"}
    };

    public Button button;
    private Image buttonImage;  
    public string candidateName;

    public Sprite activeSprite;
    public Sprite inactiveSprite;
    public Sprite selectedSprite;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    void Start()
    {
        button.interactable = true;
        if (button != null)
        {
            candidateName = mapCandidateNames[transform.parent.name];
            button.onClick.AddListener(() => {
                OnSelect();
                OnButtonClicked?.Invoke(gameObject.name, candidateName);
            });
        }
        else
        {
            Debug.LogError("Button component not found on the object!");
        }
    }

    private void OnSelect()
    {
        button.interactable = false;
        ChangeButtonSprite(selectedSprite);
        Debug.Log("Button Clicked");
    }

    private void ChangeButtonSprite(Sprite sprite)
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = sprite;
        }
    }

    public void Activate()
    {
        button.interactable = true;
        ChangeButtonSprite(activeSprite);
    }

    public void Deactivate()
    {
        button.interactable = false;
        ChangeButtonSprite(inactiveSprite);
    }

    public void DestroyButton()
    {
        Destroy(gameObject);
    }
}