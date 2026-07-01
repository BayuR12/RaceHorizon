using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class CheckpointManager : MonoBehaviour
{
    [Header("Guide Settings")]
    public GameObject guidePanel;   

    [Header("Checkpoint Settings")]
    public int currentCheckpoint = 0;  
    public int totalCheckpoints = 4;  
    public TMP_Text checkpointText;  
    public GameObject winText;  

    [Header("Respawn")]
    public Transform currentRespawnPoint; 

    private bool gameEnded = false;     
    private bool isLevelActive = false;

    void Start()
    {
        currentCheckpoint = 0;
        UpdateUI();

        if (winText != null)
        {
            winText.SetActive(false);
        }

        if (guidePanel != null)
        {
            guidePanel.SetActive(true);  
            Time.timeScale = 0f;       
            isLevelActive = false;    
        }
        else
        {
            Time.timeScale = 1f;
            isLevelActive = true;
        }
    }

    public void CloseGuide()
    {
        if (guidePanel != null)
        {
            guidePanel.SetActive(false); 
        }
        Time.timeScale = 1f;        
        isLevelActive = true;       
    }

    public void ReachCheckpoint(Transform checkpointTransform)
    {
        if (gameEnded || !isLevelActive) return; 

        currentCheckpoint++;
        currentRespawnPoint = checkpointTransform;

        UpdateUI();

        if (currentCheckpoint >= totalCheckpoints)
        {
            WinGame();
        }
    }

    void UpdateUI()
    {
        if (checkpointText != null)
        {
            checkpointText.text = "Checkpoint : " + currentCheckpoint + " / " + totalCheckpoints;
        }
    }

    void WinGame()
    {
        gameEnded = true;
        Debug.Log("GAME TAMAT! Menunggu 3 detik lalu kembali ke Main Menu...");

        if (winText != null)
        {
            winText.SetActive(true); 
        }

        Time.timeScale = 0f; 

        Invoke("BackToMainMenu", 3f);
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene(0); 
    }
}