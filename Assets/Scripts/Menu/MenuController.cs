using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public TMP_InputField ipInputField;
    public TMP_InputField repPortInputField;
    public TMP_InputField pushPortInputField;
    public TMP_Dropdown sceneDropdown;

    void Start()
    {
        PopulateDropdown();
    }

    void PopulateDropdown()
    {
        sceneDropdown.options.Clear();
        sceneDropdown.options.Add(new TMP_Dropdown.OptionData("Please Select")); // Default prompt
        List<string> scenes = new List<string> { "Walkthrough", "Video National Security", "Video Race & Immigration", 
                                                    "Interactive National Security", "Interactive Race & Immigration" };
        foreach (string scene in scenes)
        {
            sceneDropdown.options.Add(new TMP_Dropdown.OptionData(scene));
        }
    }

    public void SaveSettings()
    {
        NetworkSettings.Instance.serverIP = ipInputField.text;
        int.TryParse(repPortInputField.text, out NetworkSettings.Instance.repPort);
        int.TryParse(pushPortInputField.text, out NetworkSettings.Instance.pushPort);
    }

    public void DropdownIndexChanged(int index)
    {
        SaveSettings();
        string selectedScene = "Scenes/" + sceneDropdown.options[index].text;   
        NetworkSettings.Instance.LoadScene(selectedScene);
    }
}