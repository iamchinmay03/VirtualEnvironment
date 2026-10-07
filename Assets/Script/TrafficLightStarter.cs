using UnityEngine;
using System.Collections.Generic;

public class TrafficLightStarter : MonoBehaviour
{
    public List<TrafficLight> trafficLights;

    void Start()
    {
        foreach (TrafficLight light in trafficLights)
        {
            if (light != null && light.gameObject.activeInHierarchy)
            {
                light.StartCycle();
            }
        }
    }
}