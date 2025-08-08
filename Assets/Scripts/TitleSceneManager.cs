using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    public void OnClickStartBuntton()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnClickExitBuntton()
    {
        Application.Quit();
    }
}
