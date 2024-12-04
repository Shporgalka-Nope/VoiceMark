using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Voice_Control.VM
{
    internal class VoiceEngineVM
    {
        private SpeechRecognitionEngine engine;
        private CommandList commandList;
        private List<string> commands;

        public string tb_CheechToText { get; set; }
        public string tb_cultureName { get; set; }
        public string tb_configName { get; set; }
        public VoiceEngineVM(MainWindow mainWin)
        {
            engine = new SpeechRecognitionEngine();
            SetUpWindow setupWin = new SetUpWindow(engine);
            setupWin.ShowDialog();

            if (setupWin.DialogResult != true) { Environment.Exit(0); }

            commandList = SetUpCookies.selectedConfig;
            CultureInfo cultureInfo = SetUpCookies.selectedCulture;

            engine.SpeechRecognized += Engine_SpeechRecognized;
            engine.SpeechDetected += Engine_SpeechDetected;

            //Loading grammar from config
            Choices choices = new Choices();
            commands = new List<string>();
            foreach (Command com in commandList.Commands) { commands.Add(com.Phrase); }
            choices.Add(commands.ToArray());

            GrammarBuilder gb = new GrammarBuilder(choices);
            gb.Culture = cultureInfo;
            Grammar grammar = new Grammar(gb);
            engine.LoadGrammar(grammar); //new DictationGrammar for any words (bad)
            engine.SetInputToDefaultAudioDevice();

            //Start
            engine.RecognizeAsync(RecognizeMode.Multiple);

            //INFORMATION setup
            tb_cultureName = "Culture: " + commandList.Culture;
            tb_configName = "Name: " + commandList.Name;
        }

        private void Engine_SpeechDetected(object? sender, SpeechDetectedEventArgs e)
        {
            tb_CheechToText = "Result: [Listening...]";
        }
        private void Engine_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            tb_CheechToText = "Result: " + e.Result.Text;
            int act = -1;
            string arg = null;
            foreach (Command command in commandList.Commands)
            {
                if (e.Result.Text == command.Phrase)
                {
                    act = command.Action;
                    arg = command.Argument;
                    break;
                }
            }
            DoTheAct(act, arg);
        }

        private void DoTheAct(int actType, string actArg)
        {
            switch (actType)
            {
                case -1:
                    break;
                case 1: //Case 1 - Open an application (process.exe)
                    try
                    {
                        Process.Start(actArg);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An unexpected error occured!\n {ex.Message}");
                    }
                    break;
                case 2: //Case 2 - Open explorer with spetisfied path
                    try
                    {
                        Process.Start("explorer.exe", @$"{actArg}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An unexpected error occured!\n {ex.Message}");
                    }
                    break;
            }
        }

        private RelayCommand settingsClick;
        public RelayCommand SettingsClick
        {
            get
            {
                return settingsClick ?? (settingsClick = new RelayCommand(obj =>
                {
                    SetUpWindow setupWin = new SetUpWindow(engine);
                    setupWin.ShowDialog();
                    (obj as MainWindow).Close();
                }));
            }
        }
    }
}
