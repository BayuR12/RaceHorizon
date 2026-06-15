using UnityEngine;
using TMPro;

public class CheckpointManager : MonoBehaviour
{
    public int currentCheckpoint = 0;
    public int totalCheckpoints = 4;

    public TMP_Text checkpointText;
    public GameObject winText;

    [Header("Respawn")]
    public Transform currentRespawnPoint;

    void Start()
    {
        UpdateUI();

        if(winText != null)
        {
            winText.SetActive(false);
        }
    }

    public void ReachCheckpoint(Transform checkpointTransform)
    {
        currentCheckpoint++;

        currentRespawnPoint = checkpointTransform;

        UpdateUI();

        if(currentCheckpoint >= totalCheckpoints)
        {
            WinGame();
        }
    }

    void UpdateUI()
    {
        checkpointText.text =
            "Checkpoint : "
            + currentCheckpoint
            + " / "
            + totalCheckpoints;
    }

    void WinGame()
    {
        Debug.Log("YOU WIN!");

        if(winText != null)
        {
            winText.SetActive(true);
        }
    }
}