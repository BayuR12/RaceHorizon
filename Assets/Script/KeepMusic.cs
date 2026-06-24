using UnityEngine;

public class KeepAudio : MonoBehaviour
{
    private static KeepAudio instance;

    void Awake()
    {
        // Sistem Singleton: Mencegah musik menjadi double/kembar saat kembali ke Main Menu
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Memerintahkan Unity agar jangan menghapus objek ini saat pindah scene
        DontDestroyOnLoad(gameObject);
    }
}