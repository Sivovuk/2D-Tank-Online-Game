using System;
using System.Collections;
using System.Threading.Tasks;
using DefaultNamespace.Networking.Server;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public const string LOGIN_SCENE = "Login";
    public const string LOADING_SCENE = "Loading";
    public const string MAIN_MENU_SCENE = "MainMenu";
    public const string GAME_SCENE = "Game";

    public static SceneController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
    
    
    public IEnumerator LoadGameSceneAsync(ServerSingletone serverSingletone, NetworkObject playerPrefab)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GAME_SCENE);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        
        Task createServerTask = serverSingletone.CreateServer(playerPrefab);
        yield return new WaitUntil(() => createServerTask.IsCompleted);

        Task startServerTask = serverSingletone.ServerGameManager.StartServerAsync();
        yield return new WaitUntil(() => startServerTask.IsCompleted);
    }
}