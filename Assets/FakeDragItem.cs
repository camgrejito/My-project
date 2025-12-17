using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FakeDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Referencias")]
    public GameObject paletteOriginal;
    public Canvas workspaceCanvas;
    public RectTransform deleteZone;
    public Image deleteZoneImage;
    public Image itemImage;

    [Header("Colores")]
    public Color normalColor = Color.white;
    public Color overSlotColor = Color.cyan;
    public Color overDeleteColor = Color.red;

    // Variables internas
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private RectTransform canvasRectTransform;       // NUEVO
    private Vector2 neutralPosition;
    private Vector2 originalLocalPointerPosition;    // NUEVO
    private Vector2 originalPanelLocalPosition;      // NUEVO
    private ItemSocket currentHoverSocket;
    private bool isOverDeleteZone = false;
    private bool isPlacedInSocket = false;
    private ItemSocket placedSocket;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        neutralPosition = rectTransform.anchoredPosition;

        // Obtener RectTransform del canvas
        if (workspaceCanvas != null)
            canvasRectTransform = workspaceCanvas.GetComponent<RectTransform>();

        gameObject.SetActive(false);
    }

    public void ActivateFromPalette()
    {
        gameObject.SetActive(true);
        rectTransform.anchoredPosition = neutralPosition;
        itemImage.color = normalColor;
        isPlacedInSocket = false;
        placedSocket = null;

        if (deleteZone != null)
            deleteZone.gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;

        // NUEVO: Guardar posición inicial
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out originalLocalPointerPosition
        );
        originalPanelLocalPosition = rectTransform.localPosition;

        if (isPlacedInSocket && placedSocket != null)
        {
            placedSocket.ReleaseItem();
            isPlacedInSocket = false;
            placedSocket = null;
        }

        if (deleteZone != null)
        {
            deleteZone.gameObject.SetActive(true);
            deleteZoneImage.color = new Color(1f, 0f, 0f, 0.3f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // CORREGIDO: Usar ScreenPointToLocalPointInRectangle
        Vector2 localPointerPosition;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition
        ))
        {
            Vector2 offset = localPointerPosition - originalLocalPointerPosition;
            rectTransform.localPosition = (Vector3)originalPanelLocalPosition + (Vector3)offset;
        }

        CheckDeleteZone();
        CheckSocketHover(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (deleteZone != null)
            deleteZone.gameObject.SetActive(false);

        if (isOverDeleteZone)
        {
            ReturnToPalette();
            return;
        }

        if (currentHoverSocket != null && !currentHoverSocket.IsOccupied())
        {
            PlaceInSocket(currentHoverSocket);
            return;
        }

        rectTransform.anchoredPosition = neutralPosition;
        itemImage.color = normalColor;
    }

    void CheckDeleteZone()
    {
        if (deleteZone == null) return;

        Vector3[] corners = new Vector3[4];
        deleteZone.GetWorldCorners(corners);

        Vector3 itemPos = rectTransform.position;

        isOverDeleteZone = itemPos.x >= corners[0].x && itemPos.x <= corners[2].x &&
                          itemPos.y >= corners[0].y && itemPos.y <= corners[2].y;

        if (isOverDeleteZone)
        {
            itemImage.color = overDeleteColor;
            deleteZoneImage.color = new Color(1f, 0f, 0f, 0.7f);
        }
        else if (currentHoverSocket == null)
        {
            itemImage.color = normalColor;
            deleteZoneImage.color = new Color(1f, 0f, 0f, 0.3f);
        }
    }

    void CheckSocketHover(PointerEventData eventData)
    {
        if (isOverDeleteZone) return;

        ItemSocket newSocket = null;

        if (eventData.pointerEnter != null)
        {
            newSocket = eventData.pointerEnter.GetComponent<ItemSocket>();
            if (newSocket == null)
                newSocket = eventData.pointerEnter.GetComponentInParent<ItemSocket>();
        }

        if (newSocket != currentHoverSocket)
        {
            if (currentHoverSocket != null)
                currentHoverSocket.SetHighlight(false);

            currentHoverSocket = newSocket;

            if (currentHoverSocket != null && !currentHoverSocket.IsOccupied())
            {
                currentHoverSocket.SetHighlight(true);
                itemImage.color = overSlotColor;
            }
            else
            {
                itemImage.color = normalColor;
            }
        }
    }

    void PlaceInSocket(ItemSocket socket)
    {
        rectTransform.position = socket.transform.position;
        socket.PlaceItem(this);
        isPlacedInSocket = true;
        placedSocket = socket;
        socket.SetHighlight(false);
        itemImage.color = normalColor;
        currentHoverSocket = null;
    }

    void ReturnToPalette()
    {
        gameObject.SetActive(false);
        rectTransform.anchoredPosition = neutralPosition;
        itemImage.color = normalColor;

        if (paletteOriginal != null)
            paletteOriginal.SetActive(true);

        isOverDeleteZone = false;
        currentHoverSocket = null;
    }
}
