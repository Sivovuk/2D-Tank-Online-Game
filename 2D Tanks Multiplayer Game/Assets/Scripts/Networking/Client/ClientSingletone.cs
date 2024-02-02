using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingletone : MonoBehaviour
{
   private ClientGameManager _clientGameManager;
   
   private static ClientSingletone instance;

   public static ClientSingletone Instance
   {
      get
      {
         if (instance != null) return instance;

         instance = FindObjectOfType<ClientSingletone>();

         if (instance == null)
         {
            Debug.LogError("No client instance in the scene");
            return null;
         }

         return instance;
      }
   }
   private void Start()
   {
      DontDestroyOnLoad(gameObject);
   }

   public async Task CreateClient()
   {
      _clientGameManager = new ClientGameManager();

      await _clientGameManager.InitAsync();
      
      
   }
}
