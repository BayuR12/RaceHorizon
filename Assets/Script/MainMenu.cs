using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel; 
    [SerializeField] private GameObject settingsPanel; 
    [SerializeField] private Slider volumeSlider;     

    private void Awake()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true); 
        if (settingsPanel != null) settingsPanel.SetActive(false); 
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("EXIT GAME");
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false); 
        settingsPanel.SetActive(true); 
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void SetVolume(float volume)
    {
        if (AudioListener.volume != volume)
        {
             AudioListener.volume = volume;
        }
    }
}