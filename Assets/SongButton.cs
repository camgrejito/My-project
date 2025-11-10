using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events; // Necesario para Unity Events

// Creamos eventos personalizados para notificar a la UI cuando algo cambie.
// Uno para cuando la canción cambia (envía el nombre y el artista).
[System.Serializable]
public class OnSongChangedEvent : UnityEvent<string, string> {}

// Otro para cuando el estado de reproducción cambia (play/pause).
[System.Serializable]
public class OnPlaybackStateChangedEvent : UnityEvent<bool> {}

public class MusicManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    // Esto permite que cualquier script acceda al MusicManager fácilmente
    // usando "MusicManager.Instance"
    public static MusicManager Instance;

    // --- Referencias y Playlist ---
    [Tooltip("El componente que realmente reproduce el sonido.")]
    public AudioSource audioSource;
    
    [Tooltip("Arrastra todos tus archivos de audio aquí en el orden que desees.")]
    public List<AudioClip> songPlaylist;

    // --- Eventos para la UI ---
    // La UI se puede suscribir a estos eventos para actualizarse automáticamente.
    public OnSongChangedEvent OnSongChanged;
    public OnPlaybackStateChangedEvent OnPlaybackStateChanged;

    // --- Variables de Estado ---
    private int currentSongIndex = 0;
    private bool isPlaying = false;

    void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Carga la primera canción al iniciar, pero no la reproduce.
        if (songPlaylist.Count > 0)
        {
            audioSource.clip = songPlaylist[currentSongIndex];
            // Notifica a la UI sobre la canción inicial
            OnSongChanged.Invoke(songPlaylist[currentSongIndex].name, "Artista Desconocido");
        }
    }

    // --- Funciones Públicas de Control ---

    public void PlaySpecificSong(int songIndex)
    {
        if (songIndex >= 0 && songIndex < songPlaylist.Count)
        {
            currentSongIndex = songIndex;
            audioSource.clip = songPlaylist[currentSongIndex];
            audioSource.Play();
            isPlaying = true;

            // Notifica a la UI sobre el cambio de canción y estado
            OnSongChanged.Invoke(audioSource.clip.name, "Artista Desconocido");
            OnPlaybackStateChanged.Invoke(isPlaying);
        }
    }

    public void PlayPause()
    {
        if (isPlaying)
        {
            audioSource.Pause();
            isPlaying = false;
        }
        else
        {
            audioSource.Play();
            isPlaying = true;
        }
        // Notifica a la UI sobre el cambio de estado
        OnPlaybackStateChanged.Invoke(isPlaying);
    }
}
