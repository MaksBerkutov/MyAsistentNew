using System.Collections.Generic;
namespace InjectionAsistent
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

}