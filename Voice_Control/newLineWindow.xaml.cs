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
using Voice_Control.VM;

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
            DataContext = new newLineVM();
        }
    }
}
