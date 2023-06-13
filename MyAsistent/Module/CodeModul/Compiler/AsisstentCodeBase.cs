using System;
using System.Collections.Generic;

namespace MyAsistent.Module.CodeModul.Compiler
{
    public class AsistenetVariableManager
    {
        List<AssistentVariable> items;

        public AsistenetVariableManager()
        {
            items = new List<AssistentVariable>();
        }

        public AssistentVariable this[int i] => items[i];
        public AssistentVariable this[string i] => items.Find(x => x.Name == i);
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
        public void add(AssistentVariable obj) => items.Add(obj);
    }
    public class AsisstentCodeBase
    {
        //Заставляет говорить асистента 
        public static void Voise(string str)=>MyAsistent.Module.Sound.Voice.PlayRuAsync(str);
        //Получаем Информацию от асисстента
        public static void SendToArduino (string plateName,string commnad)=>Module.Internet.
            InternetWorkerModule.SendToClent(plateName, commnad);
        public static AsistenetVariableManager ReadArduino(string plateName, string commnad)
        {
            if (Module.Internet.
            InternetWorkerModule.SendToClinetRecive(plateName, commnad, out var result)) {
                var item = result.Split('_');
                AsistenetVariableManager obj = new AsistenetVariableManager();
                foreach (var i in item)
                {
                    var name = i.Split(':')[0];
                    name = name.Remove(0, 1);
                    name = name.Remove(name.Length - 1, 1);
                    obj.add(new AsistenetVariableManager.AssistentVariable(name, i.Split(':')[1]));
                }
                return obj;
            }
            else return null;
        }


    }
}
