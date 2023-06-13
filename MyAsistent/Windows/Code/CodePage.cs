using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAsistent.Windows.Code
{
    public class FileCodeInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
    static class CodePage
    {
        private static string path = $@"{MainSettings.path}/ScriptDump.json";
        public static void Init()
        {
            if (System.IO.File.Exists(path))
                Files = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileCodeInfo>>(System.IO.File.ReadAllText(path));
            if(!System.IO.Directory.Exists(MainSettings.PathToSaveScript))
                System.IO.Directory.CreateDirectory(MainSettings.PathToSaveScript);

        }
        public static void Save()
        {
            System.IO.File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(Files));
        }
        static public List<FileCodeInfo> Files = new List<FileCodeInfo>();
        
    }
}
