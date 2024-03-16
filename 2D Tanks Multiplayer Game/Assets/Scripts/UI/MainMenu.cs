using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _jointCodeField;
    
    public async void StartHost()
    {
        await HostSingletone.Instance.HostGameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingletone.Instance.ClientGameManager.StartClientAsync(_jointCodeField.text);
    }
}
