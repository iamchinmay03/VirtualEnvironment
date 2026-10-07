using UnityEngine;
using System.Collections.Generic;

public class VehicleSpawner : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public List<Transform> targetDestinations;
    public List<GameObject> vehiclePrefabs;

    public List<VehicleCounter> vehicleCounters;

    public float spawnInterval = 3f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnVehicle), 1f, spawnInterval);
    }

    void SpawnVehicle()
    {
        if (spawnPoints.Count == 0 || vehiclePrefabs.Count == 0 || targetDestinations.Count == 0)
            return;

        List<int> validIndexes = new List<int>();

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            // check vehicles inside counter trigger
            if (vehicleCounters[i].vehiclesInBox.Count <= 2)
            {
                validIndexes.Add(i);
            }
        }

        if (validIndexes.Count == 0)
            return;

        int index = validIndexes[Random.Range(0, validIndexes.Count)];

        Transform spawnPoint = spawnPoints[index];
        Transform targetDestination = targetDestinations[index];

        GameObject vehiclePrefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Count)];

        GameObject newVehicle = Instantiate(vehiclePrefab, spawnPoint.position, spawnPoint.rotation);

        VehicleMovement vm = newVehicle.GetComponent<VehicleMovement>();

        if (vm != null)
        {
            vm.destination = targetDestination;
        }
    }
}