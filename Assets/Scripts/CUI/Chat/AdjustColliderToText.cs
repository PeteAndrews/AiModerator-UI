using System.Collections;
using TMPro;
using UnityEngine;

//[RequireComponent(typeof(TextMeshProUGUI))]
//[RequireComponent(typeof(BoxCollider2D))]
public class AdjustColliderToText : MonoBehaviour
{ 

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
        textMesh.ForceMeshUpdate();
        var textBounds = textMesh.textBounds;
        boxCollider.size = new Vector2(textBounds.size.x, textBounds.size.y);
        //boxCollider.offset = new Vector2(textBounds.extents.x - textMesh.transform.localPosition.x,
        //                                 -textBounds.extents.y - textMesh.transform.localPosition.y);
    }
}
