using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoadingManager : MonoBehaviour
{
    // Arrastra los objetos desde el editor de Unity aquí
    public GameObject loadingScreenPanel; 
    public VideoPlayer videoPlayer;    
    public Renderer videoBackgroundRenderer;   
    public RenderTexture blackTexture;

    // Usamos un Singleton para poder llamar a esta clase fácilmente desde cualquier otro script
    public static LoadingManager Instance;

    private void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
            // Opcional: si quieres que este objeto sobreviva entre escenas
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Esta es la función pública que llamaremos desde nuestro botón del menú
    public void LoadSceneWithVideo(string sceneName)
    {
        // Iniciamos una Coroutine para poder manejar procesos que duran varios frames
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (videoBackgroundRenderer != null && blackTexture != null)
        {
        // Esta línea le dice al material del renderer:
        // "Busca la propiedad de textura '_MainTex' y ponle esta nueva textura negra".
        videoBackgroundRenderer.material.SetTexture("_MainTex", blackTexture); // <-- MODIFICA ESTA LÍNEA
     }
        // 1. Mostrar la pantalla de carga
        loadingScreenPanel.SetActive(true);

        // 2. Preparar el video para una reproducción fluida y esperar a que esté listo
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return null; 
        }

        // 3. Reproducir el video
        videoPlayer.Play();

        // 4. Empezar a cargar la nueva escena en segundo plano
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        
        // ¡LA LÍNEA MÁS IMPORTANTE! Evita que la escena se active automáticamente al llegar al 99%
        asyncOperation.allowSceneActivation = false; 

        // 5. Esperar a que el video termine.
        // La Coroutine se pausará en esta línea hasta que videoPlayer.isPlaying sea falso.
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        // 6. Ahora que el video ha terminado, permitimos que la nueva escena se muestre.
        asyncOperation.allowSceneActivation = true;
    }
}
