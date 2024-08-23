using System.Collections.Generic;
using UnityEngine;
using M2MqttUnity;
using System;
using uPLibrary.Networking.M2Mqtt.Messages;
//the mqttObj is use to store the message received and topic subscribed, so we can select the right object from the controller
//C# Property GET/SET and event listener is used to reduce Update overhead in the controlled objects
public class mqttObj{
private string m_msg;
public string msg
{
get{return m_msg;}
set{
if (m_msg == value) return;
m_msg = value;
}
}
private string m_topic;
public string topic
{
get
{
return m_topic;
}
set
{
if (m_topic == value) return;
m_topic = value;
}
}
}
public class mqttManager : M2MqttUnityClient
{
[Header("MQTT topics")]
[Tooltip("Set the topic to subscribe. !!!ATTENTION!!! multi-level wildcard # subscribes to all topics")]
//public string topicSubscribe = "#"; // topic to subscribe. !!! The multi-level wildcard # is used to subscribe to all the topics. Attention i if #, subscribe to all topics. Attention if MQTT is on data plan
public List<string> topicSubscribe = new List<string>(); //list of topics to subscribe
[Tooltip("Set the topic to publish (optional)")]
public string topicPublish = ""; // topic to publish
public string messagePublish = ""; // message to publish
[Tooltip("Set this to true to perform a testing cycle automatically on startup")]
public bool autoTest = false;
mqttObj mqttObject = new mqttObj();
public event OnMessageArrivedDelegate OnMessageArrived;
public delegate void OnMessageArrivedDelegate(mqttObj mqttObject);
//using C# Property GET/SET and event listener to expose the connection status
private bool m_isConnected;
public bool isConnected
{
get
{
return m_isConnected;
}
set
{
if (m_isConnected == value) return;
m_isConnected = value;
if (OnConnectionSucceeded != null)
{
                OnConnectionSucceeded(isConnected);
}
}
}
public event OnConnectionSucceededDelegate OnConnectionSucceeded;
public delegate void OnConnectionSucceededDelegate(bool isConnected);
// a list to store the mqttObj received
private List<mqttObj> eventMessages = new List<mqttObj>();

public Transform hand1;
Vector3 handOriginPos;

float xx = 0,yy = 0,zz = 0,xr = 0, yr = 0, zr = 0;

public void Publish()
{
        client.Publish(topicPublish, System.Text.Encoding.UTF8.GetBytes(messagePublish), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        Debug.Log("Test message published");
}
public void SetEncrypted(bool isEncrypted)
{
this.isEncrypted = isEncrypted;
}
protected override void OnConnecting()
{
base.OnConnecting();
}
protected override void OnConnected()
{
base.OnConnected();
isConnected = true;
if (autoTest)
{
            Publish();
}
}
protected override void OnConnectionFailed(string errorMessage)
{
        Debug.Log("CONNECTION FAILED! " + errorMessage);
}
protected override void OnDisconnected()
{
        Debug.Log("Disconnected.");
isConnected = false;
}
protected override void OnConnectionLost()
{
        Debug.Log("CONNECTION LOST!");
}
protected override void SubscribeTopics()
{
foreach (string item in topicSubscribe) //subscribe to all the topics of the Public List topicSubscribe, not most efficient way (e.g. JSON object works better), but it might be useful in certain circumstances 
{
         client.Subscribe(new string[] { item }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });   
}
}
protected override void UnsubscribeTopics()
{
foreach (string item in topicSubscribe)
{
            client.Unsubscribe(new string[] { item });
}
}
protected override void Start()
{
        handOriginPos = hand1.localPosition;
        base.Start();
}
float angleX = 0, angleY = 0, angleZ = 0;
protected override void DecodeMessage(string topicReceived, byte[] message)
{
//The message is decoded and stored into the mqttObj (defined at the lines 40-63)
        mqttObject.msg = System.Text.Encoding.UTF8.GetString(message);
        mqttObject.topic=topicReceived;
        Debug.Log("Received: " + mqttObject.msg + "from topic: " + mqttObject.topic);


        if (mqttObject.topic.Contains("GX1"))
        {
            xx = float.Parse(mqttObject.msg);
            //xr = handOriginPos.x + xx*5;
            xr = handOriginPos.x + xx * 5;
            //Debug.Log(xr);
        }
        if (mqttObject.topic.Contains("GY1"))
        {
            yy = float.Parse(mqttObject.msg);
            yr = handOriginPos.y + Math.Abs(yy) *5;
            //Debug.Log(yr);
        }
        if (mqttObject.topic.Contains("GZ1"))
        {
            zz = float.Parse(mqttObject.msg);
            zr = handOriginPos.z + zz *5;
            //Debug.Log(xr + "..." + yr + "..." + zr);

        }

        hand1.localPosition = new Vector3(xr, handOriginPos.y, zr);
        //float lerpSpeed = 0.7f;
        //hand1.localPosition = Vector3.Lerp(hand1.localPosition, new Vector3(xr, yr, zr), lerpSpeed);



        StoreMessage(mqttObject);
if(OnMessageArrived !=null){
        OnMessageArrived(mqttObject);
}
}
private void StoreMessage(mqttObj eventMsg)
{
if (eventMessages.Count > 50)
{
            eventMessages.Clear();
}
        eventMessages.Add(eventMsg);
}
    protected override void Update()
    {
        //base.Update(); // call ProcessMqttEvents()
        base.ProcessMqttEvents();
    }
    private void OnDestroy()
{
        Disconnect();
}
private void OnValidate()
{
if (autoTest)
{
autoConnect = true;
}
}
}