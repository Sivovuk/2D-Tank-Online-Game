using UnityEngine.SceneManagement;

public class SceneController
{
    public const string LOGIN_SCENE = "Login";
    public const string LOADING_SCENE = "Loading";
    public const string MAIN_MENU_SCENE = "MainMenu";
    public const string GAME_SCENE = "Game";

    public bool IsActiveScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name == sceneName)
            return true;
        else
            return false;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}