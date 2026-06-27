using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;

    [Header("Settings Components")]
    [SerializeField] private Slider volumeSlider;

    private bool isPaused = false;

    private void Start()
    {
        // Pastikan di awal game semua panel pause & settings tersembunyi
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);

        // Sinkronisasi nilai slider dengan volume saat ini
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // 1. FUNGSI RESUME (Melanjutkan Game)
    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(false); 
        Time.timeScale = 1f;           
        isPaused = false;
    }

    // 2. FUNGSI PAUSE (Menghentikan Game)
    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;        
        isPaused = true;
    }

    // 3. FUNGSI RESTART (Mengulang scene masing-masing secara dinamis)
    public void RestartGame()
    {
        Time.timeScale = 1f;
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    // 4. FUNGSI SETTINGS (Buka panel pengatur volume)
    public void OpenSettings()
    {
        pauseMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
    }

    // 5. FUNGSI CLOSE SETTINGS (Kembali dari settings ke menu pause)
    public void CloseSettings()
    {
        settingsMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    // 6. FUNGSI UTAMA VOLUME
    public void SetVolume(float volume)
    {
        if (AudioListener.volume != volume)
        {
            AudioListener.volume = volume;
        }
    }

    // 7. FUNGSI KEMBALI KE MAIN MENU
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); 
    }
}