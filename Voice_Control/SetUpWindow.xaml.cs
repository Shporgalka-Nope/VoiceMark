using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Voice_Control
{
    /// <summary>
    /// Логика взаимодействия для SetUpWindow.xaml
    /// </summary>
    public partial class SetUpWindow : Window
    {
        private IReadOnlyCollection<RecognizerInfo> availableLangs;
        public CommandList selectedConfig = null;
        public CultureInfo selectedCulture = null;

        public SetUpWindow(SpeechRecognitionEngine engine)
        {
            InitializeComponent();

            //Culture part
            availableLangs = SpeechRecognitionEngine.InstalledRecognizers();
            if (availableLangs == null) 
            { 
                MessageBox.Show("Error getting languages"); 
                Application.Current.Shutdown();
            }

            //Config part
            string dir = Directory.GetCurrentDirectory() + @"\jsons";
            Directory.CreateDirectory(dir);
        }

        private void UpdateAll()
        {
            tb_configName.Text = selectedConfig.Name;
            tb_configName.Foreground = Brushes.Black;
            tb_configLang.Text = selectedConfig.Culture;
            tb_configLang.Foreground = Brushes.Black;
            bt_finish.IsEnabled = true;

            foreach(var lang in availableLangs)
            {
                if (lang.Culture.Name == selectedConfig.Culture) { selectedCulture = lang.Culture; }
            }

            if (selectedCulture == null)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Language of selected config is missing from installed voice recognision packs. " +
                    $"\nCheck your installed languages packs and try again. " +
                    $"\nMissing voice recognision pack for: {selectedConfig.Culture}",
                    "Missing voice pack",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
                tb_configLang.Foreground = Brushes.Red;
                bt_finish.IsEnabled = false;
                return; 
            }
            if (selectedConfig.Commands == null || selectedConfig.Commands.Length == 0)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Selected config has no commands in it." +
                    $"\nAdd commands in CFG Editor and try again.",
                    "Missing commands",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
                tb_configName.Foreground = Brushes.Red;
                bt_finish.IsEnabled = false;
                return;
            }
        }

        private void bt_finish_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
        private void bt_createCfg_Click(object sender, RoutedEventArgs e)
        {
            List<CultureInfo> cultures = new List<CultureInfo>();
            foreach (var lang in availableLangs) { cultures.Add(lang.Culture); }
            cfgCreationWindow cfgWin = new cfgCreationWindow(cultures.ToArray());
            cfgWin.ShowDialog();
            if (DialogResult == true)
            {
                UpdateAll();
            }
        }
        private async void bt_load_Click(object sender, RoutedEventArgs e)
        {
            string? cfgDir = null;
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Pick json file";
            op.Filter = "Json Files (*.json)|*.json";
            op.InitialDirectory = Directory.GetCurrentDirectory() + @"\jsons";
            if (op.ShowDialog() == true) { cfgDir = op.FileName; }
            else { return; }

            try
            {
                using (FileStream fs = new FileStream(cfgDir, FileMode.Open))
                {
                    selectedConfig = await JsonSerializer.DeserializeAsync<CommandList>(fs);
                }
                if (selectedConfig == null)
                {
                    throw new NullReferenceException("NewList was null!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occured!\n{ex.Message}\n{ex.TargetSite}");
                return;
            }
            UpdateAll();
        }
    }
}
