using ClothesShopCursovaya.Extensions;
using Microsoft.Win32;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
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

namespace ClothesShopCursovaya
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        Authorisation auth = Application.Current.Windows.OfType<Authorisation>().FirstOrDefault();
        NpgsqlCommand command = new NpgsqlCommand();
        DataTable dataTable = new DataTable();
        Helper helper = new Helper();
        public AdminPage()
        {
            InitializeComponent();
            connectionString = new NpgsqlConnection(helper.connect);
            connectionString.Open();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("select * from Post", connectionString);
            adapter.Fill(dataTable);
            cbPost.DisplayMemberPath = "post_name";
            cbPost.SelectedValuePath = "id_post";
            cbPost.ItemsSource = dataTable.DefaultView;
            btnEmployeeRestore.Visibility = Visibility.Hidden;
            Refresh();
        }
        /// <summary>
        /// Выгрузка в CSV
        /// </summary>
        /// <param name="dtDataTable">Данные из Datagrid</param>
        /// <param name="strFilePath">Путь до файла</param>
        public static void ToCSV( DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
        /// <summary>
        /// Обработчик кнопок 
        /// </summary>
        /// <param name="sender">ссылка на элемент управления/объект, вызвавший событие</param>
        /// <param name="e">экземпляр класса для классов, содержащих данные событий, и предоставляет данные событий</param>
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)dg.SelectedItem;
            if (e.Source == btnBack)
            {
                    auth.MainFrame.NavigationService.Navigate(new SignIn());
            }
            if(e.Source == btnCSVV)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV Files (.csv)|*.csv";
                saveFileDialog.InitialDirectory = @"C:\Users\Дмитрий\Desktop";

                if (saveFileDialog.ShowDialog() == true)
                {
                    ToCSV(dataTable, saveFileDialog.FileName);
                }

                MessageBox.Show("Выгрузка в CSV прошла успешно");

            }
            if(e.Source == btnCopy)
            {
                SaveFileDialog dialog = new SaveFileDialog()
                {
                    Filter = "BACKUP Files|*.backup*"
                };
                dialog.Title = "Save as backup file";
                if (dialog.ShowDialog() == true)
                {
                    Backup(dialog.FileName + ".backup");
                }
            }
            if(e.Source == btnRestore)
            {
                OpenFileDialog dialog = new OpenFileDialog()
                {
                    Filter = "BACKUP Files|*.backup*"
                };
                dialog.Title = "Choose backup file";
                if (dialog.ShowDialog() == true)
                {
                    Restore(dialog.FileName);
                }
            }
            if (e.Source == btnDeleted)
            {
                if (btnDeleted.Header.ToString() == "Уволенные")
                {
                    tbSurname.Visibility = tbName.Visibility = tbPatronymic.Visibility = tbEmail.Visibility = tbPhone.Visibility = tbSeries.Visibility
                        = tbNumber.Visibility = tbLogin.Visibility = tbPassword.Visibility = cbPost.Visibility = lbname.Visibility =
                        lbSur.Visibility = lbpatr.Visibility = lbphone.Visibility = lbemail.Visibility = lblog.Visibility = lbpas.Visibility =
                        lbpost.Visibility = lbser.Visibility = lbnum.Visibility = btnAdd.Visibility = btnChange.Visibility = btnDelete.Visibility =
                        Visibility.Hidden;
                    btnEmployeeRestore.Visibility = Visibility.Visible;
                    DeletedRefresh();
                    btnDeleted.Header = "Работающие";
                }
                else
                {
                    tbSurname.Visibility = tbName.Visibility = tbPatronymic.Visibility = tbEmail.Visibility = tbPhone.Visibility = tbSeries.Visibility
                                            = tbNumber.Visibility = tbLogin.Visibility = tbPassword.Visibility = cbPost.Visibility = lbname.Visibility =
                                            lbSur.Visibility = lbpatr.Visibility = lbphone.Visibility = lbemail.Visibility = lblog.Visibility = lbpas.Visibility =
                                            lbpost.Visibility = lbser.Visibility = lbnum.Visibility = btnAdd.Visibility = btnChange.Visibility = btnDelete.Visibility =
                                            Visibility.Visible;
                    btnEmployeeRestore.Visibility = Visibility.Hidden;
                    Refresh();
                    btnDeleted.Header = "Уволенные";
                }    
            }
            if (e.Source == btnAdd || e.Source == btnChange)
            {
                try
                {
                    if (helper.IsNull(tbName.Text) && helper.IsNull(tbSurname.Text) && (!tbPhone.Text.Contains("_")) && (!tbNumber.Text.Contains("_")) && (!tbSeries.Text.Contains("_")) && helper.IsNull(tbLogin.Text) && helper.IsNull(tbPassword.Text) && helper.IsNull(tbEmail.Text) && helper.IsNull(cbPost.Text))
                    {
                        if (tbLogin.Text.Length < 8)
                        {
                            MessageBox.Show("Логин должен быть не короче 8 символов", "Логин", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (helper.IsValidEmail(tbEmail.Text))
                            if (helper.IsValidPassword(tbPassword.Text))
                            {
                                if(e.Source==btnAdd)
                                    command = new NpgsqlCommand($@"call Employee_Insert('{tbSurname.Text}','{tbName.Text}','{tbPatronymic.Text}','{tbEmail.Text}','{tbSeries.Text}','{tbNumber.Text}','{tbPhone.Text}','{tbLogin.Text}','{tbPassword.Text}','false','{cbPost.SelectedValue}')", connectionString);
                                else if(row != null)
                                    command = new NpgsqlCommand($@"call Employee_Update('{(int)row["Табельный номер"]}','{tbSurname.Text}','{tbName.Text}','{tbPatronymic.Text}','{tbEmail.Text}','{tbSeries.Text}','{tbNumber.Text}','{tbPhone.Text}','{tbLogin.Text}','{tbPassword.Text}','false','{cbPost.SelectedValue}')", connectionString);
                                tbSurname.Text = tbName.Text = tbPatronymic.Text = tbEmail.Text = tbPhone.Text = tbSeries.Text = tbNumber.Text = tbLogin.Text = tbPassword.Text = cbPost.Text = "";
                                command.ExecuteNonQuery();
                            }
                            else
                                MessageBox.Show("Пароль должен содержать\n• Минимум 8 символов\n• Минимум 1 прописная буква\n• Минимум 1 цифра\n• Один из спецсимволов: ! @ # $ % ^!", "Пароль", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                        MessageBox.Show("Заполните данные", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (NpgsqlException ex) { MessageBox.Show(ex.Message); }
                finally { Refresh(); }
            }
            if (e.Source == btnDelete)
            {
                try
                {
                    if (row != null)
                    {
                        command = new NpgsqlCommand($@"call Employee_Update('{(int)row["Табельный номер"]}','{tbSurname.Text}','{tbName.Text}','{tbPatronymic.Text}','{tbEmail.Text}','{tbSeries.Text}','{tbNumber.Text}','{tbPhone.Text}','{tbLogin.Text}','{tbPassword.Text}','true','{cbPost.SelectedValue}')", connectionString);
                        tbSurname.Text = tbName.Text = tbPatronymic.Text = tbEmail.Text = tbPhone.Text = tbSeries.Text = tbNumber.Text = tbLogin.Text = tbPassword.Text = cbPost.Text = "";
                        command.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException ex) { MessageBox.Show(ex.Message); }
                finally { Refresh(); }
            }
            if (e.Source == btnEmployeeRestore)
            {
                if (row != null)
                {
                    command = new NpgsqlCommand($@"call Employee_Update('{(int)row["Табельный номер"]}','{tbSurname.Text}','{tbName.Text}','{tbPatronymic.Text}','{tbEmail.Text}','{tbSeries.Text}','{tbNumber.Text}','{tbPhone.Text}','{tbLogin.Text}','{tbPassword.Text}','false','{cbPost.SelectedValue}')", connectionString);
                    command.ExecuteNonQuery();
                    DeletedRefresh();
                }
            }
        }
        /// <summary>
        /// Метод вывода данных в поля
        /// </summary>
        /// <param name="sender">ссылка на элемент управления/объект, вызвавший событие</param>
        /// <param name="e">экземпляр класса для классов, содержащих данные событий, и предоставляет данные событий</param>

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg.SelectedItem == null) return;
            DataRowView row = (DataRowView)dg.SelectedItem;
            tbSurname.Text = row["Фамилия"].ToString();
            tbName.Text = row["Имя"].ToString();
            tbPatronymic.Text = row["Отчество"].ToString();
            tbEmail.Text = row["Email"].ToString();
            tbPhone.Text = row["Телефон"].ToString();
            tbSeries.Text = row["Серия паспорта"].ToString();
            tbNumber.Text = row["Номер паспорта"].ToString();
            tbLogin.Text = row["Логин"].ToString();
            tbPassword.Text = row["Пароль"].ToString();
            cbPost.Text = row["Должность"].ToString();
        }
        /// <summary>
        /// Метод обновления данных
        /// </summary>
        public void Refresh()
        {
            command = new NpgsqlCommand($"select ID_Employee as \"Табельный номер\",  Surname as \"Фамилия\",Employee_Name as \"Имя\", patronymic as\"Отчество\", Email as \"Email\", Phone_number as \"Телефон\", Passport_series as \"Серия паспорта\", Passport_number as \"Номер паспорта\", Login as \"Логин\", employee_password as \"Пароль\", Post_name as \"Должность\" FROM Employee  join Post on Post_ID=ID_Post where deleted = false", connectionString);
            dataTable = new DataTable();
            dataTable.Load(command.ExecuteReader());
            dg.ItemsSource = dataTable.DefaultView;
        }

        public void DeletedRefresh()
        {
            command = new NpgsqlCommand($"select ID_Employee as \"Табельный номер\",  Surname as \"Фамилия\",Employee_Name as \"Имя\", patronymic as\"Отчество\", Email as \"Email\", Phone_number as \"Телефон\", Passport_series as \"Серия паспорта\", Passport_number as \"Номер паспорта\", Login as \"Логин\", employee_password as \"Пароль\", Post_name as \"Должность\" FROM Employee  join Post on Post_ID=ID_Post where deleted = true", connectionString);
            dataTable = new DataTable();
            dataTable.Load(command.ExecuteReader());
            dg.ItemsSource = dataTable.DefaultView;
        }
        /// <summary>
        /// Метод, который обрабатывает производит поиск данных
        /// </summary>
        /// <param name="sender">ссылка на элемент управления/объект, вызвавший событие</param>
        /// <param name="e">экземпляр класса для классов, содержащих данные событий, и предоставляет данные событий</param>
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {

            command = new NpgsqlCommand($"select ID_Employee as \"Табельный номер\",  Surname as \"Фамилия\",Employee_Name as \"Имя\", patronymic as\"Отчество\", Email as \"Email\", Phone_number as \"Телефон\"," +
                $" Passport_series as \"Серия паспорта\", Passport_number as \"Номер паспорта\", Login as \"Логин\", employee_password as \"Пароль\", " +
                $"Post_name as \"Должность\" FROM Employee  join Post on Post_ID=ID_Post where deleted = false and Login like '%{Search.Text}%' or Surname like '%{Search.Text}%'" +
                $"or Employee_Name like '%{Search.Text}%' or patronymic like '%{Search.Text}%'", connectionString);
       
            dataTable = new DataTable();
            dataTable.Load(command.ExecuteReader());
            dg.ItemsSource = dataTable.DefaultView;
        }
        public NpgsqlConnection connectionString { get; }

        string strPG_dumpPath = "SET PGPASSWORD=123\r\n\r\ncd /D C:\\Program Files\r\n\r\ncd PostgreSQL\r\n\r\ncd 13\r\n\r\ncd bin\r\n\r\n";
        string strServer = "localhost";
        string strPort = "5432";
        string strDatabaseName = "ClothesShop";

        /// <summary>
        /// Создание бэкапа
        /// </summary>
        /// <param name="pathSave">Пусть сохранения</param>
        public void Backup(string pathSave)
        {
            try
            {
                StreamWriter sw = new StreamWriter("DBBackup.bat");
                // Do not change lines / spaces b/w words.
                StringBuilder strSB = new StringBuilder(strPG_dumpPath);

                if (strSB.Length != 0)
                {
                    //strSB.Append("SET PGPASSWORD=1");
                    strSB.Append("pg_dump.exe --host " + strServer + " --port " + strPort + " --username postgres --format custom --blobs --verbose --file ");
                    strSB.Append("\"" + pathSave + "\"");
                    strSB.Append(" \"" + strDatabaseName + "\"" + "\r\n\r\n");
                    sw.WriteLine(strSB);
                    sw.Dispose();
                    sw.Close();
                    Process processDB = Process.Start("DBBackup.bat");
                    MessageBox.Show("Резервная копия успешно сохранена");
                }
                else
                {
                    MessageBox.Show("Пожалуйста, укажите место для создания резервной копии");
                }
            }
            catch
            { }
        }
        /// <summary>
        /// Метод восстановления базы данных
        /// </summary>
        /// <param name="pathFile">Путь до бэкапа</param>
        public void Restore(string pathFile)
        {
            strDatabaseName = "ClothesShopRestore";
            try
            {
                try {
                    new NpgsqlCommand("CREATE DATABASE \"ClothesShopRestore\"",connectionString).ExecuteNonQuery(); }
                catch
                {
                    new NpgsqlCommand("DROP DATABASE \"ClothesShopRestore\"", connectionString).ExecuteNonQuery();
                    new NpgsqlCommand("CREATE DATABASE \"ClothesShopRestore\"", connectionString).ExecuteNonQuery();
                }

                if (strDatabaseName != "")
                {
                    if (pathFile != "")
                    {
                        StreamWriter sw = new StreamWriter("DBRestore.bat");
                        // Do not change lines / spaces b/w words.
                        StringBuilder strSB = new StringBuilder(strPG_dumpPath);
                        if (strSB.Length != 0)
                        {
                            strSB.Append("pg_restore.exe --host " + strServer +
                               " --port " + strPort + " --username postgres --dbname");
                            strSB.Append(" \"" + strDatabaseName + "\"");
                            strSB.Append(" --verbose ");
                            strSB.Append("\"" + pathFile + "\"");
                            sw.WriteLine(strSB);
                            sw.Dispose();
                            sw.Close();
                            Process processDB = Process.Start("DBRestore.bat");
                            MessageBox.Show("Успешный backup данных");
                        }
                        else
                        {
                            MessageBox.Show("Пожалуйста, введите путь сохранения, чтобы получить резервную копию");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста введите название базы данных");
                }
            }
            catch
            { }
        }

    }
}
