using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FunctionButton : MonoBehaviour
{
    public delegate void ButtonClickHandler(string buttonName, string candidate);
    public event ButtonClickHandler OnButtonClicked;
    private static Dictionary<string, string> mapCandidateNames = new Dictionary<string, string>()
    {
        {"FunctionButtonsParent-Trump", "Trump"},
        {"FunctionButtonsParent-Biden", "Biden"}
    };
    private Button button;
    private Image buttonImage;
    public Color activeColour;
    public Color inactiveColour;
    public Color selectedColour;


    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();

    }
    void Start()
    {
        button.interactable = false;
        if (button != null)
        {
            string candidateName = mapCandidateNames[transform.parent.name];
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
        ChangeButtonColour(selectedColour);
        Debug.Log("Button Clicked");
    }
    private void ChangeButtonColour(Color colour)
    {
        if (buttonImage != null)
        {
            buttonImage.color = colour;
        
        }
    }
    public void Activate()
    {
        button.interactable = true;
        ChangeButtonColour(activeColour);
    }
    public  void Deactivate()
    {
        button.interactable = false;
        ChangeButtonColour(inactiveColour);

    }
    public void DestroyButton()
    {
        Destroy(gameObject);
    }
}