using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();


    public static Vector3 GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
        {
            return Vector3.zero;
        }
        int randomSpawnIndex = Random.Range(0, spawnPoints.Count - 1);
        return spawnPoints[randomSpawnIndex].transform.position;
    }

    private void OnEnable()
    {
        spawnPoints.Add(this);
    }

    private void OnDisable()
    {
        spawnPoints.Remove(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
