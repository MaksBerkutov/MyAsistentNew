using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAsistent.Module.Sound
{
    public static class Voice
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
        public static List<string> GetName()
        {
            List<string> res = new List<string>();
            foreach (var voice in synthesizer.GetInstalledVoices())
                res.Add(voice.VoiceInfo.Name);
            return res;
        }
        public static void PlayRu(string str_text)
        {
           // synthesizer.GetInstalledVoices();
           
            synthesizer.SpeakAsync(str_text);
        }
        public static void PlayRuAsync(string str_text)=> synthesizer.SpeakAsync(str_text);
    }
    public class Sound
    {
        private static System.Globalization.CultureInfo ci;
        private static SpeechRecognitionEngine sre;
        private static Choices numbers;
        private static GrammarBuilder gb;
        private static Grammar g;
        private static List<string> SaveS = new List<string>() { "1" };
        public static double Sensivity { get => sensivity;set
            {
                if (value >= 0 && value <= 1.0)
                {
                    MainSettings.Sensivity = sensivity = value;
                }
                   
            }
        }
        public static double sensivity = 0.5;
        private static bool Check_add_range_Lib(string lib) => (!(SaveS.FindIndex(p => p == lib) >= 0));
        public static void Change_Culture(string culture) { ci = new System.Globalization.CultureInfo(culture);}
        public static IReadOnlyCollection<RecognizerInfo> getAllCultures() => SpeechRecognitionEngine.InstalledRecognizers();
        public static void Init()
        {
            ci = new System.Globalization.CultureInfo("en-US");
            sre = new SpeechRecognitionEngine(ci);

            numbers = new Choices();
            gb = new GrammarBuilder();

            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {
                if (e.Result.Confidence > 0.7) Codes.interpretatot.Run(e.Result.Text);
            };
            numbers.Add(SaveS.ToArray());
            gb.Culture = ci;
            gb.Append(numbers);
            g = new Grammar(gb);
            sre.LoadGrammar(g);
            sre.RecognizeAsync(RecognizeMode.Multiple);

        }
        public static void ClearItems()
        {
            SaveS.Clear();
            SaveS.Add("1");
            sre.RecognizeAsyncStop();

            sre = new SpeechRecognitionEngine(ci);

            numbers = new Choices();
            gb = new GrammarBuilder();

            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {
               if(e.Result.Confidence>= sensivity) Codes.interpretatot.Run(e.Result.Text);
            };
            numbers.Add(SaveS.ToArray());
            gb.Culture = ci;
            gb.Append(numbers);
            g = new Grammar(gb);
            sre.LoadGrammar(g);
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }
        public static void add_string(string text)
        {
            if (Check_add_range_Lib(text))
            {
                SaveS.Add(text);
                gb = gb = new GrammarBuilder(); gb.Culture = ci; numbers = new Choices();
                numbers.Add(SaveS.ToArray());
                gb.Append(numbers);
                g = new Grammar(gb);
                sre.LoadGrammar(g);
            }

        }

        // Example Hadler
        //static void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        //{
        //    if (e.Result.Confidence > 0.7) l.Text = e.Result.Text;
        //}
    }
}