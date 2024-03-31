using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAligner : MonoBehaviour
{
    private ParticleSystem.MainModule _psMain;
    
    void Start()
    {
        _psMain = GetComponent<ParticleSystem>().main;
    }

    void Update()
    {
        _psMain.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }
}
