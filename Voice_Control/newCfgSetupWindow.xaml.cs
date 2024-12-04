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
using Voice_Control.VM;

namespace Voice_Control
{
    /// <summary>
    /// Логика взаимодействия для newCfgSetupWindow.xaml
    /// </summary>
    public partial class newCfgSetupWindow : Window
    {
        public newCfgSetupWindow(CultureInfo[] cultures)
        {
            InitializeComponent();
            DataContext = new newCfgVM(cultures);
        }
    }
}
