using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourLerp : MonoBehaviour
{
    public Image targetImage;       
    public Color endColour;   
    public float transitionTime = 1f; 

    private Color startColour;       

    void Start()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        if (targetImage != null)
        {
            startColour = targetImage.color;
        }
        else
        {
            Debug.LogError("No Image component found on the GameObject.");
        }
        ChangeColour();
    }
    public void ChangeColour()
    {
        StartCoroutine(LerpColour());
    }

    IEnumerator LerpColour()
    {
        float time = 0;
        while (time < transitionTime)
        {
            targetImage.color = Color.Lerp(startColour, endColour, time / transitionTime);
            time += Time.deltaTime;
            yield return null;  
        }

        targetImage.color = endColour;
    }
}
