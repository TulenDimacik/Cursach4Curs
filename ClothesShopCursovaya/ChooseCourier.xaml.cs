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
            dataTable.Rows.Add(0);
            cbWarehouse.ItemsSource = dataTable.DefaultView;
            Refresh();
        }
        /// <summary>
        /// Обработчик кнопок 
        /// </summary>
        /// <param name="sender">ссылка на элемент управления/объект, вызвавший событие</param>
        /// <param name="e">экземпляр класса для классов, содержащих данные событий, и предоставляет данные событий</param>
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)dg.SelectedItem;
            if(e.Source==btnBack)
                Application.Current.Windows.OfType<Authorisation>().FirstOrDefault().MainFrame.NavigationService.GoBack();

            if (e.Source == btnChange)
            {
                try
                {
                    if (row != null)
                    {
                        NpgsqlDataReader dataReader = null;
                        command = new NpgsqlCommand($@"Select ID_Client from Client where client_email='{row["Почта клиента"]}'", connectionString);
                        dataReader = command.ExecuteReader();
                        int id = 0;
                        while (dataReader.Read())
                        {
                            id = (int)dataReader["ID_Client"];
                        }
                        dataReader.Close();
                        if (cbWarehouse.SelectedValue != null)
                        {
                            if (cbWarehouse.SelectedValue.ToString() != "0")
                                command = new NpgsqlCommand($@"call Order_Update({(int)row["Номер заказа"]},'{row["Адрес доставки"]}','{row["Дата доставки"]}',{row["Доставлено"]},'{row["Оплачено"]}','{cbWarehouse.SelectedValue}',{id})", connectionString);
                            else
                                command = new NpgsqlCommand($@"call Order_Update({(int)row["Номер заказа"]},'{row["Адрес доставки"]}','{row["Дата доставки"]}',{row["Доставлено"]},'{row["Оплачено"]}',null,{id})", connectionString);
                            command.ExecuteNonQuery();
                        }
                        else
                            MessageBox.Show("Выберите курьера","",MessageBoxButton.OK,MessageBoxImage.Warning);
                    }
                    
                }
                catch (NpgsqlException ex) { MessageBox.Show(ex.Message); }
                finally { Refresh(); }
            }
        }
        /// <summary>
        /// Метод обновления данных
        /// </summary>
        public void Refresh()
        {
            command = new NpgsqlCommand($"select ID_order as \"Номер заказа\",delivery_address as \"Адрес доставки\", Delivery_date as \"Дата доставки\",delivered as \"Доставлено\",payment as \"Оплачено\",client_email as \"Почта клиента\", concat(surname,' ',employee_name) as \"Курьер\"" +
                $" from Orderr left join Employee on Employee_ID = ID_Employee join Client on Client_ID = ID_Client where payment = true", connectionString);
            dataTable = new DataTable();
            dataTable.Load(command.ExecuteReader());
            dg.ItemsSource = dataTable.DefaultView;

        }
        public NpgsqlConnection connectionString { get; }
    }
}
