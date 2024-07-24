using System.Collections;
using TMPro;
using UnityEngine;

//[RequireComponent(typeof(TextMeshProUGUI))]
//[RequireComponent(typeof(BoxCollider2D))]
public class AdjustColliderToText : MonoBehaviour
{
    [SerializeField]private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private BoxCollider2D boxCollider;


    public IEnumerator UpdateColliderSizeAfterFrame()
    {
        {
            yield return new WaitForEndOfFrame();  
            AdjustColliderSize();
        }
    }

    private void AdjustColliderSize()
    {
        boxCollider.size = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

        float pivotX = rectTransform.pivot.x;
        float pivotY = rectTransform.pivot.y;
        boxCollider.offset = new Vector2((0.5f - pivotX) * rectTransform.rect.width, (0.5f - pivotY) * rectTransform.rect.height);
;
    }
    
}
