using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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
        public string ActionsPhrase { get; set; }
        public int ActionType { get; set; }
        public string ActionArgument { get; set; }

        private CAction[] actionsArray;

        public newLineWindow()
        {
            InitializeComponent();

            foreach(CAction act in CActions.ActionList)
            {
                cb_action.Items.Add(act.Discription);
            }

            actionsArray = CActions.ActionList.ToArray();
        }

        private void bt_done_Click(object sender, RoutedEventArgs e)
        {
            if (tbox_phrase.Text.Trim() == "")
            {
                MessageBox.Show("Key phrase required");
                return;
            }
            if (cb_action.SelectedItem == null)
            {
                MessageBox.Show("Action required");
                return; 
            }
            if (actionsArray[cb_action.SelectedIndex].argRequired && tbox_arg.Text.Trim() == "")
            {
                MessageBox.Show("Argument required");
                return;
            }

            ActionsPhrase = tbox_phrase.Text;
            ActionType = actionsArray[cb_action.SelectedIndex].ActionNum;
            ActionArgument = tbox_arg.Text;

            DialogResult = true;
            this.Close();
        }
    }
}
