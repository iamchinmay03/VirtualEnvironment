using UnityEngine;


public class CarCounter : MonoBehaviour
{
    public Unity.InferenceEngine.ModelAsset modelAsset;
    
    public int inputWidth = 640;
    public int inputHeight = 640;

    WebCamTexture webcam;

    Unity.InferenceEngine.Model runtimeModel;
    Unity.InferenceEngine.Worker worker;

    public int vehicleCount = 0;
    public VehicleCounter counter;

    void Start()
    {
        // Start webcam
        webcam = new WebCamTexture();
        webcam.Play();

        // Load Sentis model
        runtimeModel = Unity.InferenceEngine.ModelLoader.Load(modelAsset);

        // Create worker
        worker = new Unity.InferenceEngine.Worker(runtimeModel, Unity.InferenceEngine.BackendType.GPUCompute);
    }

    void Update()
    {
        if (!webcam.isPlaying || webcam.width <= 16)
            return;

        RunModel();
        vehicleCount = counter.vehiclesInBox.Count;
    }

    void RunModel()
    {
        // Convert webcam frame to tensor
        Unity.InferenceEngine.Tensor<float> inputTensor = Unity.InferenceEngine.TextureConverter.ToTensor(webcam, inputWidth, inputHeight, 3);

        // Run inference
        worker.Schedule(inputTensor);

        // Get GPU tensor
        Unity.InferenceEngine.Tensor<float> gpuOutput = worker.PeekOutput() as Unity.InferenceEngine.Tensor<float>;

        if (gpuOutput != null)
        {
            // Copy tensor from GPU to CPU
            Unity.InferenceEngine.Tensor<float> cpuTensor = gpuOutput.ReadbackAndClone();

            ParseOutput(cpuTensor);

            cpuTensor.Dispose();
        }

        inputTensor.Dispose();
    }

    void ParseOutput(Unity.InferenceEngine.Tensor<float> output)
    {
        vehicleCount = 0;

        int detections = output.shape[1];

        for (int i = 0; i < detections; i++)
        {
            float confidence = output[0, i, 4];

            if (confidence < 0.5f)
                continue;

            int classID = Mathf.RoundToInt(output[0, i, 5]);

            // Vehicle classes in COCO dataset
            if (classID == 2 || classID == 3 || classID == 5 || classID == 7)
            {
                vehicleCount++;
            }
        }

       // Debug.Log("Vehicle Count: " + vehicleCount);
    }

    void OnDestroy()
    {
        if (worker != null)
            worker.Dispose();

        if (webcam != null)
            webcam.Stop();
    }
}