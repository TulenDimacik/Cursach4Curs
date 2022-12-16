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
    /// Логика взаимодействия для Warehouse.xaml
    /// </summary>
    public partial class Warehouse : Page
    {
        Authorisation auth = Application.Current.Windows.OfType<Authorisation>().FirstOrDefault();
        NpgsqlCommand command = new NpgsqlCommand();
        DataTable dataTable = new DataTable();
        Helper helper = new Helper();
        public Warehouse()
        {
            InitializeComponent();
            connectionString = new NpgsqlConnection(helper.connect);
            connectionString.Open();
            Refresh();
        }
        /// <summary>
        /// Метод, который обрабатывает производит поиск данных
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
            if (e.Source == btnInvoice)
            {
                auth.MainFrame.NavigationService.Navigate(new Invoice());
            }
            if (e.Source == btnAdd)
            {
                try
                {
                    if (helper.IsNull(tbAddress.Text) && helper.IsNull(tbShelf.Text))
                    {
                        command = new NpgsqlCommand($@"call Warehouse_Insert('{tbAddress.Text}','{tbShelf.Text}')", connectionString);
                        command.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException ex) { MessageBox.Show(ex.Message); }
                finally { Refresh(); }
            }
            if (e.Source == btnChange)
            {
                try
                {
                    if (row != null)
                    {
                        if (helper.IsNull(tbAddress.Text) && helper.IsNull(tbShelf.Text))   
                        {
                            command = new NpgsqlCommand($@"call Warehouse_Update({(int)row["Номер склада"]},'{tbAddress.Text}','{tbShelf.Text}')", connectionString);
                            command.ExecuteNonQuery();
                        }
                    }

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
                        command = new NpgsqlCommand($@"call Warehouse_Delete({(int)row["Номер склада"]})", connectionString);
                        command.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException ex) { MessageBox.Show(ex.Message); }
                finally { Refresh(); }
            }
        }
        /// <summary>
        /// Метод, который обрабатывает производит поиск данных
        /// </summary>
        /// <param name="sender">ссылка на элемент управления/объект, вызвавший событие</param>
        /// <param name="e">экземпляр класса для классов, содержащих данные событий, и предоставляет данные событий</param>

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            command = new NpgsqlCommand($"select ID_Warehouse as \"Номер склада\", Warehouse_address as \"Адрес склада\", Shelf as \"Ячейка\" FROM Warehouse where Warehouse_address like '%{Search.Text}%' or Shelf     like '%{Search.Text}%'", connectionString);
            dataTable = new DataTable();
            dataTable.Load(command.ExecuteReader());
            dg.ItemsSource = dataTable.DefaultView;
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
            tbAddress.Text = row["Адрес склада"].ToString();
            tbShelf.Text = row["Ячейка"].ToString();
        }
        /// <summary>
        /// Метод обновления данных
        /// </summary>
        public void Refresh()
        {

            tbAddress.Text = "";
            tbShelf.Text = "";
            command = new NpgsqlCommand($"select ID_Warehouse as \"Номер склада\", Warehouse_address as \"Адрес склада\", Shelf as \"Ячейка\" FROM Warehouse", connectionString);
            dataTable = new DataTable();
            dataTable.Load(command.ExecuteReader());
            dg.ItemsSource = dataTable.DefaultView;
        }
        public NpgsqlConnection connectionString { get; }
    }
}
