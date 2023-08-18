#ifndef Assistent_h
#define Assistent_h
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include<string.h>
class AssistentVariable{

    String obj;

    public:
        AssistentVariable(String* NameVar,String* Date,int size){
            obj="";

            for(int i = 0;i<size;i++){
                if (i == 0)
                    obj += "[" + NameVar[i] + "]:" + Date[i];
                else
                    obj += "_[" + NameVar[i] + "]:" + Date[i];
            }
            Serial.println(obj);
        }

        String GetStringVal(){
           return obj;
        }
 
    

};
typedef void (*OnNewMessageFromServer)(String message);
typedef void (*HandlerCMD)();
typedef AssistentVariable (*HandlerCMDRec)();


class AssistenWiFi
{

    char *host = "";
    int port;
    String PlatName;

    String *CMD;
    HandlerCMD *HandlerCMDS;
    int SizeCMD;
    String *CMDRec;
    HandlerCMDRec *HandlerCMDSRec;
    int SizeCMDRec;
    OnNewMessageFromServer handler;
    WiFiClient client;
    bool UsingStandartHandler = true;

private:
    void ConnectToServere();
    void StandartHandler(String str);
    String Read();
    #ifndef OTA

    void ReadOTA();
    #endif
    bool ThisStandartCommand(String str);
    void SendMessage(String msg);
public:

    void Begin(String name, String *CMD, HandlerCMD *HandlerCMDS, int SizeCMD, String* CMDRec, HandlerCMDRec* HandlerCMDSRec, int SizeCMDRec, char *ssid, char *password, char *host, int port, int BhaudRate = 9600, OnNewMessageFromServer handler = NULL);
    AssistenWiFi();
    void Reader(bool OTA = false);
    void CloseConnection();
};
#endif