using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    } 

    public void ChangeScene(int _sceneID)
    {
        SceneManager.LoadScene(_sceneID);
    }
}
