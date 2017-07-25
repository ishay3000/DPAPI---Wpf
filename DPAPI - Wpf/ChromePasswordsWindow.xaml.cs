﻿using System;
using System.Collections.Generic;
using System.Data;
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

namespace DPAPI___Wpf
{
    /// <summary>
    /// Interaction logic for ChromePasswordsWindow.xaml
    /// </summary>
    public partial class ChromePasswordsWindow : Window
    {
        DataTable _dt;
        public ChromePasswordsWindow(DataTable dt)
        {
            _dt = dt;
            InitializeComponent();
            gv.ItemsSource = _dt.AsDataView();
        }
    }
}
