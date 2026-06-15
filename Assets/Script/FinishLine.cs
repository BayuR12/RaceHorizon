using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    public GameObject winText;

    private bool finished = false;

    private void Start()
    {
        if(winText != null)
        {
            winText.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !finished)
        {
            finished = true;

            winText.SetActive(true);

            Invoke("LoadNextLevel", 3f);
        }
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(2);
    }
}