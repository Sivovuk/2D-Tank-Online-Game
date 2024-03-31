using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Networking.Client
{
//  this class is creating and storing client instance
   public class ClientSingletone : MonoBehaviour
   {
      public ClientGameManager ClientGameManager { get; private set; }

      private static ClientSingletone _instance;

      public static ClientSingletone Instance
      {
         get
         {
            if (_instance != null) return _instance;

            _instance = FindObjectOfType<ClientSingletone>();

            if (_instance == null)
            {
               Debug.LogWarning("No client instance in the scene");
               return null;
            }

            return _instance;
         }
      }

      private void Start()
      {
         DontDestroyOnLoad(gameObject);
      }

      public async Task<bool> CreateClient()
      {
         ClientGameManager = new ClientGameManager();

         return await ClientGameManager.InitAsync();


      }

      private void OnDestroy()
      {
         ClientGameManager?.Dispose();
      }
   }
}