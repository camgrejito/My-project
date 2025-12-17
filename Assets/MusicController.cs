using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    [Header("Referencias")]
    public AudioSource audioSource;      // El AudioSource que reproduce la música
    public Button playButton;            // Botón de play
    public Button pauseButton;           // Botón de pause
    public Slider progressSlider;        // Slider de progreso

    [Header("Estado")]
    private bool isPlaying = false;      // Si la música está sonando actualmente
    private bool isPaused = false;       // Si la música está en pausa
    private bool isSliderBeingDragged = false; // Tracking del estado del slider por el usuario

    void Start()
    {
        // Validar referencias
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no asignado en MusicController!");
            return;
        }

        if (playButton == null || pauseButton == null)
        {
            Debug.LogError("Botones no asignados en MusicController!");
            return;
        }

        // Configurar eventos de los botones
        playButton.onClick.AddListener(OnPlayButtonPressed);
        pauseButton.onClick.AddListener(OnPauseButtonPressed);

        // Estado inicial: pauseButton deshabilitado
        pauseButton.interactable = false;

        // Configurar slider de progreso
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
            progressSlider.interactable = true; // El usuario puede interactuar con él

            // Suscribirse a eventos para detectar cuando el usuario arrastra o suelta el slider
            progressSlider.onValueChanged.AddListener(OnProgressSliderChanged);
        }

        Debug.Log("MusicController inicializado correctamente");
    }

    // Método cuando se presiona el botón Play
    public void OnPlayButtonPressed()
    {
        if (!isPlaying)
        {
            audioSource.Play();
            isPlaying = true;
            isPaused = false;
            Debug.Log("Música iniciada");
        }
        else if (isPaused)
        {
            audioSource.UnPause();
            isPaused = false;
            Debug.Log("Música reanudada");
        }

        playButton.interactable = false;
        pauseButton.interactable = true;
    }

    // Método cuando se presiona el botón Pause
    public void OnPauseButtonPressed()
    {
        if (isPlaying && !isPaused)
        {
            audioSource.Pause();
            isPaused = true;
            Debug.Log("Música pausada");

            playButton.interactable = true;
            pauseButton.interactable = false;
        }
    }

    // Método para detener completamente (útil para después)
    public void StopMusic()
    {
        audioSource.Stop();
        isPlaying = false;
        isPaused = false;

        playButton.interactable = true;
        pauseButton.interactable = false;

        Debug.Log("Música detenida");
    }

    void Update()
    {
        // Actualizar el slider de progreso SOLO si el usuario no lo está arrastrando
        if (!isSliderBeingDragged && audioSource != null && progressSlider != null && audioSource.clip != null)
        {
            if (audioSource.isPlaying || isPaused)
            {
                float pct = audioSource.time / audioSource.clip.length;
                progressSlider.value = pct;
            }
            else
            {
                if (!audioSource.isPlaying && !isPaused)
                    progressSlider.value = 0f;
            }
        }

        // Detectar si la canción terminó de reproducirse
        if (isPlaying && !audioSource.isPlaying && !isPaused)
        {
            isPlaying = false;
            playButton.interactable = true;
            pauseButton.interactable = false;

            Debug.Log("Canción terminada");
        }
    }

    void OnDestroy()
    {
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayButtonPressed);

        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(OnPauseButtonPressed);

        if (progressSlider != null)
            progressSlider.onValueChanged.RemoveListener(OnProgressSliderChanged);
    }

    // Evento cuando el valor del slider cambia
    private void OnProgressSliderChanged(float value)
    {
        // Si el usuario no está interactuando con el slider, no hacer nada (protegido por isSliderBeingDragged)
        if (!isSliderBeingDragged) return;

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.time = value * audioSource.clip.length;
        }
    }

    // Si usas eventos de UI para detectar cuándo el usuario empieza/termina de arrastrar el slider:
    // En el inspector, agrega estos métodos a los eventos OnPointerDown/OnPointerUp del slider

    public void OnSliderBeginDrag()
    {
        isSliderBeingDragged = true;
    }

    public void OnSliderEndDrag()
    {
        isSliderBeingDragged = false;

        // Al soltar, aplica la posición actual del slider a la canción
        if (audioSource != null && audioSource.clip != null && progressSlider != null)
        {
            audioSource.time = progressSlider.value * audioSource.clip.length;
        }
    }
}
