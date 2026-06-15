using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeRemaining = 60f;

    public TMP_Text timerText;

    public GameObject loseText;

    private bool gameEnded = false;

    void Start()
    {
        if(loseText != null)
        {
            loseText.SetActive(false);
        }
    }

    void Update()
    {
        if(gameEnded)
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
        timerText.text =
            "TIME : " +
            Mathf.Ceil(timeRemaining).ToString();
    }

    void LoseGame()
    {
        gameEnded = true;

        loseText.SetActive(true);

        Time.timeScale = 0f;
    }
}