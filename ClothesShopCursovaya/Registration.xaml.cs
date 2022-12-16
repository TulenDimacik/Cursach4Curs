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
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        Authorisation auth = Application.Current.Windows.OfType<Authorisation>().FirstOrDefault();
        NpgsqlCommand command = new NpgsqlCommand();
        Helper helper = new Helper();
        int id_Client;

        public Registration(int ID_client)
        {
            InitializeComponent();
            connectionString = new NpgsqlConnection(helper.connect);
            connectionString.Open();
            id_Client = ID_client;
            if (id_Client != 0)
            {
                MainLabel.Content = "Профиль";
                btnRegistration.Content = "Изменить данные";
                command = new NpgsqlCommand($"select * from Client where ID_Client = {id_Client}", connectionString);
                NpgsqlDataReader dataReader = null;
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    tbLogin.Text = dataReader["Client_Login"].ToString();
                    tbEmail.Text = dataReader["Client_email"].ToString();
                    tbName.Text = dataReader["Client_name"].ToString();
                    tbPassword.Text = dataReader["Client_password"].ToString();
                    tbPatronymic.Text = dataReader["Client_patronymic"].ToString();
                    tbSurname.Text = dataReader["Client_surname"].ToString();
                    
                }
                dataReader.Close();
                tbLogin.Text = helper.Decrypt(tbLogin.Text);
                tbPassword.Text = helper.Decrypt(tbPassword.Text);

            }
        }
        /// <summary>
        /// Обработчик кнопок 
        /// </summary>
        /// <param name="sender">ссылка на элемент управления/объект, вызвавший событие</param>
        /// <param name="e">экземпляр класса для классов, содержащих данные событий, и предоставляет данные событий</param>


        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source == btnBack)
            {
                if(id_Client==0)
                    auth.MainFrame.NavigationService.Navigate(new SignIn());
                else
                    auth.MainFrame.NavigationService.Navigate(new ProductChoose(id_Client));
            }
            if (e.Source == btnRegistration)
            {
                try
                {   
                    if (helper.IsNull(tbLogin.Text)&& helper.IsNull(tbName.Text) && helper.IsNull(tbSurname.Text))
                    {
                        if(tbLogin.Text.Length < 8)
                        {
                            MessageBox.Show("Логин должен быть не короче 8 символов", "Логин", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (helper.IsValidEmail(tbEmail.Text))
                            if (helper.IsValidPassword (tbPassword.Text))
                            {
                                if (id_Client == 0)
                                {
                                    command = new NpgsqlCommand($@"call Client_Insert('{tbName.Text}','{tbSurname.Text}','{tbPatronymic.Text}','{helper.Encrypt(tbLogin.Text)}', '{helper.Encrypt(tbPassword.Text)}','{tbEmail.Text}')", connectionString);
                                    auth.MainFrame.NavigationService.Navigate(new SignIn());
                                }
                                else
                                {
                                    command = new NpgsqlCommand($@"call Client_Update({id_Client},'{tbName.Text}','{tbSurname.Text}','{tbPatronymic.Text}','{helper.Encrypt(tbLogin.Text)}', '{helper.Encrypt(tbPassword.Text)}','{tbEmail.Text}')", connectionString);
                                    MessageBox.Show("Данные успешно обновлены","",MessageBoxButton.OK, MessageBoxImage.Information);
                                }

                                command.ExecuteNonQuery();
                            }
                            else
                                MessageBox.Show("Пароль должен содержать\n• Минимум 8 символов\n• Минимум 1 прописная буква\n• Минимум 1 цифра\n• Один из спецсимволов: ! @ # $ % ^!", "Пароль", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public NpgsqlConnection connectionString { get; }

    }
}
