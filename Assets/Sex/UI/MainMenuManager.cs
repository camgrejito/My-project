using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject levelSelectorPanel;

    public void Play()
    {
        levelSelectorPanel.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void BackToMain()
    {
        levelSelectorPanel.SetActive(false);
    }

    public void LoadMusicLevel()
    {
        LoadingManager.Instance.LoadSceneWithVideo("BuilderScene"); // Asegúrate que esté en Build Settings
    }
}
