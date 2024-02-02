using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingletone : MonoBehaviour
{
    private HostGameManager _clientGameManager;
   
    private static HostSingletone instance;

    public static HostSingletone Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindObjectOfType<HostSingletone>();

            if (instance == null)
            {
                Debug.LogError("No host instance in the scene");
                return null;
            }

            return instance;
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        _clientGameManager = new HostGameManager();
    }
}
