using System.Collections.Generic;

namespace MyAssistentDLL.Module.Internet
{
    internal class MainArduinoCommand
   {
        internal static readonly List<string> command  = new List<string>() { 
          "SERV_GAI",//Get ALL Info NOTE: From Arduino WARNING: 0 index this is name arduino in server
          "SERV_GP"//GET platform
      };
        internal static readonly Dictionary<string, string> InfoAboutCommand = new Dictionary<string, string>()
      {
          {command[0],"Получение краткой информации об устройстве которое пытаеться установить соедение с сервером.\n" +
              "Ситаксис ответа: \"NamePlate.DiscriptionPlate\"" },
          {command[1],"Получение информации о устройстве которое пытаеться установить подключение\n" +
              "Это нужно для того что бы сервер понимал какеи метод применять." }
      };

    
    }
}
