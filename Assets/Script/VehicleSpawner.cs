using UnityEngine;
using System.Collections.Generic;

public class VehicleSpawner : MonoBehaviour
{
    public List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> targetDestinations = new List<Transform>();
    public List<GameObject> vehiclePrefabs = new List<GameObject>();

    public List<VehicleCounter> vehicleCounters = new List<VehicleCounter>();

    public float spawnInterval = 3f;

    readonly List<GameObject> validVehiclePrefabs = new List<GameObject>();
    int laneCount;

    void Start()
    {
        if (spawnInterval <= 0f)
        {
            Debug.LogError("VehicleSpawner requires a spawn interval greater than zero.", this);
            enabled = false;
            return;
        }

        if (spawnPoints == null || targetDestinations == null ||
            vehicleCounters == null || vehiclePrefabs == null)
        {
            Debug.LogError("VehicleSpawner requires spawn points, destinations, counters, and vehicle prefabs.", this);
            enabled = false;
            return;
        }

        foreach (GameObject vehiclePrefab in vehiclePrefabs)
        {
            if (vehiclePrefab != null)
                validVehiclePrefabs.Add(vehiclePrefab);
        }

        if (validVehiclePrefabs.Count == 0)
        {
            Debug.LogError("VehicleSpawner requires at least one non-null vehicle prefab.", this);
            enabled = false;
            return;
        }

        laneCount = Mathf.Min(spawnPoints.Count, targetDestinations.Count, vehicleCounters.Count);
        if (laneCount != spawnPoints.Count || laneCount != targetDestinations.Count ||
            laneCount != vehicleCounters.Count)
        {
            Debug.LogWarning("VehicleSpawner lane lists have different lengths. Entries without a matching spawn point, destination, and counter will be ignored.", this);
        }

        bool hasValidLane = false;
        for (int i = 0; i < laneCount; i++)
        {
            if (spawnPoints[i] != null && targetDestinations[i] != null &&
                vehicleCounters[i] != null && vehicleCounters[i].vehiclesInBox != null)
            {
                hasValidLane = true;
                continue;
            }

            Debug.LogWarning("VehicleSpawner is ignoring an incomplete lane at index " + i + ".", this);
        }

        if (!hasValidLane)
        {
            Debug.LogError("VehicleSpawner requires at least one lane with a spawn point, destination, and counter.", this);
            enabled = false;
            return;
        }

        InvokeRepeating(nameof(SpawnVehicle), 1f, spawnInterval);
    }

    void SpawnVehicle()
    {
        int selectedIndex = -1;
        int availableLaneCount = 0;

        for (int i = 0; i < laneCount; i++)
        {
            Transform spawnPoint = spawnPoints[i];
            Transform targetDestination = targetDestinations[i];
            VehicleCounter vehicleCounter = vehicleCounters[i];

            if (spawnPoint == null || targetDestination == null || vehicleCounter == null ||
                vehicleCounter.vehiclesInBox == null || vehicleCounter.vehiclesInBox.Count > 2)
            {
                continue;
            }

            availableLaneCount++;
            if (Random.Range(0, availableLaneCount) == 0)
                selectedIndex = i;
        }

        if (selectedIndex < 0)
            return;

        Transform selectedSpawnPoint = spawnPoints[selectedIndex];
        Transform targetDestination = targetDestinations[selectedIndex];
        GameObject vehiclePrefab = validVehiclePrefabs[Random.Range(0, validVehiclePrefabs.Count)];

        GameObject newVehicle = Instantiate(vehiclePrefab, selectedSpawnPoint.position, selectedSpawnPoint.rotation);

        VehicleMovement vm = newVehicle.GetComponent<VehicleMovement>();

        if (vm != null)
        {
            vm.destination = targetDestination;
        }
    }
}