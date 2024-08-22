// #define SDA_PIN D2
// #define SCL_PIN D1
#define USE_NODE_MCU_BOARD



#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <PubSubClient.h>
#include <Wire.h>

#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>

TwoWire I2Cone = TwoWire();
TwoWire I2Ctwo = TwoWire();
TwoWire I2Cthree = TwoWire();

Adafruit_MPU6050 mpu1;
Adafruit_MPU6050 mpu2;
Adafruit_MPU6050 mpu3;


#include <SoftwareSerial.h>
#include <ezTime.h>
#define FIREBASE_PROJECT_ID "smartfitness-dfb43"
#include <Firebase_ESP_Client.h>
#include <addons/TokenHelper.h>


// Date and time
Timezone GB;

#define DATABASE_URL "https://smartfitness-dfb43-default-rtdb.europe-west1.firebasedatabase.app/"

#define API_KEY "AIzaSyA-PZ6RflIqF3gW86OMbDuN-wzMajvI54Q"
// Define Firebase Data objects
FirebaseData fbdo;
FirebaseAuth auth;
FirebaseConfig configF;
// Replace with authorized email and corresponding password
#define USER_EMAIL "8888@ucl.ac.uk"
#define USER_PASSWORD "123456"



const int analogInPin = A0;
float EMG;
float MPU1_AX;
float MPU1_AY;
float MPU1_AZ;
float MPU1_GX;
float MPU1_GY;
float MPU1_GZ;
float MPU1_temp;
float MPU2_AX;
float MPU2_AY;
float MPU2_AZ;
float MPU2_GX;
float MPU2_GY;
float MPU2_GZ;
float MPU2_temp;
float MPU3_AX;
float MPU3_AY;
float MPU3_AZ;
float MPU3_GX;
float MPU3_GY;
float MPU3_GZ;
float MPU3_temp;
// float latitude , longitude;



// Wifi and MQTT
#include "secret.h" 

const char* ssid = SECRET_SSID;
const char* password = SECRET_PASS;
const char* mqttuser = SECRET_MQTTUSER;
const char* mqttpass = SECRET_MQTTPASS;

ESP8266WebServer server(80);
const char* mqtt_server = "mqtt.cetools.org";
WiFiClient espClient;
PubSubClient client(espClient);
long lastMsg = 0;
char msg[50];
int value = 0;




void setup(void) {
  Serial.begin(115200);

  while (!Serial)
    delay(10); // will pause Zero, Leonardo, etc until serial console opens

  Serial.println("Adafruit MPU6050 test!");

  // Try to initialize!
  I2Cone.begin(D2, D1); // SDA, SCL
  if (!mpu1.begin(0x68, &I2Cone)) {
    Serial.println("Failed to find MPU6050 chip");
    while (1) {
      delay(10);
    }
  }
  Serial.println("MPU6050 Found!");

  I2Ctwo.begin(D4, D3); // SDA, SCL
  if (!mpu2.begin(0x68, &I2Ctwo)) {
    Serial.println("Failed to find MPU6050 chip 2");
    while (1) {
      delay(10);
    }
  }
  Serial.println("MPU6050 2 ready!");

  I2Cthree.begin(D7, D6); // SDA, SCL
  if (!mpu3.begin(0x68, &I2Cthree)) {
    Serial.println("Failed to find MPU6050 chip 3");
    while (1) {
      delay(10);
    }
  }
  Serial.println("MPU6050 3 ready!");


  //SerialGPS.begin(115200);

  // run initialisation functions
  startWifi();
  //startWebserver();

  configF.api_key = API_KEY;
  auth.user.email = USER_EMAIL;
  auth.user.password = USER_PASSWORD;
  configF.token_status_callback = tokenStatusCallback;
  configF.max_token_generation_retry = 5;
  Firebase.begin(&configF, &auth);
  Firebase.reconnectWiFi(true);

  syncDate();

  // start MQTT server
  client.setServer(mqtt_server, 1884);
  client.setCallback(callback);


}

void loop() {

      server.handleClient();

      sendMQTT();

      client.loop();
      //screenTime();

      firebasepush();
      // batteryCapacity();
      //ui.refresh();
      // screenTime();

}

void startWifi() {
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);
  WiFi.begin(ssid, password);

  // check to see if connected and wait until you are
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("WiFi connected");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
}


void syncDate() {
  // get real date and time
  waitForSync();
  Serial.println("UTC: " + UTC.dateTime());
  GB.setLocation("Europe/London");
  Serial.println("London time: " + GB.dateTime());
}



// void batteryCapacity(){
//   sensorValue = analogRead(analogInPin);
//   voltage = (((sensorValue * 3.3) / 1024) * 2 + calibration); //multiply by two as voltage divider network is 100K & 100K Resistor
//   bat_percentage = mapfloat(voltage, 2.8, 4.2, 0, 100); //2.8V as Battery Cut off Voltage & 4.2V as Maximum Voltage
//   if (bat_percentage >= 100)
//   {
//     bat_percentage = 100;
//   }
//   if (bat_percentage <= 0)
//   {
//     bat_percentage = 1;
//   }
  
  
//   //Print data on serial monitor

//   Serial.print("Analog Value = ");
//   Serial.println(sensorValue);
//   Serial.print("Output Voltage = ");
//   Serial.println(voltage);
//   Serial.print("Battery Percentage = ");
//   Serial.print(bat_percentage);
//   Serial.println(" %");
//   Serial.println();
//   Serial.println("****************************");
//   Serial.println();
//   delay(1000);
 
//        if (bat_percentage <=30)
//     {
//       Serial.println("Battery level below 30%, Charge battery on time");
//       screen.setTextColor(COLOR_RGB565_RED);
//       screen.fillScreen(COLOR_RGB565_BLACK);
//       screen.setTextSize(2.5);
//       screen.println("Battery level below 30%, Charge battery on time");
//       delay(500);
//     }
// }

// float mapfloat(float x, float in_min, float in_max, float out_min, float out_max)
// {
//   return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
// }

void firebasepush(void){

  static unsigned long printTimepoint = millis();
  if (millis() - printTimepoint > 100U) {
    printTimepoint = millis();

    String documentPath = String(GB.dateTime("Y-m-d")) + "/" + String(GB.dateTime("H:i:s"));
    FirebaseJson content;

    sensors_event_t accel1,gyro1,temp1,accel2,gyro2,temp2,accel3,gyro3,temp3;
    // mpu1.getEvent(&accel1, &gyro1, &temp1);
    mpu1.getEvent(&accel1, &gyro1, &temp1);
    mpu2.getEvent(&accel2, &gyro2, &temp2);
    mpu3.getEvent(&accel3, &gyro3, &temp3);
    MPU1_temp = temp1.temperature;
    Serial.println(MPU1_temp);
    content.set("fields/temp1/doubleValue", String(MPU1_temp).c_str());
    MPU1_AX = accel1.acceleration.x;
    content.set("fields/AX1/doubleValue", String(MPU1_AX).c_str());
    MPU1_AY = accel1.acceleration.y;
    Serial.println(MPU1_AY);
    content.set("fields/AY1/doubleValue", String(MPU1_AY).c_str());
    MPU1_AZ = accel1.acceleration.z;
    content.set("fields/AZ1/doubleValue", String(MPU1_AZ).c_str());
    MPU1_GX = gyro1.gyro.x;
    content.set("fields/GX1/doubleValue", String(MPU1_GX).c_str());
    MPU1_GY = gyro1.gyro.y;
    content.set("fields/GY1/doubleValue", String(MPU1_GY).c_str());
    MPU1_GZ = gyro1.gyro.z;
    content.set("fields/GZ1/doubleValue", String(MPU1_GZ).c_str());
    // MPU2_temp = temp2.temperature;
    // Serial.println(MPU2_temp);
    // content.set("fields/temp2/doubleValue", String(MPU2_temp).c_str());
    MPU2_AX = accel2.acceleration.x;
    content.set("fields/AX2/doubleValue", String(MPU2_AX).c_str());
    MPU2_AY = accel2.acceleration.y;
    Serial.println(MPU2_AY);
    content.set("fields/AY2/doubleValue", String(MPU2_AY).c_str());
    MPU2_AZ = accel2.acceleration.z;
    content.set("fields/AZ2/doubleValue", String(MPU2_AZ).c_str());
    MPU2_GX = gyro2.gyro.x;
    content.set("fields/GX2/doubleValue", String(MPU2_GX).c_str());
    MPU2_GY = gyro2.gyro.y;
    content.set("fields/GY2/doubleValue", String(MPU2_GY).c_str());
    MPU2_GZ = gyro2.gyro.z;
    content.set("fields/GZ2/doubleValue", String(MPU2_GZ).c_str());
    // MPU3_temp = temp3.temperature;
    // Serial.println(MPU3_temp);
    // content.set("fields/temp3/doubleValue", String(MPU3_temp).c_str());
    MPU3_AX = accel3.acceleration.x;
    content.set("fields/AX3/doubleValue", String(MPU3_AX).c_str());
    MPU3_AY = accel3.acceleration.y;
    Serial.println(MPU3_AY);
    content.set("fields/AY3/doubleValue", String(MPU3_AY).c_str());
    MPU3_AZ = accel3.acceleration.z;
    content.set("fields/AZ3/doubleValue", String(MPU3_AZ).c_str());
    MPU3_GX = gyro3.gyro.x;
    content.set("fields/GX3/doubleValue", String(MPU3_GX).c_str());
    MPU3_GY = gyro3.gyro.y;
    content.set("fields/GY3/doubleValue", String(MPU3_GY).c_str());
    MPU3_GZ = gyro3.gyro.z;
    content.set("fields/GZ3/doubleValue", String(MPU3_GZ).c_str());
    EMG = analogRead(analogInPin);
    Serial.println(EMG);
    content.set("fields/EMG/doubleValue", String(EMG).c_str());

    if (Firebase.Firestore.patchDocument(&fbdo, FIREBASE_PROJECT_ID, "", documentPath.c_str(), content.raw(), "AX1,AY1,AZ1,GX1,GY1,GZ1,AX2,AY2,AZ2,GX2,GY2,GZ2,AX3,AY3,AZ3,GX3,GY3,GZ3,EMG")) {
      // Serial.print("ok\n%s\n\n", fbdo.payload().c_str());
      return;
    } else {
      Serial.println(fbdo.errorReason());
    }

    if (Firebase.Firestore.createDocument(&fbdo, FIREBASE_PROJECT_ID, "", documentPath.c_str(), content.raw())) {
      // Serial.print("ok\n%s\n\n", fbdo.payload().c_str());
      return;
    } else {
      Serial.println(fbdo.errorReason());
    }




    // delay(500);  // Wait for 1 second before publishing again
  }
}

void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");

}

void reconnect() {
  // Loop until we're reconnected
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    // Create a random client ID
    String clientId = "ESP8266Client-";
    clientId += String(random(0xffff), HEX);
    
    // Attempt to connect with clientID, username and password
    if (client.connect(clientId.c_str(), mqttuser, mqttpass)) {
      Serial.println("connected");
      // ... and resubscribe
      client.subscribe("student/ucfnega/inTopics");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}

void sendMQTT() {

  if (!client.connected()) {
    reconnect();
  }
  client.loop();

  static unsigned long printTimepoint = millis();
  if (millis() - printTimepoint > 100U) {
    printTimepoint = millis();

  //   String documentPath = String(GB.dateTime("Y-m-d")) + "/" + String(GB.dateTime("H:i"));

    // sensors_event_t accel,gyro,temp;
    // mpu.getEvent(&accel, &gyro, &temp);
    // MPU1_temp = temp.temperature;
    snprintf (msg, 50, "%.1f", MPU1_temp);
    Serial.print("temp1: ");
    Serial.println(msg);
    client.publish("student/ucfnega/temp1", msg);

    // MPU1_AX = accel.acceleration.x;
    snprintf (msg, 50, "%.1f", MPU1_AX);
    Serial.print("AX1: ");
    Serial.println(msg);
    client.publish("student/ucfnega/AX1", msg);

    // MPU1_AY = accel.acceleration.y;
    snprintf (msg, 50, "%.1f", MPU1_AY);
    Serial.print("AY1: ");
    Serial.println(msg);
    client.publish("student/ucfnega/AY1", msg);

    // MPU1_AZ = accel.acceleration.z;
    snprintf (msg, 50, "%.1f", MPU1_AZ);
    Serial.print("AZ1: ");
    Serial.println(msg);
    client.publish("student/ucfnega/AZ1", msg);

    // MPU1_GX = gyro.gyro.x;
    snprintf (msg, 50, "%.1f", MPU1_GX);
    Serial.print("GX1: ");
    Serial.println(msg);
    client.publish("student/ucfnega/GX1", msg);

    // MPU1_GY = gyro.gyro.y;
    snprintf (msg, 50, "%.1f", MPU1_GY);
    Serial.print("GY1: ");
    Serial.println(msg);
    client.publish("student/ucfnega/GY1", msg);

    // MPU1_GZ = gyro.gyro.z;
    snprintf (msg, 50, "%.1f", MPU1_GZ);
    Serial.print("GZ1: ");
    Serial.println(msg);
    client.publish("student/ucfnega/GZ1", msg);

    // snprintf (msg, 50, "%.1f", MPU2_temp);
    // Serial.print("temp2: ");
    // Serial.println(msg);
    // client.publish("student/ucfnega/temp2", msg);

    // MPU1_AX = accel.acceleration.x;
    snprintf (msg, 50, "%.1f", MPU2_AX);
    Serial.print("AX2: ");
    Serial.println(msg);
    client.publish("student/ucfnega/AX2", msg);

    // MPU1_AY = accel.acceleration.y;
    snprintf (msg, 50, "%.1f", MPU2_AY);
    Serial.print("AY2: ");
    Serial.println(msg);
    client.publish("student/ucfnega/AY2", msg);

    // MPU1_AZ = accel.acceleration.z;
    snprintf (msg, 50, "%.1f", MPU2_AZ);
    Serial.print("AZ2: ");
    Serial.println(msg);
    client.publish("student/ucfnega/AZ2", msg);

    // MPU1_GX = gyro.gyro.x;
    snprintf (msg, 50, "%.1f", MPU2_GX);
    Serial.print("GX2: ");
    Serial.println(msg);
    client.publish("student/ucfnega/GX2", msg);

    // MPU1_GY = gyro.gyro.y;
    snprintf (msg, 50, "%.1f", MPU2_GY);
    Serial.print("GY2: ");
    Serial.println(msg);
    client.publish("student/ucfnega/GY2", msg);

    // MPU1_GZ = gyro.gyro.z;
    snprintf (msg, 50, "%.1f", MPU2_GZ);
    Serial.print("GZ2: ");
    Serial.println(msg);
    client.publish("student/ucfnega/GZ2", msg);

    // snprintf (msg, 50, "%.1f", MPU3_temp);
    // Serial.print("temp3: ");
    // Serial.println(msg);
    // client.publish("student/ucfnega/temp3", msg);

    // MPU1_AX = accel.acceleration.x;
    snprintf (msg, 50, "%.1f", MPU3_AX);
    Serial.print("AX3: ");
    Serial.println(msg);
    client.publish("student/ucfnega/AX3", msg);

    // MPU1_AY = accel.acceleration.y;
    snprintf (msg, 50, "%.1f", MPU3_AY);
    Serial.print("AY3: ");
    Serial.println(msg);
    client.publish("student/ucfnega/AY3", msg);

    // MPU1_AZ = accel.acceleration.z;
    snprintf (msg, 50, "%.1f", MPU3_AZ);
    Serial.print("AZ3: ");
    Serial.println(msg);
    client.publish("student/ucfnega/AZ3", msg);

    // MPU1_GX = gyro.gyro.x;
    snprintf (msg, 50, "%.1f", MPU3_GX);
    Serial.print("GX3: ");
    Serial.println(msg);
    client.publish("student/ucfnega/GX3", msg);

    // MPU1_GY = gyro.gyro.y;
    snprintf (msg, 50, "%.1f", MPU3_GY);
    Serial.print("GY3: ");
    Serial.println(msg);
    client.publish("student/ucfnega/GY3", msg);

    // MPU1_GZ = gyro.gyro.z;
    snprintf (msg, 50, "%.1f", MPU3_GZ);
    Serial.print("GZ3: ");
    Serial.println(msg);
    client.publish("student/ucfnega/GZ3", msg);

    snprintf (msg, 50, "%.1f", EMG);
    Serial.print("EMG: ");
    Serial.println(msg);
    client.publish("student/ucfnega/EMG", msg);
  } else {
      Serial.println("Error");
    }
  // delay(500); 
}
