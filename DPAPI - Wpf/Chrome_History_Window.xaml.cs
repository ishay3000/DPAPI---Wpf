using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
        public DataTable dt;
        HashSet<string> hs = null;

        public Chrome_History_Window(DataTable dt)
        {
            try
            {
                InitializeComponent();
                
                this.dt = dt;
                gv.ItemsSource = dt.AsDataView();
                #region comments
                //gv.Columns[1].Width = 300;
                //foreach (var column in gv.Columns)
                //    column.Width = DataGridLength.SizeToHeader;
                //gv.Columns.Last().Width = DataGridLength.SizeToCells;
                #endregion
                //tbSearchUrl.GotFocus += GotFocus.EventHandle(RemoveText);
                //Keyboard.ClearFocus();
                hs = new HashSet<string>();
                
                //Parallel.ForEach(dt.AsEnumerable(), drow =>
                //{
                //    hs.Add(drow[1].ToString());
                //});
                tbSearchSite.Text = "Enter Site here...";
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

        //private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    gv.Width = e.NewSize.Width;
        //    gv.Height = e.NewSize.Height;
        //}

        private void gv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MessageBoxResult r = MessageBox.Show("Are you sure you want to close the screen ?", "Close History", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (r == MessageBoxResult.Yes)
                {
                    GC.Collect();
                    Close();
                }

            }
            else if (e.Key == Key.Enter)
            {

            }
        }

        private void tbSearchSite_GotFocus(object sender, RoutedEventArgs e)
        {
            tbSearchSite.Text = string.Empty;
        }

        private void tbSearchSite_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearchSite.Text))
            {
                tbSearchSite.Text = "Enter Site here...";
            }
        }

        protected async Task BtnSearchMethod()
        {
            await Task.Run(async () =>
            {
                try
                {
                    await Dispatcher.Invoke(async () =>
                    {
                        DataTable tmp = await SearchDatatablEntry();
                        gv.ItemsSource = tmp.AsDataView();
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Dispatcher.Invoke(async () =>
                {
                    await BtnSearchMethod();
                });
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            #region comments
            //try
            //{
            //    await Dispatcher.Invoke(async () =>
            //     {
            //         DataTable tmp = await SearchDatatablEntry();
            //         gv.ItemsSource = tmp.AsDataView();
            //     });
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
            //try
            //{
            //    DataTable tmpTable = null;
            //Dispatcher.Invoke(() =>
            //{
            //    Parallel.ForEach(dt.AsEnumerable(), myRow =>
            //    {
            //            if (myRow[1].ToString().Contains(tbSearchSite.Text))
            //            {
            //                tmpTable = new DataTable();
            //                tmpTable.ImportRow(myRow);
            //            }
            //    });
            //    gv.ItemsSource = tmpTable.AsDataView();
            //});
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
            #endregion

        }

        protected async Task<DataTable> SearchDatatablEntry()
        {
                return await Task.Run(() =>
                {

                    try
                    {
                        DataTable tmpTable = new DataTable();
                        tmpTable = dt.Clone();
                        DataRow dr = this.dt.NewRow();
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                dr = row;
                                string temp = row[1].ToString();
                                if (temp.Contains(tbSearchSite.Text))
                                {
                                    tmpTable.ImportRow(dr);
                                }
                            }
                            #region comments
                            //Parallel.ForEach(dt.AsEnumerable(), myRow =>
                            //    {
                            //        Dispatcher.Invoke(() =>
                            //        {
                            //            dr[0] = myRow[0];
                            //            dr[1] = myRow[1];
                            //            dr[2] = myRow[2];
                            //            dr[3] = myRow[3];
                            //            dr[4] = myRow[4];
                            //            string tmpStr = tbSearchSite.Text;
                            //            string rowStr = (string)dr[1];
                            //            if (rowStr.Contains(tbSearchSite.Text))
                            //            {
                            //                tmpTable = new DataTable();
                            //                tmpTable.ImportRow(dr);
                            //            }
                            //        });
                            //    });
#endregion
                        });
                        return tmpTable;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return null;
                    }
                });
          
        }

        private async void tbSearchSite_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                await Dispatcher.Invoke(async () =>
                 {
                     await BtnSearchMethod();
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    await BtnSearchMethod();
                }
                 });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
