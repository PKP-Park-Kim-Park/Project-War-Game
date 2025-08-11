using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Application.targetFrameRate = 60;
    }

    public void StartGame()
    {
        Debug.Log("게임 시작!");
    }

    public void EndGame()
    {
        Debug.Log("게임 종료");
        SceneManager.LoadScene("Title");
    }
}
