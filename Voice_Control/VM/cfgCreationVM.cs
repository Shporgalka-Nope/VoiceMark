using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Voice_Control.VM
{
    internal class cfgCreationVM
    {
        private CultureInfo[] _cultures; //Needed for new cfg creation (not used here)
        private CommandList currentList; //Stores all CAction after JSON load
        private string selectedCfgPath = null;

        public string tb_cfgName;
        public string tb_cfgCulture;
        public bool bt_saveIsEnabled;
        public bool bt_delIsEnabled;
        public bool bt_delLineIsEnabled;
        public bool bt_addNewLineIsEnabled;
        public bool tbox_lineNumIsEnabled;
        public UIElementCollection sp_allCommands;

        public cfgCreationVM(CultureInfo[] cultures)
        {
            _cultures = cultures;
        }

        private RelayCommand newClick;
        public RelayCommand NewClick
        {
            get
            {
                return newClick ?? (newClick = new RelayCommand(obj =>
                {
                    newCfgSetupWindow createCfg = new newCfgSetupWindow(_cultures);
                    createCfg.ShowDialog();

                    if (createCfg.DialogResult == true) { LoadJson(newCfgCookies.pathToCfg); }
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
                    OpenFileDialog op = new OpenFileDialog();
                    op.Title = "Pick json file";
                    op.Filter = "Json Files (*.json)|*.json";
                    op.InitialDirectory = Directory.GetCurrentDirectory() + @"\jsons";
                    if (op.ShowDialog() == true) { LoadJson(op.FileName); }
                }));
            }
        }

        private RelayCommand delClick;
        public RelayCommand DelClick
        {
            get
            {
                return delClick ?? (delClick = new RelayCommand(obj =>
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
                        tb_cfgName = "-";
                        tb_cfgCulture = "-";
                        selectedCfgPath = null;
                        currentList = null;
                        bt_delIsEnabled = false;
                        UpdateScroll();
                    }
                }));
            }
        }

        private RelayCommand newLineClick;
        public RelayCommand NewLineClick
        {
            get
            {
                return newLineClick ?? (newLineClick = new RelayCommand(obj =>
                {
                    newLineWindow newLineWin = new newLineWindow();
                    newLineWin.ShowDialog();

                    List<Command> newCommands;
                    if (newLineWin.DialogResult == true)
                    {
                        if (currentList.Commands != null) { newCommands = currentList.Commands.ToList(); }
                        else { newCommands = new List<Command>(); }
                        Command cmd = new Command(newLineCookies.ActionPhrase, newLineCookies.ActionType, newLineCookies.ActionArgument);
                        newCommands.Add(cmd);
                        currentList.Commands = newCommands.ToArray();
                        UpdateScroll();
                    }
                }));
            }
        }

        private RelayCommand saveClick;
        public RelayCommand SaveClick
        {
            get
            {
                return saveClick ?? (saveClick = new RelayCommand(obj =>
                {
                    try
                    {
                        string dir = selectedCfgPath;
                        File.Delete(dir);

                        using (FileStream fs = new FileStream(dir, FileMode.OpenOrCreate))
                        {
                            CommandList newList = currentList;
                            JsonSerializer.Serialize<CommandList>(fs, newList);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An unexpected error occured!\n{ex.Message}\n{ex.TargetSite}");
                        return;
                    }
                    cfgCreationWindow cfgCreate = obj as cfgCreationWindow;
                    cfgCreate.DialogResult = true;
                    cfgCreate.Close();
                }));
            }
        }

        private RelayCommand delLineClick;
        public RelayCommand DelLineClick
        {
            get
            {
                return delLineClick ?? (delLineClick = new RelayCommand(obj =>
                {
                    if (int.TryParse((obj as TextBox).Text, out int num))
                    {
                        if (num < 1 || num > currentList.Commands.Length) { return; }
                        List<Command> newList = currentList.Commands.ToList<Command>();
                        newList.RemoveAt(num - 1);
                        currentList.Commands = newList.ToArray();
                        UpdateScroll();
                    }
                }));
            }
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

            tb_cfgName = newList.Name;
            tb_cfgCulture = newList.Culture;
            selectedCfgPath = path;

            bt_delIsEnabled = true;
            bt_addNewLineIsEnabled = true;
        }
        private void UpdateScroll()
        {
            StackPanel empty = new StackPanel();
            sp_allCommands = empty.Children;
            if (currentList.Commands == null)
            {
                bt_saveIsEnabled = false;
                bt_addNewLineIsEnabled = false;
                bt_delLineIsEnabled = false;
                tbox_lineNumIsEnabled = false;
                return;
            }
            int lineNum = 0;
            foreach (Command act in currentList.Commands)
            {
                Border border = new Border();
                StackPanel newPanel = new StackPanel();
                newPanel.Orientation = Orientation.Horizontal;
                border.Child = newPanel;
                border.BorderThickness = new Thickness(2);

                TextBlock phrase = new TextBlock();
                phrase.Text = $"{lineNum + 1} Phrase: {act.Phrase}; ";
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
                sp_allCommands.Add(border);
            }
            bt_saveIsEnabled = true;
            bt_addNewLineIsEnabled = true;
            bt_delLineIsEnabled = true;
            tbox_lineNumIsEnabled = true;
            lineNum++;
        }
    }
}
