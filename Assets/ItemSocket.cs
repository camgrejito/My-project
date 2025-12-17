using UnityEngine;
using UnityEngine.UI;

public class ItemSocket : MonoBehaviour
{
    [Header("Visual")]
    public Image socketImage;
    public Color normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    public Color highlightColor = Color.cyan;

    [Header("Estado")]
    private FakeDragItem currentItem;
    private bool isOccupied = false;

    void Start()
    {
        if (socketImage == null)
            socketImage = GetComponent<Image>();
        
        socketImage.color = normalColor;
    }

    public void SetHighlight(bool highlighted)
    {
        if (!isOccupied)
            socketImage.color = highlighted ? highlightColor : normalColor;
    }

    public void PlaceItem(FakeDragItem item)
    {
        currentItem = item;
        isOccupied = true;
        socketImage.color = normalColor;
    }

    public void ReleaseItem()
    {
        currentItem = null;
        isOccupied = false;
        socketImage.color = normalColor;
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public FakeDragItem GetCurrentItem()
    {
        return currentItem;
    }
}
