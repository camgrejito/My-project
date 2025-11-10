using UnityEngine;
using UnityEngine.UI; // Necesario para interactuar con componentes de UI como Button

// Esta línea es una buena práctica: asegura que este script solo se pueda
// añadir a un GameObject que ya tenga un componente "Button".
[RequireComponent(typeof(Button))]
public class PlaySoundOnClick : MonoBehaviour
{
    [Header("Configuración de Sonido")]
    [Tooltip("Arrastra aquí el archivo de la canción que quieres reproducir.")]
    public AudioClip songToPlay;

    [Tooltip("Arrastra aquí el componente Audio Source que usaremos como altavoz.")]
    public AudioSource audioSource;

    private Button button;

    void Awake()
    {
        // Al iniciar, encontramos el componente Button en este mismo GameObject.
        button = GetComponent<Button>();

        // Le decimos al botón: "Cuando alguien te haga clic, ejecuta mi función 'PlayTheSong'".
        button.onClick.AddListener(PlayTheSong);
    }

    void PlayTheSong()
    {
        // 1. Verificamos que todo esté asignado para no tener errores.
        if (audioSource != null && songToPlay != null)
        {
            // 2. Le asignamos nuestra canción al "altavoz" (Audio Source).
            audioSource.clip = songToPlay;

            // 3. ¡Le damos al play!
            audioSource.Play();
        }
        else
        {
            // Un mensaje de error útil si olvidamos conectar algo en el Inspector.
            Debug.LogWarning("Falta asignar el AudioClip o el AudioSource en el botón: " + gameObject.name);
        }
    }

    // Es bueno "limpiar" el listener si el objeto se destruye para evitar problemas de memoria.
    void OnDestroy()
    {
        button.onClick.RemoveListener(PlayTheSong);
    }
}