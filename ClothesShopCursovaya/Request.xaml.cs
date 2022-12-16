using ClothesShopCursovaya.Extensions;
using Npgsql;
using System;
using System.Collections.Generic;
using SD=System.Data;
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
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
namespace ClothesShopCursovaya
{
    /// <summary>
    /// Логика взаимодействия для Request.xaml
    /// </summary>
    public partial class Request : System.Windows.Controls.Page
    {
        Authorisation auth = System.Windows.Application.Current.Windows.OfType<Authorisation>().FirstOrDefault();
        NpgsqlCommand command = new NpgsqlCommand();
        SD.DataTable dataTable = new SD.DataTable();
        Helper helper = new Helper();
        int ID_Employe;
        public Request(int ID_Employee)
        {
            InitializeComponent();
            connectionString = new NpgsqlConnection(helper.connect);
            connectionString.Open();
            ID_Employe = ID_Employee;
            dataTable = new SD.DataTable();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter($"select id_product, concat(product_name,' ',size_name) as \"Товар\"  from Product join Size on Size_ID = ID_Size", connectionString);
            adapter.Fill(dataTable);
            cbProduct.DisplayMemberPath = "Товар";
            cbProduct.SelectedValuePath = "id_product";
            cbProduct.ItemsSource = dataTable.DefaultView;
            dataTable = new SD.DataTable();
            adapter = new NpgsqlDataAdapter("select * from Contract", connectionString);
            adapter.Fill(dataTable);
            cbContract.DisplayMemberPath = "id_contract";
            cbContract.SelectedValuePath = "id_contract";
            cbContract.ItemsSource = dataTable.DefaultView;
            Refresh();
        }
        /// <summary>
        /// Обработчик кнопок 
        /// </summary>
        /// <param name="sender">ссылка на элемент управления/объект, вызвавший событие</param>
        /// <param name="e">экземпляр класса для классов, содержащих данные событий, и предоставляет данные событий</param>

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            SD.DataRowView row = (SD.DataRowView)dg.SelectedItem;
            if (e.Source == btnBack)
            {
                auth.MainFrame.NavigationService.Navigate(new SignIn());
            }
            if (e.Source == btnContract)
            {
                auth.MainFrame.NavigationService.Navigate(new Contracts(ID_Employe));
            }
            if (e.Source == btnSupplier)
            {
                auth.MainFrame.NavigationService.Navigate(new Suppliers(ID_Employe));
            }
            if (e.Source == btnProducts)
            {
                auth.MainFrame.NavigationService.Navigate(new Products(ID_Employe));
            }
            if (e.Source == btnGraph)
            {
                Graph graph = new Graph();
                graph.Show();
            }
            if (e.Source == btnExport)
            {
                Excel.Application excel = new Excel.Application();
                Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
                Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

                for (int j = 0; j < dg.Columns.Count; j++)
                {
                    Range myRange = (Range)sheet1.Cells[1, j + 1];
                    sheet1.Cells[1, j + 1].Font.Bold = true;
                    sheet1.Columns[j + 1].ColumnWidth = 15;
                    myRange.Value2 = dg.Columns[j].Header;
                }
                for (int i = 0; i < dg.Columns.Count; i++)
                {
                    for (int j = 0; j < dg.Items.Count; j++)
                    {
                        TextBlock b = dg.Columns[i].GetCellContent(dg.Items[j]) as TextBlock;
                        Range myRange = (Range)sheet1.Cells[j + 2, i + 1];
                        myRange.Value2 = b.Text;
                    }
                }
                excel.Visible = true;
            }

            if (e.Source == btnAdd)
            {
                try
                {
                    if (helper.IsNull(cbContract.Text) && helper.IsNull(Count.Text) && helper.IsNull(cbProduct.Text))
                    {
                        command = new NpgsqlCommand($@"call Request_Insert({Count.Text},{TotalPrice.Text},'{cbProduct.SelectedValue}','{cbContract.SelectedValue}')", connectionString);
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
                        if (helper.IsNull(cbContract.Text) && helper.IsNull(Count.Text) && helper.IsNull(cbProduct.Text))
                        {
                            command = new NpgsqlCommand($@"call Request_Update({(int)row["Номер поставки"]}, {Count.Text},{TotalPrice.Text},'{cbProduct.SelectedValue}','{cbContract.SelectedValue}')", connectionString);
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
                        command = new NpgsqlCommand($@"call Request_Delete({(int)row["Номер поставки"]})", connectionString);
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
            command = new NpgsqlCommand($"select ID_Request as \"Номер поставки\", Quantity as \"Количество\", Total_price as \"Общая сумма\",  Product_name as \"Товар\", size_name as \"Размер\", Price as \"Цена товара\", ID_Contract as \"Номер контракта\"" +
                $" FROM Request join Product on Product_ID=ID_Product join Contract on Contract_ID = ID_Contract join Size on Product.Size_ID = ID_Size where Product_name like '%{Search.Text}%'", connectionString);
            dataTable = new SD.DataTable();
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
            SD.DataRowView row = (SD.DataRowView)dg.SelectedItem;
            Count.Text = row["Количество"].ToString();
            cbProduct.Text = row["Товар"].ToString() +" "+ row["Размер"].ToString();
            cbContract.Text = row["Номер контракта"].ToString();
            TotalPrice.Text = row["Общая сумма"].ToString();
        }

        private void Count_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(helper.Isnum);
        }
        /// <summary>
        /// Метод обновления данных
        /// </summary>
        public void Refresh()
        {

            Count.Text = "";
            cbProduct.Text = "";
            cbContract.Text = "";
            TotalPrice.Text = "0";
            command = new NpgsqlCommand($"select ID_Request as \"Номер поставки\", Quantity as \"Количество\", Total_price as \"Общая сумма\",  Product_name as \"Товар\", size_name as \"Размер\", Price as \"Цена товара\", ID_Contract as \"Номер контракта\"" +
                $" FROM Request join Product on Product_ID=ID_Product join Contract on Contract_ID = ID_Contract join Size on Product.Size_ID = ID_Size", connectionString);
            dataTable = new SD.DataTable();
            dataTable.Load(command.ExecuteReader());
            dg.ItemsSource = dataTable.DefaultView;
        }
        private void Count_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Count.Text.Replace(" ", "") != "")
            {
                command = new NpgsqlCommand($@"select Request_total_price({Count.Text},{Convert.ToInt32(cbProduct.SelectedValue)})", connectionString);
                TotalPrice.Text = command.ExecuteScalar().ToString();
            }
            else
                TotalPrice.Text = "0";
        }

        private void cbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Count.Text.Replace(" ", "") != "")
            {
                command = new NpgsqlCommand($@"select Request_total_price({Count.Text},{Convert.ToInt32(cbProduct.SelectedValue)})", connectionString);
                TotalPrice.Text = command.ExecuteScalar().ToString();
            }
            else
                TotalPrice.Text = "0";
        }
        public NpgsqlConnection connectionString { get; }
    }
}
