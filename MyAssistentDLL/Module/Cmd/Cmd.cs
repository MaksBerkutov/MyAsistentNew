using System;
using System.Collections.Generic;

namespace MyAssistentDLL.Module.Cmd
{
    public static class Cmd
    {
        public enum TypeCmd
        {
            Successful,
            Error
        }
        static Dictionary<string, Func<string, (TypeCmd,string)>> _Funcs = new Dictionary<string, Func<string, (TypeCmd, string)>>()
        {
            //FuncGetItems
                {"GetNumCommand",new Func<string, (TypeCmd,string)>(s=>{return (TypeCmd.Successful,$"Общее количество команд = {Codes.CodeSaver.CountCommand}"); })  },
                {"time",new Func<string, (TypeCmd,string)>(s=>{return (TypeCmd.Successful,$"Время {DateTime.Now.ToLongTimeString()}"); })  },
                {"date",new Func<string, (TypeCmd,string)>(s=>{return (TypeCmd.Successful,$"Время {DateTime.Now.ToLongDateString()}"); })  }
        };
        public static (TypeCmd, string) Push(string str)
        {
            if (str.ToLower() == "help")
            {
                string res = String.Empty;
                foreach (var item in _Funcs)
                    res += $"{item.Key}\n";
                return (TypeCmd.Successful,res);

            }
            if (_Funcs.TryGetValue(str, out var func))
            {
                return func.Invoke(str);
            }
            else return (TypeCmd.Error, "Комманда не найдена!");
        }
    }
}
