using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Core.Player
{
    public class ShellSelection : NetworkBehaviour
    {
        
        public enum TankShells
        {
            AntyTankShell = 1,
            PeircingShell = 2
        }

        public NetworkVariable<TankShells> ShellSelected = new NetworkVariable<TankShells>();

        [field: SerializeField] public int AntyTankShellCost { get; private set; } = 5;
        [field: SerializeField] public int PeircingShellCost { get; private set; } = 10;


        private void Update()
        {
            if (!IsOwner) return;
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SelectShellServerRpc(1);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                SelectShellServerRpc(2);
        }

        public TankShells GetActiveShell()
        {
            return ShellSelected.Value;
        }

        public int GetActiveShellCost()
        {
            return ShellSelected.Value == TankShells.AntyTankShell ? AntyTankShellCost : PeircingShellCost;
        }

        [ServerRpc]
        public void SelectShellServerRpc(int shellID)
        {
            if (shellID == 1)
                ShellSelected.Value = TankShells.AntyTankShell;
            else
                ShellSelected.Value = TankShells.PeircingShell;
        }
    }
}