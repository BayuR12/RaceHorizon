using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
}