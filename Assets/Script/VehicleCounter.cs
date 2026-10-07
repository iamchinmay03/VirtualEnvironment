using UnityEngine;
using System.Collections.Generic;

public class VehicleCounter : MonoBehaviour
{
    public List<VehicleMovement> vehiclesInBox = new List<VehicleMovement>();

    public bool isGreen = true;

    void OnTriggerEnter(Collider other)
    {
        VehicleMovement car = other.GetComponent<VehicleMovement>();

        if (car != null && !vehiclesInBox.Contains(car))
        {
            vehiclesInBox.Add(car);

            // Apply current traffic light state immediately
            car.trafficLight = isGreen;
        }
    }

    void OnTriggerExit(Collider other)
    {
        VehicleMovement car = other.GetComponent<VehicleMovement>();

        if (car != null)
        {
            vehiclesInBox.Remove(car);

            // Once vehicle leaves the area, allow it to move freely
            car.trafficLight = true;
        }
    }

    public void SetTrafficLight(bool green)
    {
        isGreen = green;

        foreach (VehicleMovement car in vehiclesInBox)
        {
            car.trafficLight = green;
        }
    }
}