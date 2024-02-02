using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public enum TankShells
{
    AntyTankShell = 1,
    PeircingShell = 2
}

public class ShellSelection : NetworkBehaviour
{
    [field: SerializeField] public int AntyTankShellCost { get; private set; } = 5;
    [field: SerializeField] public int PeircingShellCost { get; private set; } = 10;

    [SerializeField] private GameObject _antyTankSelecGUI;
    [SerializeField] private GameObject _peircingTankSelecGUI;

    [SerializeField] private TankShells _shellSelected = TankShells.AntyTankShell;

    public override void OnNetworkSpawn()
    {
        SelectShell(1);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            SelectShell(1);
        else if(Input.GetKeyDown(KeyCode.Alpha2))
            SelectShell(2);
    }

    public TankShells GetActiveShell()
    {
        return _shellSelected;
    }
    
    public int GetActiveShellCost()
    {
        return _shellSelected == TankShells.AntyTankShell ? AntyTankShellCost : PeircingShellCost ;
    }

    public void SelectShell(int shellID)
    {
        _antyTankSelecGUI.SetActive(false);
        _peircingTankSelecGUI.SetActive(false);
        
        if (shellID == 1)
        {
            _shellSelected = TankShells.AntyTankShell;
            _antyTankSelecGUI.SetActive(true);
        }
        else
        {
            _shellSelected = TankShells.PeircingShell;
            _peircingTankSelecGUI.SetActive(true);
        }
    }

}
