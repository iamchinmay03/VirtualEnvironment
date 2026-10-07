using UnityEngine;
using System.Collections.Generic;

public class NextPath : MonoBehaviour
{
    public List<Transform> possiblePaths;

    public float pathUpdateCooldown = 1f;

    Dictionary<VehicleMovement, float> lastUpdateTime = new Dictionary<VehicleMovement, float>();

    void OnTriggerEnter(Collider other)
    {
        VehicleMovement car = other.GetComponent<VehicleMovement>();

        if (car == null || possiblePaths.Count == 0)
            return;

        // check cooldown
        if (lastUpdateTime.ContainsKey(car))
        {
            if (Time.time - lastUpdateTime[car] < pathUpdateCooldown)
                return;
        }

        int randomIndex = Random.Range(0, possiblePaths.Count);
        car.destination = possiblePaths[randomIndex];

        lastUpdateTime[car] = Time.time;
    }
}