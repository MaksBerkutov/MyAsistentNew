using System.Collections.Generic;
using MyAssistentDLL;

namespace MyAsistent.Windows.Code
{
    public class FileCodeInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
    static class CodePage
    {
        private static string path = $@"{MainSettings.PATH}/ScriptDump.json";
        public static void Init()
        {
            if (System.IO.File.Exists(path))
                Files = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileCodeInfo>>(System.IO.File.ReadAllText(path));
            if(!System.IO.Directory.Exists(MainSettings.PATH_SAVE_SCRIPT))
                System.IO.Directory.CreateDirectory(MainSettings.PATH_SAVE_SCRIPT);

        }
        public static void Save()
        {
            System.IO.File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(Files));
        }
        static public List<FileCodeInfo> Files = new List<FileCodeInfo>();
        
    }
}
