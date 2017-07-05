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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.IO;
using System.Data;
using System.Security.Cryptography;


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
                SQLiteDataAdapter da = new SQLiteDataAdapter("select * from urls order by last_visit_time desc LIMIT 30", con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dataGridView1.ItemsSource = ds.Tables[0].AsDataView();
                con.Close();
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

        private async Task<DataTable> GetPasswordsAsync()
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
        }

        private async void btnGetPasswords_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = await GetPasswordsAsync();
                dataGridView1.ItemsSource = dt.AsDataView();
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
        public static DataTable ReverseRowsInDataTable(DataTable inputTable)
        {
            DataTable outputTable = inputTable.Clone();

            for (int i = inputTable.Rows.Count - 1; i >= 0; i--)
            {
                outputTable.ImportRow(inputTable.Rows[i]);
            }

            return outputTable;
        }
    }
}
