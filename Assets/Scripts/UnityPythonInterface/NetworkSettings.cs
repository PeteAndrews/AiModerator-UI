using UnityEngine.SceneManagement;
using UnityEngine;

public class NetworkSettings : MonoBehaviour
{
    public static NetworkSettings Instance;

    public string serverIP;
    public int repPort;
    public int pushPort;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Makes this object persistent across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
