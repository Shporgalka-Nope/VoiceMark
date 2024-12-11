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

        public List<CultureInfo> cb_cultures { get; set; } = new List<CultureInfo>();
        public int cb_culturesSelectedIndex { get; set; }
        public object cb_culturesSelectedItem { get; set; }
        public bool bt_finishIsEnabled { get; set; }
        public string tbox_cfgName { get; set; }
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

                        using (FileStream fs = new FileStream(dir + $@"\{tbox_cfgName}.json", FileMode.OpenOrCreate))
                        {
                            CommandList newList = new CommandList(tbox_cfgName, null, (CultureInfo)cb_culturesSelectedItem, false);
                            JsonSerializer.Serialize<CommandList>(fs, newList);
                            pathToCfg = dir + $@"\{tbox_cfgName}.json";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An unexpected error occured!\n{ex.Message}\n{ex.TargetSite}");
                        return;
                    }
                    newCfgCookies.pathToCfg = pathToCfg;
                    (obj as newCfgSetupWindow).DialogResult = true;
                    (obj as newCfgSetupWindow).Close();
                }));
            }
        }

        private RelayCommand cfgName_TextChanged;
        public RelayCommand CfgName_TextChanged
        {
            get
            {
                return finishClick ?? (finishClick = new RelayCommand(obj =>
                { 
                    if (tbox_cfgName.Trim() == "") { bt_finishIsEnabled = false; }
                    else { bt_finishIsEnabled = true; }
                }));
            }
        }
    }
}
