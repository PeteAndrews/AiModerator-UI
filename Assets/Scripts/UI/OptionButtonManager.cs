using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
//OnPythonNetworkInput Function needs to take each option from the list
//and create a button for each option, which will mean instantiating a button gameobject
//for each option

//upon clicking the button, the button will send the option text to the python server
//and the python server will send back the response
//and the response will show in the bottom UI banner panel
public class OptionButtonManager : MonoBehaviour
{
    private static OptionButtonManager _instance;
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        globalTimer = GlobalTimer._instance;

    }
    private FrameJsonLoader loader = new FrameJsonLoader();
    private RectTransform canvasTransform;
    private GlobalTimer globalTimer;
    private Vector2 _frameTie;
    public GameObject optionsButtonPrefab;
    public List<int> buttonEventPersons;
    public RectTransform startPosition;

    private void OnEnable()
    {
        RetrievePushData.Instance.OnOptionsEvent += HandlePythonNetworkInput;

    }

    private void OnDisable()
    {
        RetrievePushData.Instance.OnOptionsEvent -= HandlePythonNetworkInput;

    }

    private void HandlePythonNetworkInput(string eventName, string[] text)
    {
        // Handle the event
        Debug.Log($"Received event: {eventName}, with data: {text}");
        OnPythonNetworkInput(text, 1);
    }
    private void OnPythonNetworkInput(string[] options, int eventIndex)
    {
        FrameData frameData = loader.LoadFrameData((int)globalTimer.CurrentFrame);
        foreach (string option in options)
        {
            //instantiate a button gameobject for each option
            GameObject button = Instantiate(optionsButtonPrefab, canvasTransform);
            RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            //set position of button
            if (eventIndex == 0)
            {
                _frameTie = new Vector2(frameData.boxes[2][2], (-1) * frameData.boxes[2][1]);
            }
            else
            {
                _frameTie = new Vector2(frameData.boxes[3][2], (-1) * frameData.boxes[3][1]);
            }
            buttonRectTransform.anchorMin = new Vector2(0, 1);
            buttonRectTransform.anchorMax = new Vector2(0, 1);
            buttonRectTransform.anchoredPosition = _frameTie + new Vector2(240, 50); 
            buttonText.text = option;


            //upon clicking the button, the button will send the option text to the python server
            //and the python server will send back the response
            //and the response will show in the bottom UI banner panel
        }
        // Handle the event
    }
    // Start is called before the first frame update
    void Start()
    {
        //get the canvas RectTransform in parent gameobject
        canvasTransform = transform.parent.GetComponent<RectTransform>();


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
    }
    
}
