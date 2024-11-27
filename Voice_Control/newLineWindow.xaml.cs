using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для newLineWindow.xaml
    /// </summary>
    public partial class newLineWindow : Window
    {
        public newLineWindow()
        {
            InitializeComponent();

            foreach (CAction act in CActions.ActionList)
            {
                cb_action.Items.Add(act);
            }
        }

        private void tbox_phrase_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbox_phrase.Text.Trim() != "") { cb_action.IsEnabled = true; }
        }

        private void cb_action_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CAction act = cb_action.SelectedItem as CAction;
            if (act.argRequired) { tbox_arg.IsEnabled = true; }
        }

        private void ConditionsCheck()
        {
            bool flag = true;
            if (tbox_arg.Text.Trim() == "") { flag = false; }
            else { cb_action.IsEnabled = true; }
            if (cb_action.SelectedItem == null) { flag = false; }
            CAction act = cb_action.SelectedItem as CAction;
            if (act.argRequired && tbox_arg.Text.Trim() == "") { flag = false; }

            if (flag) { bt_done.IsEnabled = true; }
        }
    }
}
