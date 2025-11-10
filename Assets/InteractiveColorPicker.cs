using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InteractiveColorPicker : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    [Header("Referencias UI")]
    public Image colorWheelImage;          // La imagen de la rueda de colores
    public Image selectedColorPreview;     // Preview del color seleccionado

    [Header("Evento de Color")]
    public UnityEvent<Color> OnColorChanged; // Evento cuando cambia el color

    private Texture2D colorWheelTexture;
    private RectTransform rectTransform;
    private Color currentColor = Color.white;

    void Start()
    {
        rectTransform = colorWheelImage.GetComponent<RectTransform>();
        
        // Obtener la textura del sprite
        Sprite sprite = colorWheelImage.sprite;
        if (sprite != null)
        {
            colorWheelTexture = DuplicateTexture(sprite.texture);
        }
        else
        {
            Debug.LogError("ColorWheelImage no tiene sprite asignado!");
        }
    }

    // Duplicar textura para poder leer pixels
    Texture2D DuplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear
        );

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;

        Texture2D readableTexture = new Texture2D(source.width, source.height);
        readableTexture.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);

        return readableTexture;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PickColorAtPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        PickColorAtPosition(eventData);
    }

    void PickColorAtPosition(PointerEventData eventData)
    {
        if (colorWheelTexture == null) return;

        // Convertir posición del click a coordenadas locales del RectTransform
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        // Convertir a coordenadas de textura (0-1)
        Vector2 normalizedPoint = new Vector2(
            (localPoint.x / rectTransform.rect.width) + 0.5f,
            (localPoint.y / rectTransform.rect.height) + 0.5f
        );

        // Clamp para asegurar que está dentro de los límites
        normalizedPoint.x = Mathf.Clamp01(normalizedPoint.x);
        normalizedPoint.y = Mathf.Clamp01(normalizedPoint.y);

        // Obtener coordenadas del pixel en la textura
        int texX = Mathf.RoundToInt(normalizedPoint.x * colorWheelTexture.width);
        int texY = Mathf.RoundToInt(normalizedPoint.y * colorWheelTexture.height);

        // Leer el color del pixel
        Color pickedColor = colorWheelTexture.GetPixel(texX, texY);

        // Si el pixel es transparente, ignorar
        if (pickedColor.a < 0.1f) return;

        // Guardar el color actual
        currentColor = pickedColor;

        // Actualizar preview
        if (selectedColorPreview != null)
        {
            selectedColorPreview.color = pickedColor;
        }

        // Disparar evento para que otros objetos escuchen
        OnColorChanged?.Invoke(pickedColor);

        Debug.Log($"Color seleccionado: RGB({pickedColor.r:F2}, {pickedColor.g:F2}, {pickedColor.b:F2})");
    }

    // Método para establecer color programáticamente
    public void SetColor(Color newColor)
    {
        currentColor = newColor;
        
        if (selectedColorPreview != null)
        {
            selectedColorPreview.color = newColor;
        }

        OnColorChanged?.Invoke(newColor);
    }

    // Método para obtener el color actual
    public Color GetCurrentColor()
    {
        return currentColor;
    }
}
