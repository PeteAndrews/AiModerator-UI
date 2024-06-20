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
    private Dictionary<FunctionButton, Vector3> originalPositions = new Dictionary<FunctionButton, Vector3>();
    private Dictionary<string, FunctionButton> currentBottomButtons = new Dictionary<string, FunctionButton>();
    private Dictionary<string, FunctionButton> activeButtons = new Dictionary<string, FunctionButton>();

    private FunctionButton activeButton;
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

        StartCoroutine(StorePositionsAfterFrame());
        currentBottomButtons[GetContainerName(trumpFunctionButtonContainer)] = buttonContainers[GetContainerName(trumpFunctionButtonContainer)].Last();
        currentBottomButtons[GetContainerName(bidenFunctionButtonContainer)] = buttonContainers[GetContainerName(bidenFunctionButtonContainer)].Last();
    }
    IEnumerator StorePositionsAfterFrame()
    {
        yield return null; // Wait for one frame
        StoreOriginalPositions();
    }
    private void StoreOriginalPositions()
    {
        // Disable the layout group
        var layoutGroup = trumpFunctionButtonContainer.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.enabled = false;
        }

        var layoutGroupBiden = bidenFunctionButtonContainer.GetComponent<VerticalLayoutGroup>();
        if (layoutGroupBiden != null)
        {
            layoutGroupBiden.enabled = false;
        }

        // Store positions
        foreach (var pair in buttonContainers)
        {
            foreach (var button in pair.Value)
            {
                originalPositions[button] = button.transform.position;
                Debug.Log("Storing position for " + button.gameObject.name + ": " + button.transform.position);
            }
        }

        // Re-enable the layout group
        if (layoutGroup != null)
        {
            layoutGroup.enabled = true;
        }
        if (layoutGroupBiden != null)
        {
            layoutGroupBiden.enabled = true;
        }
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
        ForceButtonsToOrigin();
        FunctionButton[] buttons = buttonContainers[candidate];
        FunctionButton selectedButton = Array.Find(buttons, b => b.name == eventName);
        if (activeButton != null)
        {
            DeactivateActiveFunctionButton();
        } 
        if (activeButtons.ContainsKey(candidate) && activeButtons[candidate] != selectedButton)
        {
            // Swap the currently active button back to its original position
            SwapButtonToOriginalPosition(activeButtons[candidate]);
        }

        SwapButtonPosition(selectedButton, candidate);
        activeButton = selectedButton;
        TabManager.Instance.SwitchTab(eventName, candidate);
    }
    private void ForceButtonsToOrigin()
    {
        foreach (var pair in buttonContainers)
        {
            foreach (var button in pair.Value)
            {
                SwapButtonToOriginalPosition(button);
                Debug.Log("Resetting position for " + button.gameObject.name + " to " + originalPositions[button]);

            }
        }
        var keys = new List<string>(currentBottomButtons.Keys); 
        foreach (var key in keys)
        {
            currentBottomButtons[key] = buttonContainers[key].Last();
        }
        activeButtons.Clear();
    }
    private void SwapButtonToOriginalPosition(FunctionButton button)
    {
        if (button != null)
        {
            button.transform.position = originalPositions[button];
        }
    }
    private void SwapButtonPosition(FunctionButton selectedButton, string candidate)
    {
        FunctionButton bottomButton = currentBottomButtons[candidate];

        if (selectedButton != bottomButton) // If not currently at the bottom
        {
            Vector3 tempPosition = selectedButton.transform.position;
            selectedButton.transform.position = bottomButton.transform.position;
            bottomButton.transform.position = tempPosition;

            // Update the current bottom button
            currentBottomButtons[candidate] = selectedButton;
            activeButtons[candidate] = selectedButton; // Update the active button
        }
        else
        {
            selectedButton.transform.position = originalPositions[selectedButton];
            currentBottomButtons[candidate] = buttonContainers[candidate].Last();
            activeButtons.Remove(candidate); // Clear the active button as it's reset
        }
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
    public void DeactivateActiveFunctionButton()
    {
        activeButton.Deactivate();
        ForceButtonsToOrigin();
    }

    private void OnDestroy()
    {
        // Unregister event handlers to avoid memory leaks
        UnregisterAllButtons(trumpButtons);
        UnregisterAllButtons(bidenButtons);
    }

}
