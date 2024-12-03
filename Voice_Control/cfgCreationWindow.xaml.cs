using Accessibility;
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
        private CultureInfo[] _cultures; //Needed for new cfg creation (not used here)
        private CommandList currentList; //Stores all CAction after JSON load
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
            UpdateScroll();

            tb_cfgName.Text = newList.Name;
            tb_cfgCulture.Text = newList.Culture;
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
                tb_cfgName.Text = "-";
                tb_cfgCulture.Text = "-";
                selectedCfgPath = null;
                currentList = null;
                bt_del.IsEnabled = false;
                UpdateScroll();
            }
        }
        private void bt_addNewLine_Click(object sender, RoutedEventArgs e)
        {
            newLineWindow newLineWin = new newLineWindow();
            newLineWin.ShowDialog();

            List<Command> newCommands;
            if (DialogResult != true) 
            {
                if (currentList.Commands != null) { newCommands = currentList.Commands.ToList(); }
                else { newCommands = new List<Command>(); }
                Command cmd = new Command(newLineWin.ActionsPhrase, newLineWin.ActionType, newLineWin.ActionArgument);
                newCommands.Add(cmd);
                currentList.Commands = newCommands.ToArray();
                UpdateScroll(); 
            }
        }

        private void UpdateScroll()
        {
            sp_allCommands.Children.Clear();
            if (currentList.Commands == null) 
            {
                bt_save.IsEnabled = false;
                bt_addNewLine.IsEnabled = false;
                bt_delLine.IsEnabled = false;
                tbox_lineNum.IsEnabled = false;
                return; 
            }
            int lineNum = 0;
            foreach(Command act in currentList.Commands)
            {
                Border border = new Border();
                StackPanel newPanel = new StackPanel();
                newPanel.Orientation = Orientation.Horizontal;
                border.Child = newPanel;
                border.BorderThickness = new Thickness(2);

                TextBlock phrase = new TextBlock();
                phrase.Text = $"{lineNum+1} Phrase: {act.Phrase}; ";
                phrase.FontSize = 14;
                phrase.Foreground = Brushes.White;

                TextBlock action = new TextBlock();
                int i = 0;
                while (act.Action != CActions.ActionList[i].ActionNum)
                {
                    i++;
                }
                action.Text = $"Action: {CActions.ActionList[i].Discription}; ";
                action.FontSize = 14;
                action.Foreground = Brushes.White;

                TextBlock argument = new TextBlock();
                argument.Text = $"Argument: {act.Argument}; ";
                argument.FontSize = 14;
                argument.Foreground = Brushes.White;

                newPanel.Children.Add(phrase);
                newPanel.Children.Add(action);
                newPanel.Children.Add(argument);
                sp_allCommands.Children.Add(border);
            }
            bt_save.IsEnabled = true;
            bt_addNewLine.IsEnabled = true;
            bt_delLine.IsEnabled = true;
            tbox_lineNum.IsEnabled = true;
            lineNum++;
        }

        private async void bt_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dir = selectedCfgPath;
                File.Delete(dir);

                using (FileStream fs = new FileStream(dir, FileMode.OpenOrCreate))
                {
                    CommandList newList = currentList;
                    await JsonSerializer.SerializeAsync<CommandList>(fs, newList);
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

        private void bt_delLine_Click(object sender, RoutedEventArgs e)
        {
            if(int.TryParse(tbox_lineNum.Text, out int num))
            {
                if(num < 1 || num > currentList.Commands.Length) { return; }
                List<Command> newList = currentList.Commands.ToList<Command>();
                newList.RemoveAt(num - 1);
                currentList.Commands = newList.ToArray();
                UpdateScroll();
            }
            else { return; }
        }
    }
}
