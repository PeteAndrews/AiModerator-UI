using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FunctionButtonManager : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private GameObject trumpFunctionButtonContainer;
    [SerializeField] private GameObject bidenFunctionButtonContainer;
    #endregion

    #region Private Fields
    private const string TrumpKey = "Trump";
    private const string BidenKey = "Biden";

    private GameObject currentFunctionButtons = null;
    private Dictionary<string, FunctionButton[]> buttonContainers = new Dictionary<string, FunctionButton[]>();
    private Dictionary<FunctionButton, Vector3> originalPositions = new Dictionary<FunctionButton, Vector3>();
    private Dictionary<string, FunctionButton> currentBottomButtons = new Dictionary<string, FunctionButton>();
    private Dictionary<string, GameObject> functionButtonContainer = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> activeFunctionButtons = new Dictionary<string, GameObject>();
    private Dictionary<string, FunctionButton> activeButtons = new Dictionary<string, FunctionButton>();

    private FunctionButton activeButton = null;
    private FunctionButton originalBottomButton = null;
    private bool flagTrailing = false;
    private string trailingName = null;
    #endregion


    void Start()
    {
        functionButtonContainer.Add("Trump", trumpFunctionButtonContainer);
        functionButtonContainer.Add("Biden", bidenFunctionButtonContainer);
    }
    private void InstantiateFunctionButtons(string candidateName)
    {
        currentFunctionButtons = GameObject.Instantiate(functionButtonContainer[candidateName], canvasTransform);
        SetInitialPosition();
        SetupButtons(candidateName);
    }

    private void SetInitialPosition()
    {
        int lastChildIndex = canvasTransform.childCount - 1; 
        if (lastChildIndex >= 0)
        {
            int insertIndex = flagTrailing ? lastChildIndex - 1 : lastChildIndex;
            insertIndex = Mathf.Clamp(insertIndex, 0, lastChildIndex);  
            currentFunctionButtons.transform.SetSiblingIndex(insertIndex);
        }
    }

    private void SetupButtons(string candidateName)
    {
        Button[] candidateButtons = currentFunctionButtons.GetComponentsInChildren<Button>();
        activeFunctionButtons.Add(candidateName, currentFunctionButtons);
        BuildButtonDictionary(candidateName, candidateButtons);
        RegisterFunctionButtons(candidateButtons);
        StartCoroutine(StorePositionsAfterFrame(candidateName));
        UpdateCurrentAndOriginalBottomButtons(candidateName);
    }

    private void UpdateCurrentAndOriginalBottomButtons(string candidateName)
    {
        currentBottomButtons[candidateName] = buttonContainers[candidateName].LastOrDefault();
        originalBottomButton = currentBottomButtons[candidateName];
    }

    IEnumerator StorePositionsAfterFrame(string candidateName)
    {
        yield return null;
        StoreOriginalPositions();
    }
    private void StoreOriginalPositions()
    {
        foreach (var pair in buttonContainers)
        {
            foreach (var button in pair.Value)
            {
                originalPositions[button] = button.transform.position;
            }
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
            }
            currentBottomButtons[pair.Key] = pair.Value.Last();
            activeButtons.Clear();
        }
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

        if (selectedButton != bottomButton)
        {
            Vector3 tempPosition = selectedButton.transform.position;
            selectedButton.transform.position = bottomButton.transform.position;
            bottomButton.transform.position = tempPosition;

            currentBottomButtons[candidate] = selectedButton;
            activeButtons[candidate] = selectedButton;
        }
        else
        {
            selectedButton.transform.position = originalPositions[selectedButton];
            currentBottomButtons[candidate] = selectedButton;
            activeButtons.Remove(candidate); 
        }
    }


    public void DestroyFunctionButtons()
    {
        string activeTabName = TabManager.Instance.activeTab?.shortName;
        if (activeTabName != null)
        {
            flagTrailing = true;
            trailingName = TabManager.Instance.activeTab.candidateName;
        }
        if (buttonContainers.Count > 0)
        {
            foreach (var pair in buttonContainers)
            {
                foreach (FunctionButton button in pair.Value)
                {
                    if (button.name != activeTabName)
                    {
                        GameObject.Destroy(button.gameObject);
                    }

                }
            }
            buttonContainers.Clear();
            activeFunctionButtons.Clear();
            currentBottomButtons.Clear();
        }
    }
    public void RemoveLowestGameObjectByName(Transform canvasTransform, string objectName)
    {
        GameObject objectToRemove = null;
        foreach (Transform child in canvasTransform)
        {
            if (child.name == objectName)
            {
                objectToRemove = child.gameObject;  
            }
        }
        if (objectToRemove != null)
        {
            Destroy(objectToRemove);
        }
        else
        {
            Debug.LogError("GameObject not found: " + objectName);
        }
    }
    public void ActivateFunctionButtons(string candidateName)
    {
        DestroyFunctionButtons();
        InstantiateFunctionButtons(candidateName);
        foreach (FunctionButton button in buttonContainers[candidateName])
        {
            button.Activate();
        }
    }
    public void DeactivateFunctionButtons(string candidate)
    {
        CheckAndHandleTrailingFlag();

        string activeTabName = TabManager.Instance.activeTab?.shortName;
        if (buttonContainers.TryGetValue(candidate, out var buttons))
        {
            foreach (FunctionButton button in buttons)
            {
                if (button.name != activeTabName)
                {
                    button.OnButtonClicked -= HandleFunctionButtonEvent;  
                    button.Deactivate();
                }
            }
        }
    }
    public void DeactivateActiveFunctionButton()
    {
        CheckAndHandleTrailingFlag();

        if (activeButton != null) 
        {
            activeButton.OnButtonClicked -= HandleFunctionButtonEvent;  
            activeButton.Deactivate();
        }

        ForceButtonsToOrigin();
    }
    private void CheckAndHandleTrailingFlag()
    {
        if (flagTrailing)
        {
            RemoveLowestGameObjectByName(canvasTransform, "FunctionButtonsParent-" + trailingName + "(Clone)");
            flagTrailing = false;
        }
    }
    private void OnDestroy()
    {
        foreach (var buttons in buttonContainers.Values)
        {
            foreach (FunctionButton button in buttons)
            {
                button.OnButtonClicked -= HandleFunctionButtonEvent;
            }
        }
        if (currentFunctionButtons != null)
        {
            Destroy(currentFunctionButtons);
        }
    }

}
