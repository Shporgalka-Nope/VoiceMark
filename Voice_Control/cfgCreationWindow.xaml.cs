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
using Voice_Control.VM;

namespace Voice_Control
{
    public partial class cfgCreationWindow : Window
    {
        public cfgCreationWindow(CultureInfo[] cultures)
        {
            InitializeComponent();
            DataContext = new cfgCreationVM(cultures);
        }
    }
}
