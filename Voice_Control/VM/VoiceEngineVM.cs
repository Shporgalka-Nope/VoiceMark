using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps.Serialization;

namespace Voice_Control.VM
{
    internal class VoiceEngineVM : INotifyPropertyChanged
    {
        private SpeechRecognitionEngine engine;
        private CommandList commandList;
        private List<string> commands;
        private bool _isAllowedToListen = true;
        private bool isAllowedToListen
        {
            get { return _isAllowedToListen; }
            set
            {
                _isAllowedToListen = value;
                switch (_isAllowedToListen)
                {
                    case true:
                        tb_CheechToText = "[Ready]";
                        break;
                    case false:
                        tb_CheechToText = "[Disabled]";
                        break;
                }
            }
        }

        private string _tb_CheechToText;
        public string tb_CheechToText 
        { 
            get { return _tb_CheechToText; } 
            set
            {
                _tb_CheechToText = value;
                OnPropertyChanged("tb_CheechToText");
            }
        }

        private string _tb_cultureName;
        public string tb_cultureName 
        { 
            get { return _tb_cultureName; } 
            set
            {
                _tb_cultureName = value;
                OnPropertyChanged("tb_cultureName");
            }
        }

        private string _tb_configName;
        public string tb_configName 
        { 
            get { return _tb_configName; } 
            set
            {
                _tb_configName = value;
                OnPropertyChanged("tb_configName");
            } 
        }

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
            tb_CheechToText = "[Ready]";
        }

        private async void Engine_SpeechDetected(object? sender, SpeechDetectedEventArgs e)
        {
            if (!isAllowedToListen) { return; }
            tb_CheechToText = "Result: [Listening...]";
            await Task.Delay(1000);
            tb_CheechToText = "[Ready]";
        }
        private void Engine_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            if (!isAllowedToListen) { return; }
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

        private async void DoTheAct(int actType, string actArg)
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
            await Task.Delay(1000);
            tb_CheechToText = "[Ready]";
        }

        private RelayCommand settingsClick;
        public RelayCommand SettingsClick
        {
            get
            {
                return settingsClick ?? (settingsClick = new RelayCommand(obj =>
                {
                    isAllowedToListen = false;
                    (obj as MainWindow).Hide();
                    MainWindow mainWin = new MainWindow();
                    mainWin.Show();
                }));
            }
        }

        private RelayCommand muteClick;
        public RelayCommand MuteClick
        {
            get
            {
                return muteClick ?? (muteClick = new RelayCommand(obj =>
                {
                    isAllowedToListen = !isAllowedToListen;
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
