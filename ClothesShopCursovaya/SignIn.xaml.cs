using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using ClothesShopCursovaya.Extensions;

namespace ClothesShopCursovaya
{
    /// <summary>
    /// Логика взаимодействия для SignIn.xaml
    /// </summary>
    public partial class SignIn : Page
    {
        Authorisation auth = Application.Current.Windows.OfType<Authorisation>().FirstOrDefault();
        Helper helper = new Helper();
        
        
        public SignIn()
        {
            InitializeComponent();
            connectionString = new NpgsqlConnection(helper.connect);
        }
        /// <summary>
        /// Обработчик кнопок 
        /// </summary>
        /// <param name="sender">ссылка на элемент управления/объект, вызвавший событие</param>
        /// <param name="e">экземпляр класса для классов, содержащих данные событий, и предоставляет данные событий</param>

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            NpgsqlCommand command = new NpgsqlCommand("");
            if (e.Source == btnpswdRecovery)
            {
                auth.MainFrame.NavigationService.Navigate(new Recovery());
            }
            if (e.Source == btnRegistration)
            {
                auth.MainFrame.NavigationService.Navigate(new Registration(0));
            }
            if (e.Source == btnSignIn)
            {
                connectionString.Open();
                command = new NpgsqlCommand($"select count(*) from Client where Client_login = '{helper.Encrypt(tbLogin.Text)}' and Client_password = '{helper.Encrypt(tbPassword.Password)}'", connectionString);
                if (command.ExecuteScalar().ToString() == "1")
                {
                    int ID_Client = 0;
                    command = new NpgsqlCommand($"select ID_Client from Client where Client_login = '{helper.Encrypt( tbLogin.Text)}' and Client_password = '{helper.Encrypt(tbPassword.Password)}'", connectionString);
                    NpgsqlDataReader dataReader = null;
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        ID_Client = (int)dataReader["ID_Client"];
                    }
                    auth.MainFrame.NavigationService.Navigate(new ProductChoose(ID_Client));
                }
                else if(command.ExecuteScalar().ToString() == "0")
                {
                    string role = "";
                    int ID_Employee = 0;
                    command = new NpgsqlCommand($"select count(*) from Employee where Login = '{tbLogin.Text}' and Employee_password = '{tbPassword.Password}'", connectionString);
                    if (command.ExecuteScalar().ToString() == "1")
                    {
                        command = new NpgsqlCommand($"select ID_Employee, Post_name from Employee join Post on Post_ID = ID_Post where Login = '{tbLogin.Text}' and Employee_password = '{tbPassword.Password}'", connectionString);
                        NpgsqlDataReader dataReader = null;
                        dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        {
                           role = (dataReader["Post_name"].ToString());
                           ID_Employee = (int)dataReader["ID_Employee"];
                        }
                        switch (role)
                        {
                            case "Администратор":
                                auth.MainFrame.NavigationService.Navigate(new AdminPage());
                                break;

                            case "Доставщик":
                                auth.MainFrame.NavigationService.Navigate(new CourierPage(ID_Employee));
                                break;

                            case "Менеджер по закупкам":
                                auth.MainFrame.NavigationService.Navigate(new Suppliers(ID_Employee));
                                break;

                            case "Кассир":
                                auth.MainFrame.NavigationService.Navigate(new ChooseCourier());
                                break;

                            case "Кладовщик":
                                auth.MainFrame.NavigationService.Navigate(new Warehouse());
                                break;
                        }
                    }
                    else
                        MessageBox.Show("Проверьте введенные данные!", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Warning);
                    connectionString.Close();
                }
               
            }

        }
        public NpgsqlConnection connectionString { get;}

    }
}
