using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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
    public partial class cfgCreationWindow : Window
    {
        private CultureInfo[] _cultures;
        private CommandList currentList;
        private string selectedCfgPath = null;

        public cfgCreationWindow(CultureInfo[] cultures)
        {
            InitializeComponent();
            _cultures = cultures;
            //When opening this window, directory setup should
            //already be done and ready for new jsons to be created
            

        }

        private void bt_new_Click(object sender, RoutedEventArgs e)
        {
            newCfgSetupWindow createCfg = new newCfgSetupWindow(_cultures);
            createCfg.ShowDialog();

            if (createCfg.DialogResult == true) { LoadJson(createCfg.pathToCfg); }
        }

        private async void LoadJson(string path)
        {
            CommandList newList = null;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    newList = await JsonSerializer.DeserializeAsync<CommandList>(fs);
                }
                if (newList == null)
                {
                    throw new NullReferenceException("newList was null!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occured!\n{ex.Message}\n{ex.TargetSite}");
                return;
            }
            currentList = newList;

            tb_cfgName.Text = newList.Name;
            tb_cfgCulture.Text = newList.Culture;
            dg_commands.ItemsSource = newList.Commands;
            selectedCfgPath = path;
            bt_del.IsEnabled = true;

            bt_addNewLine.IsEnabled = true;
        }

        private void bt_load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Pick json file";
            op.Filter = "Json Files (*.json)|*.json";
            op.InitialDirectory = Directory.GetCurrentDirectory() + @"\jsons";
            if (op.ShowDialog() == true) { LoadJson(op.FileName); }
            else { return; }
        }

        private void bt_del_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure, that you want to delete this config?\nThis action cannot be undone.",
                "Deleting a config.",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
                );

            if (result == MessageBoxResult.Yes) 
            {
                File.Delete(selectedCfgPath);
                dg_commands.Items.Clear();
                tb_cfgName.Text = "-";
                tb_cfgCulture.Text = "-";
                selectedCfgPath = null;
                bt_del.IsEnabled = false;
            }
        }

        private void bt_addNewLine_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
