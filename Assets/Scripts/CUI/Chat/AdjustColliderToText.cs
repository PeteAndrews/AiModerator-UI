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
            yield return new WaitForEndOfFrame();  // Ensures all UI layout calculations are complete
            AdjustColliderSize();
        }
    }

    private void AdjustColliderSize()
    {
        // Update the size to match the RectTransform's dimensions
        boxCollider.size = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

        // Calculate offset based on the pivot to adjust for non-central pivots
        float pivotX = rectTransform.pivot.x;
        float pivotY = rectTransform.pivot.y;
        boxCollider.offset = new Vector2((0.5f - pivotX) * rectTransform.rect.width, (0.5f - pivotY) * rectTransform.rect.height);

        //textMesh.ForceMeshUpdate();
        //var textBounds = textMesh.textBounds;
        //boxCollider.size = new Vector2(textBounds.size.x, textBounds.size.y);
        //boxCollider.offset = new Vector2(textBounds.extents.x - textMesh.transform.localPosition.x,
        //                                 -textBounds.extents.y - textMesh.transform.localPosition.y);
    }
}
