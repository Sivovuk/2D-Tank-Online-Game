using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ShellSelectionDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _antyTankSelecGUI;
    [SerializeField] private GameObject _peircingTankSelecGUI;

    public void Start()
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

    public void SelectShell(int shellID)
    {
        _antyTankSelecGUI.SetActive(false);
        _peircingTankSelecGUI.SetActive(false);
        
        if (shellID == 1)
            _antyTankSelecGUI.SetActive(true);
        else
            _peircingTankSelecGUI.SetActive(true);
    }

}
