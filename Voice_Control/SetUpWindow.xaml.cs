using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
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
        private List<CultureInfo> cultures = new List<CultureInfo>();
        public CultureInfo selectedCulture;

        public SetUpWindow(SpeechRecognitionEngine engine)
        {
            InitializeComponent();

            //Culture part
            IReadOnlyCollection<RecognizerInfo> languages = SpeechRecognitionEngine.InstalledRecognizers();
            if (languages == null) 
            { 
                MessageBox.Show("Error getting languages"); 
                Application.Current.Shutdown();
            }
            foreach (var language in languages)
            {
                CultureInfo info = language.Culture;
                cultures.Add(info);
                cb_languageOptions.Items.Add(info);
            }
            cb_languageOptions.SelectedIndex = 0;

            //Config part
            string dir = Directory.GetCurrentDirectory() + @"\jsons";
            Directory.CreateDirectory(dir);
            string[] files = Directory.GetFiles(dir, "*.json");
            foreach (string file in files)
            {

            }
        }

        private void cb_languageOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCulture = (CultureInfo)cb_languageOptions.SelectedItem;
        }

        private void bt_finish_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void bt_createCfg_Click(object sender, RoutedEventArgs e)
        {
            cfgCreationWindow cfgWin = new cfgCreationWindow(cultures.ToArray());
            cfgWin.ShowDialog();
            
        }
    }
}
