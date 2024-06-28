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
    private Image referenceImage;
    private TextMeshProUGUI textMain;
    private TextMeshProUGUI textTime;

    void Start()
    {
        referenceImage = FindComponentByName<Image>("Reference-Image");
        textMain = FindComponentByName<TextMeshProUGUI>("Text-Main");
        textTime = FindComponentByName<TextMeshProUGUI>("Text-Time");
        Transform referenceImageTransform = transform.Find("Reference-Image");
        gameObject.SetActive(false);    
    }

    public void UpdateImage(Texture2D newTexture)
    {
        if (referenceImage != null && newTexture != null)
        {
            Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f));
            referenceImage.sprite = newSprite;
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
        bannerImages = texture2Ds;
        gameObject.SetActive(true);
        UpdateImage(texture2Ds[0]);
        textMain.text = text;
        textTime.text = DateTime.Now.ToString("HH:mmtt").ToLower();
        StartCoroutine(ActivateBannerCoroutine());

    }
    private IEnumerator ActivateBannerCoroutine()
    {
        yield return new WaitForSeconds(bannerDuration);
        gameObject.SetActive(false);
    }
    

}
