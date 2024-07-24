using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

public class ReactButtonHandler : MonoBehaviour
{
    private Button button;
    public string label;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        string id = button.name;
        Transform transform = button.transform;
        CuiManager.Instance.InstantiateReact(id, label, transform);
        Destroy(button.gameObject);
    }
    void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick); 
    }
}
