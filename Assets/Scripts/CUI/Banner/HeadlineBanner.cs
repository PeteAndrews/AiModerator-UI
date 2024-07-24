using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeadlineBanner : MonoBehaviour
{
    public float bannerDuration = 20f;
    private List<Texture2D> bannerImages;
    private List<string> bannerImageNames;
    private Image referenceImage;
    private TextMeshProUGUI textMain;
    private TextMeshProUGUI textTime;
    private TextMeshProUGUI textImageName;
    public float imageTransitionDuration = 1.5f;
    public AdjustBoxSizetoText dynamicBox;

    void Start()
    {
        referenceImage = FindComponentByName<Image>("Reference-Image");
        textMain = FindComponentByName<TextMeshProUGUI>("Text-Main");
        textTime = FindComponentByName<TextMeshProUGUI>("Text-Time");
        textImageName = FindComponentByName<TextMeshProUGUI>("Text-Image-Name");
        Transform referenceImageTransform = transform.Find("Reference-Image");
        gameObject.SetActive(false);    
    }
    public void UpdateImage(Texture2D newTexture)
    {
        if (referenceImage != null && newTexture != null)
        {
            Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f));
            referenceImage.sprite = newSprite;

            referenceImage.preserveAspect = true;
            RectTransform rectTransform = referenceImage.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                float fixedHeight = rectTransform.rect.height; 
                float fixedWidth = rectTransform.rect.width; 

                rectTransform.sizeDelta = new Vector2(fixedWidth, fixedHeight);  
                rectTransform.anchoredPosition = new Vector2(433.6f, 394.8f);  


            }
        }
    }
    private T FindComponentByName<T>(string gameObjectName) where T : Component
    {
        Transform childTransform = transform.Find(gameObjectName);
        if (childTransform != null)
        {
            T component = childTransform.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            else
            {
                Debug.LogError($"Component of type {typeof(T)} not found in '{gameObjectName}'!");
            }
        }
        else
        {
            Debug.LogError($"No child named '{gameObjectName}' found!");
        }

        return null;
    }
    public void ActivateBanner(List<Texture2D> texture2Ds, List<string> imageNames, string text)
    {

        ShuffleList(texture2Ds, imageNames);

        bannerImages = texture2Ds;
        bannerImageNames = imageNames;

        gameObject.SetActive(true);
        UpdateImage(texture2Ds[0]);
        textMain.text = text;
        textTime.text = DateTime.Now.ToString("HH:mmtt").ToLower();
        StartCoroutine(ActivateBannerCoroutine());
    }
    private void ShuffleList<T, U>(List<T> listT, List<U> listU)
    {
        int count = listT.Count;
        System.Random random = new System.Random(); 

        for (int i = 0; i < count; i++)
        {
            int randIndex = random.Next(i, count);
            (listT[i], listT[randIndex]) = (listT[randIndex], listT[i]);
            (listU[i], listU[randIndex]) = (listU[randIndex], listU[i]);
        }
    }
    private IEnumerator ActivateBannerCoroutine()
    {
        int index = 0;
        float elapsedTime = 0f;

        while (elapsedTime < bannerDuration)
        {
            if (index >= bannerImages.Count) index = 0;
            UpdateImage(bannerImages[index]);
            textImageName.text = bannerImageNames[index];
            StartCoroutine(dynamicBox.UpdateTextBackgroundSize(textImageName));
            index++;
            yield return new WaitForSeconds(imageTransitionDuration);
            elapsedTime += imageTransitionDuration;
        }
        //yield return new WaitForSeconds(bannerDuration);
        gameObject.SetActive(false);
    }

}
