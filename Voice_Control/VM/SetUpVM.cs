using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Voice_Control.VM
{
    internal class SetUpVM
    {
        private IReadOnlyCollection<RecognizerInfo> availableLangs;
        public CommandList selectedConfig
        {
            get { return SetUpCookies.selectedConfig; }
            set { SetUpCookies.selectedConfig = value; }
        }
        public CultureInfo selectedCulture
        {
            get { return SetUpCookies.selectedCulture; }
            set { SetUpCookies.selectedCulture = value; }
        }

        public string tb_configName { get; set; }
        public Brush tb_configNameForeGr { get; set; }
        public string tb_configLang { get; set; }
        public Brush tb_configLangForeGr { get; set; }
        public bool bt_finishIsEnabled { get; set; }

        public SetUpVM(SpeechRecognitionEngine engine)
        {
            bt_finishIsEnabled = false;

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
            tb_configName = selectedConfig.Name;
            tb_configNameForeGr = Brushes.Black;
            tb_configLang = selectedConfig.Culture;
            tb_configLangForeGr = Brushes.Black;
            bt_finishIsEnabled = true;

            foreach (var lang in availableLangs)
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
                tb_configLangForeGr = Brushes.Red;
                bt_finishIsEnabled = false;
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
                tb_configNameForeGr = Brushes.Red;
                bt_finishIsEnabled = false;
                return;
            }
        }

        private RelayCommand finishClick;
        public RelayCommand FinishClick
        {
            get
            {
                return finishClick ?? (finishClick = new RelayCommand(obj =>
                {
                    SetUpWindow setUpWin = obj as SetUpWindow;
                    setUpWin.DialogResult = true;
                    setUpWin.Close();
                }));
            }
        }

        private RelayCommand createClick;
        public RelayCommand CreateClick
        {
            get
            {
                return createClick ?? (createClick = new RelayCommand(obj =>
                {
                    List<CultureInfo> cultures = new List<CultureInfo>();
                    foreach (var lang in availableLangs) { cultures.Add(lang.Culture); }
                    cfgCreationWindow cfgWin = new cfgCreationWindow(cultures.ToArray());
                    cfgWin.ShowDialog();
                    if (cfgWin.DialogResult == true) { UpdateAll(); }
                }));
            }
        }

        private RelayCommand loadClick;
        public RelayCommand LoadClick
        {
            get
            {
                return loadClick ?? (loadClick = new RelayCommand(obj =>
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
                            selectedConfig = JsonSerializer.Deserialize<CommandList>(fs);
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
                }));
            }
        }
    }
}
