using UnityEngine;
using UnityEngine.UI;

public class ColorableUIElement : MonoBehaviour
{
    [Header("Referencias")]
    public InteractiveColorPicker colorPicker; // Referencia al color picker del otro canvas
    
    private Image targetImage; // La imagen que cambiará de color (este mismo GameObject)

    void Start()
    {
        // Obtener el componente Image de este GameObject
        targetImage = GetComponent<Image>();
        
        if (targetImage == null)
        {
            Debug.LogError("ColorableUIElement necesita un componente Image!");
            return;
        }

        // Suscribirse al evento del color picker
        if (colorPicker != null)
        {
            colorPicker.OnColorChanged.AddListener(ChangeColor);
            Debug.Log("ColorableUIElement suscrito al color picker");
        }
        else
        {
            Debug.LogWarning("Color picker no asignado en ColorableUIElement");
        }
    }

    // Método que se ejecuta cuando el color picker cambia de color
    void ChangeColor(Color newColor)
    {
        if (targetImage != null)
        {
            targetImage.color = newColor;
            Debug.Log($"Color cambiado a: RGB({newColor.r:F2}, {newColor.g:F2}, {newColor.b:F2})");
        }
    }

    void OnDestroy()
    {
        // Desuscribirse del evento cuando se destruya el objeto
        if (colorPicker != null)
        {
            colorPicker.OnColorChanged.RemoveListener(ChangeColor);
        }
    }

    // Método público para cambiar color manualmente si es necesario
    public void SetColor(Color newColor)
    {
        if (targetImage != null)
        {
            targetImage.color = newColor;
        }
    }
}
