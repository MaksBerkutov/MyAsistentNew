#include "Assistent.h"

void AssistenWiFi::ConnectToServere()
{

  Serial.print("Connecting to server");
  while (!client.connect(host, port))
  {
    Serial.print(" .");

    yield();
    delay(1000);
  }
  Serial.print("\nConected to IP: ");
  Serial.print(host);
  Serial.print(":");
  Serial.println(port);
}
void AssistenWiFi::StandartHandler(String str)
{
      Serial.println("Go standartHandler");

  for (int i = 0; i < SizeCMD; i++)
    if (CMD[i] == str)
      HandlerCMDS[i]();
  for (int i = 0; i < SizeCMDRec; i++)
    if ((CMDRec[i]+"_REC") == str)
      SendMessage(HandlerCMDSRec[i]().GetStringVal());
  Serial.println("END standartHandler");

}
String AssistenWiFi::Read()
{
  Serial.println("DEBUG 1");
  size_t len = client.available();
  Serial.print("LEN: \r"); Serial.println(len);
  Serial.println("DEBUG 2");
  uint8_t sbuf[len];
   Serial.println("DEBUG 3");
  client.read(sbuf, len);
   Serial.println("DEBUG 4");
    Serial.print ("DEBUG 4:MSG: "); Serial.println( (char *)sbuf);
    String str = String((char *)sbuf);
    if(str.length()!=len){
      str = str.substring(0,len);
    }
    Serial.print ("FIX MSG: "); Serial.println( (char *)sbuf);
  return str;
}
#ifndef OTA
void AssistenWiFi::ReadOTA()
{
  size_t len = client.available();
  if(len > ESP.getFreeSketchSpace()){
      client.flush();
      SendMessage("No free space or OTA");
      return;
  }
   
  size_t bytesWritten = 0;
  while (client.available() && bytesWritten < len) {
    uint8_t buffer[128];
    size_t bytesRead = client.readBytes(buffer, sizeof(buffer));
    bytesWritten += ESP.flashWrite(bytesWritten, buffer, bytesRead);
  }  
   if (bytesWritten == len) {
          SendMessage("Sketch update complete");
          // Выполните перезагрузку модуля NodeMCU
          ESP.restart();
        } else {
          SendMessage("Error writing sketch to flash");
        }
}
#endif
bool AssistenWiFi::ThisStandartCommand(String str)
{
  Serial.println("DEBUG ThisStandartCommand");
  if (strcmp(str.c_str(),  "SERV_GAI")==0){
        Serial.print("DEBUG : ");Serial.print(str);Serial.print(" = ");Serial.println("SERV_GAI");

    String msg(this->PlatName);
    for(int i = 0;i<SizeCMD;i++){
      Serial.println(CMD[i]);
     msg+="."+CMD[i];
    }
     
    for(int i = 0;i<SizeCMDRec;i++){
            Serial.println(CMDRec[i]);

      msg+="."+CMDRec[i]+"_REC";
    }
      
      SendMessage(msg);
    return true;
  }
    
  else  if (strcmp(str.c_str(),  "SERV_GP")==0){

    Serial.print("DEBUG : ");Serial.print(str);Serial.print(" = ");Serial.println("SERV_GP");
    SendMessage("Arduino");
    return true;
    }
    
  
  else {
    Serial.println("DEBUG No finded standart command ");
    return false;
    }
}
void AssistenWiFi::SendMessage(String msg)
{
 Serial.println("DEBUG SEND");
  if (client)
  {
    while (client.connected())
    {
      Serial.print("\nSend message: ");
      Serial.println(msg);
      client.print(msg);
      return;
    }
  }
}
AssistenWiFi::AssistenWiFi(){}
void AssistenWiFi::Begin(String name, String *CMD, HandlerCMD *HandlerCMDS,
 int SizeCMD, String* CMDRec, HandlerCMDRec* HandlerCMDSRec, int SizeCMDRec, 
 char *ssid, char *password, char *host, int port, int BhaudRate, OnNewMessageFromServer handler)
{
  this->CMD = CMD;
  this->HandlerCMDS = HandlerCMDS;
  this->SizeCMD = SizeCMD;
  this->CMDRec = CMDRec;
  this->HandlerCMDSRec = HandlerCMDSRec;
  this->SizeCMDRec = SizeCMDRec;
  // Setup Wifi
  Serial.begin(BhaudRate);
  PlatName = name;
  this->host = host;
  this->port = port;
  WiFi.begin(ssid, password);
  Serial.print("Connecting");
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(1000);
    Serial.print(" .");
  }
  Serial.print("\nConnected to WiFi.\n SSID:");
  Serial.println(ssid);
  Serial.print("IP:");
  Serial.println(WiFi.localIP());
  // SetupHandler
  if (handler == NULL)
  {
    this->handler = handler;
    UsingStandartHandler = false;
  }
  ConnectToServere();
}
void AssistenWiFi::Reader(bool OTA)
{
  Serial.println("Reader is start");
  if (client)
  {
    while (client.connected())
    {
      while (client.available() > 0)
      {
        #ifndef OTA
        if(OTA){
          ReadOTA();
          return;
        }
         #endif
        String str = Read();
        Serial.print("I READ: ");
        Serial.println(str);
         #ifndef OTA
     if(strcmp(str.c_str(),  "OTA")==0){
        Reader(true);
  }
    #endif
        if (!ThisStandartCommand(str))
          {
            StandartHandler(str);
          }
        delay(10);
        return;
      }
    }
  }
}
void AssistenWiFi::CloseConnection()
{
  Serial.println();
  Serial.println("Closing connection");
  client.flush();
  client.stop();
  Serial.println("Connection Closed");
  yield();
}