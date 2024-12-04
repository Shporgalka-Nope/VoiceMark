using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Voice_Control.VM
{
    internal class newCfgVM
    {
        public string pathToCfg;

        public ItemCollection cb_cultures;
        public int cb_culturesSelectedIndex;
        public object cb_culturesSelectedItem;
        public bool bt_finishIsEnabled;
        public newCfgVM(CultureInfo[] cultures)
        {
            foreach (CultureInfo cult in cultures)
            {
                cb_cultures.Add(cult);
            }
            cb_culturesSelectedIndex = 0;
            bt_finishIsEnabled = false;
        }

        private RelayCommand finishClick;
        public RelayCommand FinishClick
        {
            get
            {
                return finishClick ?? (finishClick = new RelayCommand(obj =>
                {
                    try
                    {
                        string dir = Directory.GetCurrentDirectory() + @"\jsons";

                        using (FileStream fs = new FileStream(dir + $@"\{(obj as TextBox).Text}.json", FileMode.OpenOrCreate))
                        {
                            CommandList newList = new CommandList((obj as TextBox).Text, null, (CultureInfo)cb_culturesSelectedItem, false);
                            JsonSerializer.Serialize<CommandList>(fs, newList);
                            pathToCfg = dir + $@"\{(obj as TextBox).Text}.json";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An unexpected error occured!\n{ex.Message}\n{ex.TargetSite}");
                        return;
                    }
                    DialogResult = true;
                    this.Close();
                }));
            }
        }
        private void tbox_cfgName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbox_cfgName.Text.Trim() == "") { bt_finish.IsEnabled = false; }
            else { bt_finish.IsEnabled = true; }
        }
    }
}
