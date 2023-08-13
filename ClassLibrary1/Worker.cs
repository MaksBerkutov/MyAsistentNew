using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


    namespace Worker
    {
        public class Worker
        {
           public class AsistenetVariableManager
            {
                List<AssistentVariable> items;

                public AsistenetVariableManager()
                {
                    items = new List<AssistentVariable>();
                }

                public AssistentVariable this [int i]=>items[i];
                public AssistentVariable this [string i]=>items.Find(x=>x.Name==i);
                public class AssistentVariable
                {
                    string name;
                    string value;

                    public AssistentVariable(string name, string value)
                    {
                        this.name = name;
                        this.value = value;
                    }

                    public string Name => name;
                    public string Value => value;

                }
                public void add(AssistentVariable obj)=>items.Add(obj);
            }
            
            public class Pakage
            {
                public string Key { get; set; }
                public string Reqest { get; set; }
                public string Resume { get; set; }
            }
        Conectector.Conection conn;
            public Worker(Conectector.Conection conn)=>this.conn = conn;
            //User Function
            public void Voice(string VoiceToSpech) => SendPacakge(new Pakage { Reqest = $"Voice={VoiceToSpech}" });
            public void SendArduino(string NamePlate,string Command) => SendPacakge(new Pakage { Reqest = $"Arduino={NamePlate},{Command}" });
            public AsistenetVariableManager SendArduinoRecive(string NamePlate, string Command)
            {
                var item = SendgetResult(new Pakage { Reqest = $"ArduinoDate={NamePlate},{Command}" }).Split('_');
                AsistenetVariableManager obj = new AsistenetVariableManager();
                foreach (var i in item)
                {
                    var name = i.Split(':')[0];
                    name = name.Remove(0,1);
                    name = name.Remove(name.Length-1,1);
                    obj.add(new AsistenetVariableManager.AssistentVariable(name, i.Split(':')[1]));
                }return obj;
            }

            private string SendgetResult(Pakage pak) {
                SendPacakge(pak);
                while (true)
                    while (conn.Available > 0)
                    {
                        byte[] read = new byte[conn.Available];
                        conn.Receive(read);
                        try
                        {
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<Pakage>(
                                Encoding.UTF8.GetString(read)).Resume;
                        }
                        catch (Exception)
                        {
                            return "null";

                        }
                    }

            }
            private async Task<string> SendgetResultAsync(Pakage pak)
            {
                return await Task.Run(() => SendgetResult(pak));
            }
            private void SendPacakge(Pakage pak)
            {
                conn.SignaturePakage(ref pak);
                conn.SendPackaje(Newtonsoft.Json.JsonConvert.SerializeObject(pak));
            }
            private async void SendPacakgeAsync(Pakage pak)
            {
                await Task.Run(()=> SendPacakge(pak));
            } 

        }
    }



