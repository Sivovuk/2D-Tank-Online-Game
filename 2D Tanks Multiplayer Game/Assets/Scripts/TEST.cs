using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class TEST : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;
        
        private void Start()
        {
            _inputReader.MoveEvent += HandleMove;
        }

        private void OnDestroy()
        {
            _inputReader.MoveEvent -= HandleMove;
        }

        private void HandleMove(Vector2 movement)
        {
            Debug.Log(movement);
        }
    }
}