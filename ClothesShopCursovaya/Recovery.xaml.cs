using ClothesShopCursovaya.Extensions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
    /// Логика взаимодействия для Recovery.xaml
    /// </summary>
    public partial class Recovery : Page
    {
        int rand;
        Helper helper = new Helper();
        public Recovery()
        {
            InitializeComponent();
            connectionString = new NpgsqlConnection(helper.connect);
            Random r = new Random();
            rand = r.Next(100000, 999999);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source == btnBack)
            {
                Application.Current.Windows.OfType<Authorisation>().FirstOrDefault().MainFrame.NavigationService.GoBack();
            }
            if (e.Source == btnSendCode)
            {
                if (helper.IsValidEmail(Mail.Text))
                {
                    MailAddress from = new MailAddress("isip_d.a.gordyushin@mpt.ru", "Магазин одежды");
                    MailAddress to = new MailAddress(Mail.Text);
                    MailMessage m = new MailMessage(from, to);
                    m.Subject = "Восстановление пароля";
                    m.IsBodyHtml = false;
                    m.Body = Convert.ToString(rand);
                    m.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.Credentials = new NetworkCredential("isip_d.a.gordyushin@mpt.ru", "Go13rd09");
                    smtp.EnableSsl = true;
                    smtp.Send(m);
                    MessageBox.Show("Код для восстановления пароля был отправлен", "Восстановление пароля", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnSendCode.Visibility = Visibility.Hidden;
                    btnConfirm.Visibility = Visibility.Visible;
                    Code.Visibility = Visibility.Visible;
                    Mail.Visibility = Visibility.Hidden;
                    lab.Content = "Код";
                }
                

            }
            if (e.Source == btnConfirm)
            {
                if (rand.ToString() == Code.Text)
                {
                    connectionString.Open();
                    NpgsqlCommand command = new NpgsqlCommand($"select count(*) Employee_password from Employee where email = '{Mail.Text}' ", connectionString);
                    NpgsqlDataReader dataReader = null;

                    if (command.ExecuteScalar().ToString() == "1")
                    {
                        dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            MessageBox.Show("Ваш пароль: " + dataReader[$@"Employee_password"].ToString(), "Пароль", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        dataReader.Close();
                        connectionString.Close();
                    }
                    else
                    {
                        command = new NpgsqlCommand($"select Client_password from Client where Client_email = '{Mail.Text}' ", connectionString);
                        dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            MessageBox.Show("Ваш пароль: " + helper.Decrypt(dataReader[$@"Client_password"].ToString()), "Пароль", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        dataReader.Close();
                        connectionString.Close();
                    }
                }
                else
                    MessageBox.Show("Неверный код", "Ввод кода", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public NpgsqlConnection connectionString { get; }
    }
}
