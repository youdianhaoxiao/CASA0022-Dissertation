using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Sentis;
using TMPro;

public class UseModel : MonoBehaviour
{
    public TextMeshProUGUI corrected_value_AX1;
    public TextMeshProUGUI corrected_value_AY1;
    public TextMeshProUGUI corrected_value_AZ1;
    public TextMeshProUGUI corrected_value_GX1;
    public TextMeshProUGUI corrected_value_GY1;
    public TextMeshProUGUI corrected_value_GZ1;
    public TextMeshProUGUI corrected_value_EMG;
    public TextMeshProUGUI feedbackText;

    [SerializeField] private ModelAsset modelAsset;
    [SerializeField] private float[] results;
    [SerializeField] private float binaryResults;

    private Model runtimeModel;
    private IWorker worker;
    private TensorFloat inputTensor;
    private Root rootDataStored;

    public Transform hand;

    Vector3 handOriginPos;


    // Start is called before the first frame update
    void Start()
    {
        handOriginPos = hand.localPosition;
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(BackendType.CPU, runtimeModel);

        //ExecuteModel();
    }

    //private void ExecuteModel()
    //{
    //    inputTensor?.Dispose();
    //    float[] data = new float[] { 9.96f, 0.81f, -0.68f, -0.14f, 0.09f, -0.04f, 9.91f, 0.81f, -0.72f, -0.15f, 0.09f, -0.04f };
    //    // Create a 3D tensor shape with size 3 × 1 × 3 
    //    TensorShape shape = new TensorShape(1, 12);

    //    // Create a new tensor from the array
    //    inputTensor = new TensorFloat(shape, data);

    //    worker.Execute(inputTensor);
    //    TensorFloat outputTensor = worker.PeekOutput() as TensorFloat;
    //    outputTensor.MakeReadable();
    //    results = outputTensor.ToReadOnlyArray();

    //    ApplyThresholdAndConvertToFloat();
    //}


    public void ExecuteModel(Root rootData) 
    {
        rootDataStored = rootData;
        inputTensor?.Dispose();
        float[] data = new float[] { rootData.AX1, rootData.AY1, rootData.AZ1, rootData.GX1, rootData.GY1,rootData.GZ1};
        //float[] data = new float[] { -5.2f, 5.6f, 6f, -0.1f, 0f, 0f, -5.3f, 5.6f, 6.1f, -0.1f, 0f, 0f };
        // Create a 3D tensor shape with size 3 × 1 × 3
        TensorShape shape = new TensorShape(1, 6);

        // Create a new tensor from the array
        inputTensor = new TensorFloat(shape, data);

        worker.Execute(inputTensor);
        TensorFloat outputTensor = worker.PeekOutput() as TensorFloat;
        outputTensor.MakeReadable();
        results = outputTensor.ToReadOnlyArray();

        ApplyThresholdAndConvertToFloat(rootData);
    }
    bool isMoving = false;
    private void ApplyThresholdAndConvertToFloat(Root rootData)
    {
        //binaryResults = new float[results.Length];
        for (int i = 0; i < results.Length; i++)
        {
            binaryResults = results[i] >= 0.6 ? 1.0f : 0.0f;
            Debug.Log(results[i]);
        }

        if (binaryResults == 1 && !isMoving)
        {
            Debug.Log(binaryResults);
            suggest(rootData);

            StartCoroutine(Movehand());
        }
        else if (binaryResults == 0)
        {
            if (isMoving)
            {
                StopCoroutine("Movehand");
                hand.localPosition = handOriginPos;
                isMoving = false;
            }
            if (feedbackText != null)
            {
                feedbackText.text = "Not doing the exercise.";
            }
        }
    }

    IEnumerator Movehand()
    {
        isMoving = true;
        hand.localPosition = new Vector3(-0.1f * Mathf.Deg2Rad, -0.12f * Mathf.Deg2Rad, -0.03f * Mathf.Deg2Rad);
        corrected_value_AX1.text = (-0.5f).ToString();
        corrected_value_AY1.text = (9.15f).ToString();
        corrected_value_AZ1.text = (7.2f).ToString();
        corrected_value_GX1.text = (-0.1f).ToString();
        corrected_value_GY1.text = (-0.12f).ToString();
        corrected_value_GZ1.text = (-0.03f).ToString();
        corrected_value_EMG.text = (10).ToString();
        yield return new WaitForSeconds(0.5f);
        hand.localPosition = handOriginPos;
        isMoving = false;
    }

    public void suggest(Root rootData)
    {
        string message = ""; 


        if (Mathf.Abs(rootData.GX1 - (-0.1f)) < Mathf.Epsilon)
        {
            message += "Keep this position.\n";
        }
        else if (rootData.GX1 < -0.1f)
        {
            message += "Please tilt your elbow flexion to the left.\n";
        }
        else
        {
            message += "Please tilt your elbow flexion to the right.\n";
        }


        if (Mathf.Abs(rootData.GY1 - (-0.12f)) < Mathf.Epsilon)
        {
            message += "Keep this position.\n";
        }
        else if (rootData.GY1 < -0.12f)
        {
            message += "Please tilt your elbow flexion to the back.\n";
        }
        else
        {
            message += "Please tilt your elbow flexion to the front.\n";
        }

        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
        else
        {
            Debug.LogError("Feedback TextMeshProUGUI component is not assigned!");
        }
    }

    private void OnDisable()
    {
        inputTensor?.Dispose();
        worker.Dispose();
    }
}
