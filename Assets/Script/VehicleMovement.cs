using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    public Transform destination;

    public float speed = 5f;
    public float rotationSpeed = 5f;

    public bool trafficLight = true;

    public float stopDistance = 3f;      // distance from destination to stop at red light
    public float detectDistance = 4f;    // distance to detect car ahead

    bool vehicleAhead = false;

    void Update()
    {
        if (destination == null)
            return;

        CheckVehicleAhead();

        Vector3 targetPos = new Vector3(destination.position.x, transform.position.y, destination.position.z);
        Vector3 direction = targetPos - transform.position;

        float distance = direction.magnitude;

        // Stop if red light and near intersection
        if (!trafficLight && distance <= stopDistance)
            return;

        // Stop if vehicle ahead
        if (vehicleAhead)
            return;

        if (distance > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    void CheckVehicleAhead()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, detectDistance))
        {
            if (hit.collider.CompareTag("Vehicle"))
            {
                vehicleAhead = true;
                return;
            }
        }

        vehicleAhead = false;
    }
}