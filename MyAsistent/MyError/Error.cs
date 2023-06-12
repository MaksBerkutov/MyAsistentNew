using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAsistent.MyError
{
    public static class Error
    {
        public static readonly Dictionary<string, string> _errors = new Dictionary<string, string>()
        {
            {"A0111","При отправке запроса об иницилизации плата не дала ответ в теченее отведёного времени!\n(настройки ожидания можно изменить в Настройки>Галвные>Время ожидания.)" },
            {"A0112","При отправке запроса об получении типа устройства плата не дала ответ в теченее отведёного времени!\n(настройки ожидания можно изменить в Настройки>Галвные>Время ожидания.)" }
        };
    }
}
