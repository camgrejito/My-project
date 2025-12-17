using UnityEngine;
using UnityEngine.UI;

public class PaletteButton : MonoBehaviour
{
    [Header("Referencias")]
    public FakeDragItem workspaceCopy;  // La copia en el Canvas B
    
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        
        if (button != null)
            button.onClick.AddListener(OnPaletteButtonClicked);
    }

    void OnPaletteButtonClicked()
    {
        // Desactivar este botón
        gameObject.SetActive(false);

        // Activar la copia en el workspace
        if (workspaceCopy != null)
            workspaceCopy.ActivateFromPalette();
    }

    void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnPaletteButtonClicked);
    }
}
