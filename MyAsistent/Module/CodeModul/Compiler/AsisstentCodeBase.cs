using System;

namespace MyAsistent.Module.CodeModul.Compiler
{
    public class AsisstentCodeBase
    {
        //Заставляет говорить асистента 
        public static void Voise(string str)=>MyAsistent.Module.Sound.Voice.PlayRuAsync(str);
        //Получаем Информацию от асисстента
        public static void SendToArduino (string plateName,string commnad)=>Module.Internet.
            InternetWorkerModule.SendToClent(plateName, commnad);
        public static string ReadArduino(string plateName, string commnad)
        {
            if(Module.Internet.
            InternetWorkerModule.SendToClinetRecive(plateName, commnad,out var result))return result;   
            else return null;
        }


    }
}
