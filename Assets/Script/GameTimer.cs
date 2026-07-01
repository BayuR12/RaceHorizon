using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Original Settings")]
    public float timeRemaining = 60f;
    public TMP_Text timerText;
    public GameObject loseText;

    [Header("New Guide & Win Settings")]
    public GameObject guidePanel; 
    public GameObject winPanel; 

    private bool gameEnded = false;
    private bool isLevelActive = false;

    void Start()
    {
        if(loseText != null)
        {
            loseText.SetActive(false);
        }

        if(winPanel != null)
        {
            winPanel.SetActive(false);
        }

        // --- LOGIKA PASANG GUIDE DI AWAL LEVEL ---
        if (guidePanel != null)
        {
            guidePanel.SetActive(true);  // Munculkan gambar guide tutorial kamu
            Time.timeScale = 0f;         // Freeze total seluruh game (mobil diam)
            isLevelActive = false;       // Kunci waktu agar tidak berkurang duluan
        }
        else
        {
            // Pengaman: Jika lupa pasang objek guide, game langsung jalan otomatis
            Time.timeScale = 1f;
            isLevelActive = true;
        }
    }

    void Update()
    {
        // MODIFIKASI: Jika game berakhir ATAU level belum aktif (guide masih buka), stop Update
        if(gameEnded || !isLevelActive)
            return;

        timeRemaining -= Time.deltaTime;

        if(timeRemaining <= 0)
        {
            timeRemaining = 0;

            LoseGame();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text =
                "TIME : " +
                Mathf.Ceil(timeRemaining).ToString();
        }
    }

    // =========================================================
    // FUNGSI BARU: DIHUBUNGKAN KE TOMBOL CLOSE/MULAI DI GUIDE PANEL
    // =========================================================
    public void CloseGuide()
    {
        if (guidePanel != null)
        {
            guidePanel.SetActive(false); // Sembunyikan panel panduan gambar
        }
        Time.timeScale = 1f;             // Kembalikan waktu normal (mobil bisa jalan)
        isLevelActive = true;            // Nyalakan hitung mundur timer!
    }

    void LoseGame()
    {
        gameEnded = true;

        if (loseText != null)
        {
            loseText.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    // =========================================================
    // FUNGSI BARU: DIPANGGIL SAAT MOBIL MENYENTUH GARIS FINISH
    // =========================================================
    public void LevelCompleted()
    {
        if (gameEnded || !isLevelActive) return;

        gameEnded = true;

        if (winPanel != null)
        {
            winPanel.SetActive(true); // Munculkan pop-up berhasil menang
        }

        Time.timeScale = 0f; // Freeze game saat menang
    }
}