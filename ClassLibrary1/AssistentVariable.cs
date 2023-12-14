namespace InjectionAsistent
{
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

}