using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FunctionButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject trumpFunctionButtonContainer;
    [SerializeField] private GameObject bidenFunctionButtonContainer;
    private Dictionary<string, FunctionButton[]> buttonContainers = new Dictionary<string, FunctionButton[]>();

    Button[] trumpButtons;
    Button[] bidenButtons;

    void Start()
    {
        trumpButtons = trumpFunctionButtonContainer.GetComponentsInChildren<Button>();
        bidenButtons = bidenFunctionButtonContainer.GetComponentsInChildren<Button>();

        BuildButtonDictionary(GetContainerName(trumpFunctionButtonContainer), trumpButtons);
        BuildButtonDictionary(GetContainerName(bidenFunctionButtonContainer), bidenButtons);

        RegisterFunctionButtons(trumpButtons);
        RegisterFunctionButtons(bidenButtons);
    }
    private void BuildButtonDictionary(string candidate, Button[] buttons)
    {
        List<FunctionButton> functionButtons = new List<FunctionButton>();
        foreach (Button button in buttons)
        {
            FunctionButton functionButton = button.GetComponent<FunctionButton>();
            if (functionButton != null)
            {
                functionButtons.Add(functionButton);
            }
        }
        buttonContainers.Add(candidate, functionButtons.ToArray());
    }
    private void RegisterFunctionButtons(Button[] buttons)
    {
        foreach (Button button in buttons)
        {
            FunctionButton functionButton = button.GetComponent<FunctionButton>();
            if (functionButton != null)
            {
                functionButton.OnButtonClicked += HandleFunctionButtonEvent;
            }
        }
    }
    private string GetContainerName(GameObject container)
    {
        string containerName = container.name.Split('-')[1].Trim();
        return containerName;
    }
    private void UnregisterAllButtons(Button[] buttons)
    {
        foreach (Button button in buttons)
        {
            FunctionButton functionButton = button.GetComponent<FunctionButton>();
            if (functionButton != null)
            {
                functionButton.OnButtonClicked -= HandleFunctionButtonEvent;
            }
        }
    }
    private void HandleFunctionButtonEvent(string eventName, string candidate)
    {
        CuiManager.Instance.HandleFunctionButtonEvent(eventName, candidate);
    }
    public void ActivateFunctionButtons(string candidate)
    {
        foreach (FunctionButton button in buttonContainers[candidate])
        {
            button.Activate();
        }
    }
    public void DeactivateFunctionButtons(string candidate)
    {
        foreach (FunctionButton button in buttonContainers[candidate])
        {
            button.Deactivate();
        }
    }
    public void DeactivateFunctionButton(string buttonName, string candidateName)
    {
        var button = buttonContainers[candidateName].FirstOrDefault(b => b.name == buttonName);
        if (button != null)
        {
            button.Deactivate();
        }
        //test this, and then check that toggling through tabs works
        //after that, need to chech that follow up fucntions spawn new tabs
    }
    private void OnDestroy()
    {
        // Unregister event handlers to avoid memory leaks
        UnregisterAllButtons(trumpButtons);
        UnregisterAllButtons(bidenButtons);
    }

}
