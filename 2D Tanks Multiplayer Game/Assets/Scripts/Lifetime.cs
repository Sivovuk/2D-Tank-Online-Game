using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float _lifetime = 1f;
    
    private void Start() 
    {
        Destroy(gameObject, _lifetime);
    }
}
