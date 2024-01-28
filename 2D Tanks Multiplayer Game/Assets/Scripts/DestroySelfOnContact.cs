using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        Debug.LogError(other.name);
        Destroy(gameObject);
    }
}
