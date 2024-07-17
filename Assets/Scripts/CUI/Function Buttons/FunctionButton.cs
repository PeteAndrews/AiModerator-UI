using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Transform = System.Security.Cryptography.Xml.Transform;

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
    
    // Qingyuan: for testing the button
    public GameObject CuiMessagePrefabTrump;
    public GameObject CuiMessagePrefabBiden;
    public Transform cuiMessageParentTrump;
    public Transform cuiMessageParentBiden;
    
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
        
        // Qingyuan: for testing the button, activate it on start
        Activate();
    }
    private void OnSelect()
    {
        button.interactable = false;
        ChangeButtonColour(selectedColour);
        Debug.Log("Button Clicked");
        
        //Qingyuan: for testing the button, instantiate a speech bubble CuiMessage prefab
        if (transform.parent.name == "FunctionButtonsParent-Trump")
        {
            GameObject cuiTrump = Instantiate(CuiMessagePrefabTrump, transform.position, Quaternion.identity);
            CuiMessage cuiMessage = cuiTrump.GetComponent<CuiMessage>();
            cuiMessage.mainText.text = "Trump: test CuiMessage";
            //cuiMessage.transform.SetParent(cuiMessageParentTrump, false);
        }
        else if (transform.parent.name == "FunctionButtonsParent-Biden")
        {
            GameObject cuiBiden = Instantiate(CuiMessagePrefabBiden, transform.position, Quaternion.identity);
            CuiMessage cuiMessage = cuiBiden.GetComponent<CuiMessage>();
            cuiMessage.mainText.text = "Biden: test CuiMessage";
            //cuiMessage.transform.SetParent(cuiMessageParentBiden, false);
            
        }
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