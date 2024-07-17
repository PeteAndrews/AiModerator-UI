using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeadlineBanner : MonoBehaviour
{
    public float bannerDuration = 20f;
    private List<Texture2D> bannerImages;
    private Image referenceImage;
    private TextMeshProUGUI textMain;
    private TextMeshProUGUI textTime;
    public float imageTransitionDuration = 1.5f;            //Qingyuan: duration for each image in the banner

    void Start()
    {
        referenceImage = FindComponentByName<Image>("Reference-Image");
        textMain = FindComponentByName<TextMeshProUGUI>("Text-Main");
        textTime = FindComponentByName<TextMeshProUGUI>("Text-Time");
        Transform referenceImageTransform = transform.Find("Reference-Image");
        gameObject.SetActive(false);
        
        bannerImages = new List<Texture2D>();
        //Qingyuan: read local image file and activate the banner
        //read file "Assets/data/Example_Banner_Image", add all image files to a list
        LoadAllTexturesFromFolder("Assets/data/Example_Banner_Image");
        ActivateBanner(bannerImages, "Example Text");
    }

    //Qingyuan: Help Function for loading all image files in a folder
    void LoadAllTexturesFromFolder(string path)
    {
        string[] files = Directory.GetFiles(path, "*.png");

        foreach (string file in files)
        {
            Texture2D texture = LoadTexture(file);
            if (texture != null)
            {
                bannerImages.Add(texture);
            }
        }
    }
    
    //Qingyuan: Help Function for loading local image files
    Texture2D LoadTexture(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);

        Texture2D texture = new Texture2D(2, 2);

        if (texture.LoadImage(fileData))
        {
            return texture;
        }
        else
        {
            Debug.LogWarning("Failed to load texture: " + filePath);
            return null;
        }
    }
    
    public void UpdateImage(Texture2D newTexture)
    {
        if (referenceImage != null && newTexture != null)
        {
            Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0f, 0.5f));
            referenceImage.sprite = newSprite;
            
            RectTransform rectTransform = referenceImage.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                // set to fixed height
                float fixedHeight = rectTransform.rect.height;

                // calculate width based on aspect ratio
                float aspectRatio = (float)newTexture.width / newTexture.height;
                float calculatedWidth = fixedHeight * aspectRatio;
                
                // set anchor and pivot to left, keep the left position
                rectTransform.anchorMin = new Vector2(0, 0.5f); 
                rectTransform.anchorMax = new Vector2(0, 0.5f); 
                rectTransform.pivot = new Vector2(0, 0.5f);     
                
                rectTransform.sizeDelta = new Vector2(calculatedWidth, fixedHeight);
                rectTransform.anchoredPosition = new Vector2(438, 434);
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
    public void ActivateBanner(List<Texture2D> texture2Ds, string text)
    {
        //WANT TO SHUFFLE AND TIME TRANSITIONS BETWEEN IMAGES IN BANNER
        bannerImages = ShuffleList(texture2Ds); //Qingyuan: Shuffle the list of images
        gameObject.SetActive(true);
        UpdateImage(texture2Ds[0]);
        textMain.text = text;
        textTime.text = DateTime.Now.ToString("HH:mmtt").ToLower();
        StartCoroutine(ActivateBannerCoroutine());

    }
    
    private List<T> ShuffleList<T>(List<T> list)
    {
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            System.Random random = new System.Random();
            int randIndex = random.Next(i, count);
            (list[i], list[randIndex]) = (list[randIndex], list[i]);
        }
        return list;
    }
    
    private IEnumerator ActivateBannerCoroutine()
    {
        int index = 0;
        float elapsedTime = 0f;
        
        while (elapsedTime < bannerDuration)
        {
            if(index >= bannerImages.Count) index = 0;
            UpdateImage(bannerImages[index]);
            index++;
            yield return new WaitForSeconds(imageTransitionDuration);
            elapsedTime += imageTransitionDuration;
        }
        //yield return new WaitForSeconds(bannerDuration);
        gameObject.SetActive(false);
    }
    
    
    
}
