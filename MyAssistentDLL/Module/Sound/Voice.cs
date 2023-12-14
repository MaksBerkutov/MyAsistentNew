using Microsoft.Speech.Synthesis;
using System.Collections.Generic;

namespace MyAssistentDLL.Module.Sound
{
    internal static class Voice
    {
        private static SpeechSynthesizer synthesizer;
        public static void Init()
        {
            synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();
            var i = synthesizer.GetInstalledVoices();
            foreach(var voice in i)
            {
                synthesizer.SelectVoice(voice.VoiceInfo.Name);
            }
        }
        public static void SelectVoice(string str)
        {
            synthesizer.SelectVoice(str);
        }
        public static string[] GetName()
        {
            List<string> res = new List<string>();
            foreach (var voice in synthesizer.GetInstalledVoices())
                res.Add(voice.VoiceInfo.Name);
            return res.ToArray();
        }
        public static void PlayRu(string str_text)
        {           
            synthesizer.SpeakAsync(str_text);
        }
        public static void PlayRuAsync(string str_text)=> synthesizer.SpeakAsync(str_text);
    }
}