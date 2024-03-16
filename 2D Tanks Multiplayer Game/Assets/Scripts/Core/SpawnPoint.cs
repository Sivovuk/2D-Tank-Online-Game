using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();

    public static Vector3 GetRandomSpawnPos()
    {
        if (_spawnPoints.Count == 0)
        {
            return Vector3.zero;
        }

        return _spawnPoints[Random.Range(0, _spawnPoints.Count)].transform.position;
    }

    private void OnEnable()
    {
        _spawnPoints.Add(this);
    }

    private void OnDisable()
    {
        _spawnPoints.Remove(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
