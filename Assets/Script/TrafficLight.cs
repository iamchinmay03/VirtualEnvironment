using UnityEngine;
using System.Collections;
using TMPro;
[DefaultExecutionOrder(-100)]
public class TrafficLight : MonoBehaviour
{
    VehicleCounter counter;
    public TrafficLight nextTrafficLight;

    public float baseGreenTime = 3f;
    public float timePerVehicle = 1f;
    public float yellowTime = 2f;

    Animator animator;
    TMP_Text infoText;

    bool isRunning = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        infoText = GetComponentInChildren<TMP_Text>();
        counter = GetComponentInChildren<VehicleCounter>();
    }

    public void StartCycle()
    {
        if (!isRunning)
            StartCoroutine(TrafficRoutine());
    }

    IEnumerator TrafficRoutine()
    {
        isRunning = true;

        int vehicleCount = counter.vehiclesInBox.Count;
        float greenTime = baseGreenTime + vehicleCount * timePerVehicle;

        // RED → YELLOW → GREEN
        if (animator != null)
            animator.SetTrigger("ToGreen");

        if (infoText != null)
            infoText.text = "YELLOW";

        yield return new WaitForSeconds(yellowTime);

        // GREEN state starts
        counter.SetTrafficLight(true);

        float timer = greenTime;

        while (timer > 0)
        {
            if (infoText != null)
                infoText.text = "Car Count: " + vehicleCount + "\nGreen: " + timer.ToString("F1");

            timer -= Time.deltaTime;
            yield return null;
        }

        // GREEN → YELLOW → RED
        if (animator != null)
            animator.SetTrigger("ToRed");

        if (infoText != null)
            infoText.text = "YELLOW";

        yield return new WaitForSeconds(yellowTime);

        counter.SetTrafficLight(false);

        if (infoText != null)
            infoText.text = "RED";

        isRunning = false;

        if (nextTrafficLight != null)
            nextTrafficLight.StartCycle();
    }
}