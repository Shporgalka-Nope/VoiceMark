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

namespace Voice_Control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpeechRecognitionEngine engine;

        public MainWindow()
        {
            InitializeComponent();

            engine = new SpeechRecognitionEngine();
            SetUpWindow setupWin = new SetUpWindow(engine);

            engine.SpeechRecognized += Engine_SpeechRecognized;
            engine.SpeechDetected += Engine_SpeechDetected;

            //Add grammar here
            Choices commands = new Choices(
                "commands",
                "Choices",
                "show"
            );
            GrammarBuilder gb = new GrammarBuilder(commands);
            setupWin.ShowDialog();
            gb.Culture = setupWin.selectedCulture;
            Grammar grammar = new Grammar(gb);
            engine.LoadGrammar(grammar); //new DictationGrammar for any words (bad)
            engine.SetInputToDefaultAudioDevice();
            
            //Start
            engine.RecognizeAsync(RecognizeMode.Multiple);

            //INFORMATION setup
            tb_cultureName.Text = "Culture: " + setupWin.selectedCulture.Name;
        }

        private void Engine_SpeechDetected(object? sender, SpeechDetectedEventArgs e)
        {
            tb_CheechToText.Text = "Result: [Listening...]";
        }

        private void Engine_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            tb_CheechToText.Text = "Result: " + e.Result.Text;
        }

        private void bt_settings_Click(object sender, RoutedEventArgs e)
        {
            SetUpWindow setupWin = new SetUpWindow(engine);
            setupWin.ShowDialog();
            this.Close();
        }
    }
}