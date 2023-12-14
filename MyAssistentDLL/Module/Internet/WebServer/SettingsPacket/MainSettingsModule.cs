using System;
using System.Collections.Generic;
using System.Linq;
//netsh http add iplisten 192.168.1.200
namespace MyAssistentDLL.Module.Internet.WebServer
{
    public static partial class WebServerManager
    {
        private class MainSettingsModule: IHttpsRespones
        {
            public bool VoiceMessage;
            public bool VoiceLog;
            public string CultureSpeech;
            public string CultureRecognition;
            public double timeAutoSave;
            public double timeWaitIniit;
            public List<string> AllSpeech = new List<string>();
            public List<string> AllRecognition = new List<string>();

            private bool Flag = false;
            public MainSettingsModule()
            {
                timeAutoSave = MainSettings.SaveCommandTime.TotalSeconds;
                timeWaitIniit = MainSettings.WaitForConectionDevice.TotalSeconds;
                CultureSpeech = MainSettings.SpeechCulture;
                CultureRecognition = MainSettings.VoiceCulture;
                VoiceMessage = MainSettings.VoiceMessage;
                VoiceLog = MainSettings.VoiceLog;
                Module.Sound.Voice.GetName().ToList().ForEach(x=>AllSpeech.Add(x));
                Module.Sound.Sound.getAllCultures().ToList().ForEach(x =>
                {
                    if(x.Culture.Name.Length != 0)
                        AllRecognition.Add(x.Culture.Name);
                }); 

            }

            public string Get()
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }

            public void StartHandle()
            {
                if(Flag)
                    OnUpdateSettings?.Invoke("MainSettingsModule");
            }

            public void Update(string Date)
            {
                Flag = false;
                var item = Newtonsoft.Json.JsonConvert.DeserializeObject<MainSettingsModule>(Date);
                if (timeAutoSave != item.timeAutoSave)
                {
                    timeAutoSave = item.timeAutoSave;
                    MainSettings.SaveCommandTime = new TimeSpan(0,0, (int)timeAutoSave);
                    Flag =true;
                }
                if (timeWaitIniit != item.timeWaitIniit)
                {
                    timeWaitIniit = item.timeWaitIniit;
                    MainSettings.WaitForConectionDevice = new TimeSpan(0, 0, (int)timeWaitIniit);
                    Flag = true;
                }
                if (CultureSpeech != item.CultureSpeech)
                {
                    
                    MainSettings.SpeechCulture = CultureSpeech = item.CultureSpeech; 
                    Flag = true;
                }
                if (CultureRecognition != item.CultureRecognition)
                {

                    MainSettings.VoiceCulture = CultureRecognition = item.CultureRecognition; 
                    Flag = true;
                }
                if (VoiceMessage != item.VoiceMessage)
                {

                    MainSettings.VoiceMessage = VoiceMessage = item.VoiceMessage; 
                    Flag = true;
                }
                if (VoiceLog != item.VoiceLog)
                {

                    MainSettings.VoiceLog = VoiceLog = item.VoiceLog; 
                    Flag = true;
                }


             

            }
        }

    
    }
}
