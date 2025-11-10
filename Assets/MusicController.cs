using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    [Header("Referencias")]
    public AudioSource audioSource;      // El AudioSource que reproduce la música
    public Button playButton;            // Botón de play
    public Button pauseButton;           // Botón de pause

    [Header("Estado")]
    private bool isPlaying = false;      // Si la música está sonando actualmente
    private bool isPaused = false;       // Si la música está en pausa

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

        Debug.Log("MusicController inicializado correctamente");
    }

    // Método cuando se presiona el botón Play
    public void OnPlayButtonPressed()
    {
        if (!isPlaying)
        {
            // Si no está sonando nada, iniciar desde el principio
            audioSource.Play();
            isPlaying = true;
            isPaused = false;
            
            Debug.Log("Música iniciada");
        }
        else if (isPaused)
        {
            // Si está en pausa, reanudar
            audioSource.UnPause();
            isPaused = false;
            
            Debug.Log("Música reanudada");
        }

        // Actualizar estados de botones
        playButton.interactable = false;
        pauseButton.interactable = true;
    }

    // Método cuando se presiona el botón Pause
    public void OnPauseButtonPressed()
    {
        if (isPlaying && !isPaused)
        {
            // Pausar la música
            audioSource.Pause();
            isPaused = true;
            
            Debug.Log("Música pausada");

            // Actualizar estados de botones
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
        // Detectar si la canción terminó de reproducirse
        if (isPlaying && !audioSource.isPlaying && !isPaused)
        {
            // La canción terminó
            isPlaying = false;
            playButton.interactable = true;
            pauseButton.interactable = false;

            Debug.Log("Canción terminada");
        }
    }

    void OnDestroy()
    {
        // Limpiar eventos
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayButtonPressed);
        
        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(OnPauseButtonPressed);
    }
}
