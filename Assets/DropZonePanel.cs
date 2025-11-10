using UnityEngine;
[RequireComponent(typeof(RectTransform))]
public class DropZonePanel : MonoBehaviour
{
    private RectTransform rectTransform;
    void Awake() { rectTransform = GetComponent<RectTransform>(); }
    public bool IsPositionOverDropZone(Vector3 worldPosition)
    {
        Vector2 localPoint = rectTransform.InverseTransformPoint(worldPosition);
        return rectTransform.rect.Contains(localPoint);
    }
}