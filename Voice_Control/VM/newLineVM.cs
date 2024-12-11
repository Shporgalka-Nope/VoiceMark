using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Voice_Control.VM
{
    internal class newLineVM
    {
        public string ActionsPhrase { get; set; }
        public int ActionType { get; set; }
        public string ActionArgument { get; set; }

        private CAction[] actionsArray;

        public List<string> cb_action { get; set; } = new List<string>();
        public string tbox_phrase { get; set; }
        public object cb_actionSelectedItem { get; set; }
        public int cb_actionSelectedIndex { get; set; }
        public string tbox_arg { get; set; }

        public newLineVM()
        {
            foreach (CAction act in CActions.ActionList)
            {
                cb_action.Add(act.Discription);
            }
            actionsArray = CActions.ActionList.ToArray();
        }

        private RelayCommand doneClick;
        public RelayCommand DoneClick
        {
            get
            {
                return doneClick ?? (doneClick = new RelayCommand(obj =>
                {
                    if (tbox_phrase.Trim() == "")
                    {
                        MessageBox.Show("Key phrase required");
                        return;
                    }
                    if (cb_actionSelectedItem == null)
                    {
                        MessageBox.Show("Action required");
                        return;
                    }
                    if (actionsArray[cb_actionSelectedIndex].argRequired && tbox_arg.Trim() == "")
                    {
                        MessageBox.Show("Argument required");
                        return;
                    }

                    ActionsPhrase = tbox_phrase;
                    ActionType = actionsArray[cb_actionSelectedIndex].ActionNum;
                    ActionArgument = tbox_arg;

                    newLineCookies.ActionPhrase = ActionsPhrase;
                    newLineCookies.ActionType = ActionType;
                    newLineCookies.ActionArgument = ActionArgument;

                    (obj as newLineWindow).DialogResult = true;
                    (obj as newLineWindow).Close();
                }));
            }
        }
    }
}
