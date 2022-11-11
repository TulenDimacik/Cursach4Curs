using ClothesShopCursovaya.Extensions;
using Npgsql;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClothesShopCursovaya
{
    /// <summary>
    /// Логика взаимодействия для ChooseCourier.xaml
    /// </summary>
    public partial class ChooseCourier : Page
    {
        Authorisation auth = System.Windows.Application.Current.Windows.OfType<Authorisation>().FirstOrDefault();
        NpgsqlCommand command = new NpgsqlCommand();
        DataTable dataTable = new DataTable();
        Helper helper = new Helper();
        public ChooseCourier()
        {
            InitializeComponent();
            connectionString = new NpgsqlConnection(helper.connect);
            connectionString.Open();
            dataTable = new DataTable();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter($"select id_employee, post_name, concat(surname,' ',Employee_name) as \"Имя\"  from Employee join Post on Post_ID = ID_Post where post_name = 'Доставщик'", connectionString);
            adapter.Fill(dataTable);
            cbWarehouse.DisplayMemberPath = "Имя";
            cbWarehouse.SelectedValuePath = "id_employee";
            cbWarehouse.ItemsSource = dataTable.DefaultView;
            Refresh();
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {

        }
      

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        public void Refresh()
        {
            cbWarehouse.Text = "";
            command = new NpgsqlCommand($"select ID_order as \"Номер заказа\",delivery_address as \"Адрес доставки\", Delivery_date as \"Дата доставки\",delivered as \"Доставлено\", payment as \"Оплачено\",client_email as \"Почта клиента\", concat(surname,' ',employee_name) as \"Курьер\"" +
                $" from Orderr left join Employee on Employee_ID = ID_Employee join Client on Client_ID = ID_Client where payment = true", connectionString);
            dataTable = new DataTable();
            dataTable.Load(command.ExecuteReader());
            dg.ItemsSource = dataTable.DefaultView;

        }
        public NpgsqlConnection connectionString { get; }
    }
}
