using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class mqttController : MonoBehaviour
{
    public string nameController = "Controller 1";
    public string tag_mqttManager = "";
    public mqttManager _eventSender;
    public TextMeshProUGUI Realtime_value_AX1;
    public TextMeshProUGUI Realtime_value_AY1;
    public TextMeshProUGUI Realtime_value_AZ1;
    public TextMeshProUGUI Realtime_value_GX1;
    public TextMeshProUGUI Realtime_value_GY1;
    public TextMeshProUGUI Realtime_value_GZ1;
    public TextMeshProUGUI Realtime_value_EMG;
    public TextMeshProUGUI Realtime_value_temp;
    public Transform sphere;
    public Transform startsphere;
    //public TextMeshProUGUI Realtime_value_AX2;
    //public TextMeshProUGUI Realtime_value_AY2;
    //public TextMeshProUGUI Realtime_value_AZ2;
    //public TextMeshProUGUI Realtime_value_GX2;
    //public TextMeshProUGUI Realtime_value_GY2;
    //public TextMeshProUGUI Realtime_value_GZ2;

    public UseModel useModel;
    public GameObject hand;

    int i = 0;
    Root r = new Root();
    static AHRS.MadgwickAHRS AHRS = new AHRS.MadgwickAHRS(0.1f, 0.1f);

    void Start()
    {
        if (GameObject.FindGameObjectsWithTag(tag_mqttManager).Length > 0)
        {
            _eventSender = GameObject.FindGameObjectsWithTag(tag_mqttManager)[0].gameObject.GetComponent<mqttManager>();
        }
        else
        {
            Debug.LogError("At least one GameObject with mqttManager component and Tag == tag_mqttManager needs to be provided");
        }
        _eventSender.OnMessageArrived += OnMessageArrivedHandler;
    }

    private void OnMessageArrivedHandler(mqttObj mqttObject)
    {
       
        if (mqttObject.topic.Contains("AX1"))
        {
            startsphere.GetComponent<Renderer>().material.color = Color.blue;
            Realtime_value_AX1.text = (float.Parse(mqttObject.msg)).ToString();
            r.AX1 = float.Parse(mqttObject.msg);


        }
        if (mqttObject.topic.Contains("AY1"))
        {
            Realtime_value_AY1.text = (float.Parse(mqttObject.msg)).ToString();
            r.AY1 = float.Parse(mqttObject.msg);
        }
        if (mqttObject.topic.Contains("AZ1"))
        {
            Realtime_value_AZ1.text = (float.Parse(mqttObject.msg)).ToString();
            r.AZ1 = float.Parse(mqttObject.msg);
        }
        if (mqttObject.topic.Contains("GX1"))
        {
            Realtime_value_GX1.text = (float.Parse(mqttObject.msg)).ToString();
            r.GX1 = float.Parse(mqttObject.msg);
        }
        if (mqttObject.topic.Contains("GY1"))
        {
            Realtime_value_GY1.text = (float.Parse(mqttObject.msg)).ToString();
            r.GY1 = float.Parse(mqttObject.msg);
        }
        if (mqttObject.topic.Contains("GZ1"))
        {
            Realtime_value_GZ1.text = (float.Parse(mqttObject.msg)).ToString();
            r.GZ1 = float.Parse(mqttObject.msg);
        }
        if (mqttObject.topic.Contains("AX2"))
        {
            //Realtime_value_AX2.text = (float.Parse(mqttObject.msg)).ToString();
            r.AX2 = float.Parse(mqttObject.msg);
        }
        if (mqttObject.topic.Contains("AY2"))
        {
            //Realtime_value_AY2.text = (float.Parse(mqttObject.msg)).ToString();
            r.AY2 = float.Parse(mqttObject.msg);
        }
        if (mqttObject.topic.Contains("AZ2"))
        {
            //Realtime_value_AZ2.text = (float.Parse(mqttObject.msg)).ToString();
            r.AZ2 = float.Parse(mqttObject.msg);
        }
        if (mqttObject.topic.Contains("GX2"))
        {
            //Realtime_value_GX2.text = (float.Parse(mqttObject.msg)).ToString();
            r.GX2 = float.Parse(mqttObject.msg);
        }
        if (mqttObject.topic.Contains("GY2"))
        {
            //Realtime_value_GY2.text = (float.Parse(mqttObject.msg)).ToString();
            r.GY2 = float.Parse(mqttObject.msg);
        }
        if (mqttObject.topic.Contains("GZ2"))
        {
            //Realtime_value_GZ2.text = (float.Parse(mqttObject.msg)).ToString();
            r.GZ2 = float.Parse(mqttObject.msg);
            useModel.ExecuteModel(r);
        }
        if (mqttObject.topic.Contains("GX3"))
        {
            //Realtime_value_GX2.text = (float.Parse(mqttObject.msg)).ToString();
            r.GX3 = float.Parse(mqttObject.msg);
        }
        if (mqttObject.topic.Contains("GY3"))
        {
            //Realtime_value_GY2.text = (float.Parse(mqttObject.msg)).ToString();
            r.GY3 = float.Parse(mqttObject.msg);
        }
        if (mqttObject.topic.Contains("GZ3"))
        {
            //Realtime_value_GZ2.text = (float.Parse(mqttObject.msg)).ToString();
            r.GZ3 = float.Parse(mqttObject.msg);


            AHRS.Update(deg2rad(r.GX1), deg2rad(r.GY1), deg2rad(r.GZ1), r.AX1, r.AY1, r.AZ1);
            hand.transform.rotation = new Quaternion(AHRS.Quaternion[0], AHRS.Quaternion[1], AHRS.Quaternion[2], AHRS.Quaternion[3]);
            useModel.ExecuteModel(r);
        }
        if (mqttObject.topic.Contains("temp"))
        {
            Realtime_value_temp.text = (float.Parse(mqttObject.msg)- 6).ToString();
            //useModel.ExecuteModel(r);
        }
        if (mqttObject.topic.Contains("EMG"))
        {
            Realtime_value_EMG.text = (float.Parse(mqttObject.msg)).ToString();
            r.EMG = float.Parse(mqttObject.msg);
            float difference = Mathf.Abs(r.EMG - 10);
            float maxDifference = 10; 
            //float redIntensity = Mathf.Clamp(difference / maxDifference, 0, 1); 

            //sphere.GetComponent<Renderer>().material.color = new Color(redIntensity, 0, 0, 1);
            float t = Mathf.Clamp(difference / maxDifference, 0, 1);

            Color color = Color.Lerp(Color.green, Color.red, t);

            sphere.GetComponent<Renderer>().material.color = color;
            //if (r.EMG == 400)
            //{
            //    sphere.GetComponent<Renderer>().material.color = new Color(255f / 255f, 0 / 255f, 0 / 255f, 255f / 255f);
            //}

        }
    }
    static float deg2rad(float degrees)
    {
        return (float)(Math.PI / 180) * degrees;
    }


}
 [System.Serializable]
    public class Root
    {
        public float AX1;
        public float AY1;
        public float AZ1;
        public float GX1;
        public float GY1;
        public float GZ1;
        public float AX2;
        public float AY2;
        public float AZ2;
        public float GX2;
        public float GY2;
        public float GZ2;
        public float AX3;
        public float AY3;
        public float AZ3;
        public float GX3;
        public float GY3;
        public float GZ3;
        public float EMG;
        public float temp;
    }