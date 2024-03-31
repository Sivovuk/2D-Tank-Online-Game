using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

//  this class is creating and storing host instance
public class HostSingletone : MonoBehaviour
{
    public HostGameManager HostGameManager;
   
    private static HostSingletone _instance;

    public static HostSingletone Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = FindObjectOfType<HostSingletone>();

            if (_instance == null)
            {
                Debug.LogWarning("No host instance in the scene");
                return null;
            }

            return _instance;
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost(NetworkObject playerPrefab)
    {
        HostGameManager = new HostGameManager(playerPrefab);
    }

    private void OnDestroy()
    {
        HostGameManager?.Dispose();
    }
}
