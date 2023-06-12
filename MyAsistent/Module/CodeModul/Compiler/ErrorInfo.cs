namespace MyAsistent.Module.CodeModul.Compiler
{
    public class ErrorInfo
    {
        public ErrorInfo(int str, int pos, string name, string message)
        {
            this.str = str;
            this.pos = pos;
            this.name = name;
            this.message = message;
        }

        public int str { get; set; }
        public int pos { get; set; }
        public string name { get; set; }
        public string message { get; set; }
        public override string ToString() => $"{name}| Ошибка в строке {str}, позиция {pos}: {message}";
    }
}
