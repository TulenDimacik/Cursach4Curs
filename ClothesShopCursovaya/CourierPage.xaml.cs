using ClothesShopCursovaya.Extensions;
using Npgsql;
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

namespace ClothesShopCursovaya
{
    /// <summary>
    /// Логика взаимодействия для CourierPage.xaml
    /// </summary>
    public partial class CourierPage : Page
    {
        Authorisation auth = Application.Current.Windows.OfType<Authorisation>().FirstOrDefault();
        NpgsqlCommand command = new NpgsqlCommand();
        Helper helper = new Helper();
        int id_employee;
        DateTime date = DateTime.Now;
        public CourierPage(int ID_Employee)
        {
            InitializeComponent();
            id_employee = ID_Employee;
            connectionString = new NpgsqlConnection(helper.connect);
            connectionString.Open();
            Refresh();
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source == btnBack)
                Application.Current.Windows.OfType<Authorisation>().FirstOrDefault().MainFrame.NavigationService.GoBack();
            if (e.Source == btnAdd)
            {
                try
                {
                    if (lbDB.SelectedItem != null)
                    {
                        command = new NpgsqlCommand($@"select * from Orderr where concat('№ Заказа: ',ID_order,' ',delivery_address,' Доставить: ',Delivery_date)='{lbDB.SelectedItem}' ", connectionString);
                        NpgsqlDataReader dataReader = null;
                        dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        { date = Convert.ToDateTime(dataReader[$@"delivery_date"]); }
                        
                        if(date<DateTime.Now || date>DateTime.Now)
                        {
                            MessageBox.Show("Нельзя доставить");
                            dataReader.Close();
                            return;
                        }
                        while (dataReader.Read())
                        {
                            command = new NpgsqlCommand($@"call Order_Update({(int)dataReader[$@"ID_order"]},'{dataReader[$@"delivery_address"]}','{dataReader[$@"delivery_date"]}',true,'{dataReader[$@"payment"]}','{id_employee}',{dataReader[$@"Client_ID"]})", connectionString);
                        }
                        command.ExecuteNonQuery();
                    }
                    else
                        MessageBox.Show("Выберите заказ", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                catch (NpgsqlException ex) { MessageBox.Show(ex.Message); }
                finally { Refresh(); }
            }
        }
        public void Refresh()
        {
            lbDB.Items.Clear();
            command = new NpgsqlCommand($@"select concat('№ Заказа: ',ID_order,' | Адрес: ',delivery_address,' | Доставить: ',Delivery_date),ID_order, delivery_address, Delivery_date, client_email from Orderr join Employee on Employee_ID = ID_Employee join Client on Client_ID = ID_Client where payment = true and delivered = false and Employee_ID = {id_employee} ORDER BY ID_Order ASC", connectionString);
            NpgsqlDataReader dataReader = null;
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                lbDB.Foreground = Brushes.White;
                if (Convert.ToDateTime( dataReader["delivery_date"])<date)
                {
                    lbDB.Foreground = Brushes.Red;
                    lbDB.Items.Add(dataReader.GetValue(0).ToString() + " | Не доставлено!");
                }
                else
                    lbDB.Items.Add(dataReader.GetValue(0).ToString());
            }
            dataReader.Close();
        }
        public NpgsqlConnection connectionString { get; }
    }
}
