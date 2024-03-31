using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

namespace DefaultNamespace.Networking.Server
{
    public class ServerSingletone : MonoBehaviour
    {
        public ServerGameManager ServerGameManager;
   
        private static ServerSingletone _instance;

        public static ServerSingletone Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<ServerSingletone>();

                if (_instance == null)
                {
                    Debug.LogError("No host instance in the scene");
                    return null;
                }

                return _instance;
            }
        }
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async Task CreateServer(NetworkObject playerPrefab)
        {
            await UnityServices.InitializeAsync();
            
            ServerGameManager = new ServerGameManager
            (
                ApplicationData.IP(),
                ApplicationData.Port(),
                ApplicationData.QPort(),
                NetworkManager.Singleton,
                playerPrefab
            );
        }

        private void OnDestroy()
        {
            ServerGameManager?.Dispose();
        }
    }
}