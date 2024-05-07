using UnityEngine;
using UnityEngine.UI; // Required for accessing UI components like Button
using UnityEngine.Events; // For UnityEvent

public class OptionsButton : MonoBehaviour
{

    public delegate void ButtonClickHandler(string buttonName);
    public event ButtonClickHandler OnButtonClicked;

    void Start()
    {
        var button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => {
                Debug.Log("Button clicked: " + gameObject.name);
                OnButtonClicked?.Invoke(gameObject.name);
            });
        }
        else
        {
            Debug.LogError("Button component not found on the object!");
        }
    }

}