using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech;
using System.Speech.Recognition;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Configuration;
using System.Diagnostics;

namespace Voice_Control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpeechRecognitionEngine engine;
        CommandList commandList;
        private List<string> commands;

        public MainWindow()
        {
            InitializeComponent();

            engine = new SpeechRecognitionEngine();
            SetUpWindow setupWin = new SetUpWindow(engine);
            setupWin.ShowDialog();
            if (setupWin.DialogResult != true) { Application.Current.Shutdown(); }

            commandList = setupWin.selectedConfig;
            CultureInfo cultureInfo = setupWin.selectedCulture;

            engine.SpeechRecognized += Engine_SpeechRecognized;
            engine.SpeechDetected += Engine_SpeechDetected;

            //Loading grammar from config
            Choices choices = new Choices();
            commands = new List<string>();
            foreach(Command com in commandList.Commands) { commands.Add(com.Phrase); }
            choices.Add(commands.ToArray());

            GrammarBuilder gb = new GrammarBuilder(choices);
            gb.Culture = cultureInfo;
            Grammar grammar = new Grammar(gb);
            engine.LoadGrammar(grammar); //new DictationGrammar for any words (bad)
            engine.SetInputToDefaultAudioDevice();
            
            //Start
            engine.RecognizeAsync(RecognizeMode.Multiple);

            //INFORMATION setup
            tb_cultureName.Text = "Culture: " + commandList.Culture;
        }

        private void Engine_SpeechDetected(object? sender, SpeechDetectedEventArgs e)
        {
            tb_CheechToText.Text = "Result: [Listening...]";
        }

        private void Engine_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            tb_CheechToText.Text = "Result: " + e.Result.Text;
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

        private void bt_settings_Click(object sender, RoutedEventArgs e)
        {
            SetUpWindow setupWin = new SetUpWindow(engine);
            setupWin.ShowDialog();
            this.Close();
        }
    }
}