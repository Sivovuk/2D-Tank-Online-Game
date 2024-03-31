using System;
using Unity.Netcode;
using UnityEngine;

namespace Core.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private Rigidbody2D _rg;
        [SerializeField] private ParticleSystem _dustCloud;

        [Header("Settings")] 
        [SerializeField] private float _movementSpeed = 4f;
        [SerializeField] private float _turningRate = 30f;
        [SerializeField] private float _particleEmmisionValue = 10f;

        private ParticleSystem.EmissionModule _emissionModule;
        private Vector2 _previousMovementInput;
        private Vector3 _previousPos;

        private const float ParticleStopTrasholder = 0.005f;
        
        private void Awake()
        {
            _emissionModule = _dustCloud.emission;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            _inputReader.MoveEvent += HandleMove;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;

            _inputReader.MoveEvent -= HandleMove;
        }

        private void Update()
        {
            if ((transform.position - _previousPos).sqrMagnitude > ParticleStopTrasholder)
            {
                _emissionModule.rateOverTime = _particleEmmisionValue;
            }
            else
            {
                _emissionModule.rateOverTime = 0;
            }
            _previousPos = transform.position;
            
            if (!IsOwner) return;

            float zRotation = _previousMovementInput.x * -_turningRate * Time.deltaTime;
            _bodyTransform.Rotate(0f, 0f, zRotation);
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            _rg.velocity = (Vector2)_bodyTransform.up * (_previousMovementInput.y * _movementSpeed);
        }

        private void HandleMove(Vector2 movement)
        {
            _previousMovementInput = movement;
        }
    }
}