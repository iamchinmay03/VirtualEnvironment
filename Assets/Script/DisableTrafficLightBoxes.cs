using UnityEngine;

public class DisableTrafficLightBoxes : MonoBehaviour
{
    void Start()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "TrafficLight1Box")
            {
                MeshRenderer mr = obj.GetComponent<MeshRenderer>();

                if (mr != null)
                {
                    mr.enabled = false;
                }
            }
        }
    }
}