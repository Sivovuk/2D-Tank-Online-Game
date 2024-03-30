using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace.Networking.Server;
using Networking.Client;
using UnityEngine;

//  ApplicationController is the main scripts for setting up Server, Host and Client side
public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingletone _clientPrefab;
    [SerializeField] private HostSingletone _hostPrefab;
    [SerializeField] private ServerSingletone _serverPrefab;

    private ApplicationData _applicationData;
    
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        //  this sets graphics to null if is dedicated server because there is no player in server so rendering is not needed
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        
        
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        //  proveravamo ako je server onda samo izvrsamo serverski stranu koda
        if (isDedicatedServer)
        {
            _applicationData = new ApplicationData();
            
            ServerSingletone serverSingletone = Instantiate(_serverPrefab);
            await serverSingletone.CreateServer();

             await serverSingletone.ServerGameManager.StartServerAsync();
        }
        //  ako ne onda izvrsavamo Client i Host stranu koda
        else
        {
            HostSingletone hostSingletone = Instantiate(_hostPrefab);
            hostSingletone.CreateHost();
            
            ClientSingletone clientSingletone = Instantiate(_clientPrefab);
            bool authentication = await clientSingletone.CreateClient();

            if (authentication)
            {
                GameManager.SceneController.LoadScene(SceneController.MAIN_MENU_SCENE);
            }
        }
    }
}
