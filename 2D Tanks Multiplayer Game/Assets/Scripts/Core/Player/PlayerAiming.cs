using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Player
{
    public class PlayerAiming : NetworkBehaviour
    {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Transform _turretTransform;

        private void LateUpdate()
        {
            if (!IsOwner) return;

            Vector2 aimScreenPosition = _inputReader.AimPosition;
            Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

            _turretTransform.up = new Vector2(
                aimWorldPosition.x - _turretTransform.position.x,
                aimWorldPosition.y - _turretTransform.position.y
            );
        }
    }
}