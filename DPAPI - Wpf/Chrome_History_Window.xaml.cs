using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DPAPI___Wpf
{
    /// <summary>
    /// Interaction logic for Chrome_History_Window.xaml
    /// </summary>
    public partial class Chrome_History_Window : Window
    {
        protected DataTable dt;
        //HashSet<string> hs = null;

        public Chrome_History_Window(DataTable dt)
        {
            try
            {
                InitializeComponent();
                this.dt = dt;
                gv.ItemsSource = dt.AsDataView();
                //hs = new HashSet<string>();
                //Parallel.ForEach(dt.AsEnumerable(), drow =>
                //{
                //    hs.Add(drow[1].ToString());
                //});
                //gv.Columns[1].Width = 300;
                //foreach (var column in gv.Columns)
                //    column.Width = DataGridLength.SizeToHeader;
                //gv.Columns.Last().Width = DataGridLength.SizeToCells;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void gv_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (gv.SelectedItem == null)
                {
                    return;
                }
                DataRowView dr = gv.SelectedItem as DataRowView;
                DataRow myRow = dr.Row;
                using (Process p = new Process())
                {
                    MessageBoxResult result = MessageBox.Show($"Are you sure you want to open {myRow[2].ToString()} ?", "Opening URL", MessageBoxButton.OKCancel, MessageBoxImage.Information, MessageBoxResult.Cancel, MessageBoxOptions.None);
                    if (result == MessageBoxResult.OK)
                    {
                        Process.Start("firefox.exe", myRow[1].ToString());

                    }
                    //MessageBox.Show($"Are you sure you want to open {myRow[2].ToString()} ?", "Opening URL", MessageBoxButton.OKCancel);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }

        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gv.Width = e.NewSize.Width;
            gv.Height = e.NewSize.Height;
        }

        private void gv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
