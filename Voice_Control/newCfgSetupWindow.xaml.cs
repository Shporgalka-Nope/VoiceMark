using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
    /// Логика взаимодействия для newCfgSetupWindow.xaml
    /// </summary>
    public partial class newCfgSetupWindow : Window
    {
        public string pathToCfg;

        public newCfgSetupWindow(CultureInfo[] cultures)
        {
            InitializeComponent();

            foreach (CultureInfo cult in cultures)
            {
                cb_cultures.Items.Add(cult);
            }
            cb_cultures.SelectedIndex = 0;
            bt_finish.IsEnabled = false;
        }

        private async void bt_finish_Click(object sender, RoutedEventArgs e)
        {
            //cfgName = tbox_cfgName.Text;
            //selectedCulture = (CultureInfo)cb_cultures.SelectedItem;
            try
            {
                string dir = Directory.GetCurrentDirectory() + @"\jsons";

                using (FileStream fs = new FileStream(dir + $@"\{tbox_cfgName.Text}.json", FileMode.OpenOrCreate))
                {
                    CommandList newList = new CommandList(tbox_cfgName.Text, null, (CultureInfo)cb_cultures.SelectedItem);
                    await JsonSerializer.SerializeAsync<CommandList>(fs, newList);
                    pathToCfg = dir + $@"\{tbox_cfgName.Text}.json";
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"An unexpected error occured!\n{ex.Message}\n{ex.TargetSite}");
                return;
            }
            DialogResult = true;
            this.Close();
        }

        private void tbox_cfgName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbox_cfgName.Text.Trim() == "") { bt_finish.IsEnabled = false; }
            else { bt_finish.IsEnabled = true; }
        }
    }
}
