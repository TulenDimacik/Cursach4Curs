using ClothesShopCursovaya.Extensions;
using Npgsql;
using System;
using System.Collections.Generic;
using SD = System.Data;
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
    /// Логика взаимодействия для Invoice.xaml
    /// </summary>
    public partial class Invoice : System.Windows.Controls.Page
    {
        Authorisation auth = System.Windows.Application.Current.Windows.OfType<Authorisation>().FirstOrDefault();
        NpgsqlCommand command = new NpgsqlCommand();
        SD.DataTable dataTable = new SD.DataTable();
        Helper helper = new Helper();
        public Invoice()
        {
            InitializeComponent();
            connectionString = new NpgsqlConnection(helper.connect);
            connectionString.Open();
            dataTable = new SD.DataTable();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter($"select id_warehouse, concat(warehouse_address,' ',Shelf) as \"Склад\"  from Warehouse", connectionString);
            adapter.Fill(dataTable);
            cbWarehouse.DisplayMemberPath = "Склад";
            cbWarehouse.SelectedValuePath = "id_warehouse";
            cbWarehouse.ItemsSource = dataTable.DefaultView;
            dataTable = new SD.DataTable();
            adapter = new NpgsqlDataAdapter("select id_request,concat(product_name,' ',size_name,' ',Quantity) as \"Поставка\"  from request join Product on Product_ID=ID_Product join Size on Product.Size_ID = ID_Size ", connectionString);
            adapter.Fill(dataTable);
            cbRequest.DisplayMemberPath = "Поставка";
            cbRequest.SelectedValuePath = "id_request";
            cbRequest.ItemsSource = dataTable.DefaultView;
            dpInvoiceDate.DisplayDateEnd = DateTime.Now;
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
            if (e.Source == btnWarehouse)
            {
                auth.MainFrame.NavigationService.Navigate(new Warehouse());
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
                    if (helper.IsNull(cbWarehouse.Text) && helper.IsNull(cbRequest.Text) && helper.IsNull(dpInvoiceDate.Text))
                    {
                        command = new NpgsqlCommand($@"call Invoice_Insert('{dpInvoiceDate.Text}',{tbInShelf.Text},'{cbWarehouse.SelectedValue}','{cbRequest.SelectedValue}')", connectionString);
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
                        if (helper.IsNull(cbWarehouse.Text) && helper.IsNull(cbRequest.Text) && helper.IsNull(dpInvoiceDate.Text))
                        {
                            command = new NpgsqlCommand($@"call Invoice_Update({(int)row["Номер накладной"]}, '{dpInvoiceDate.Text}',{tbInShelf.Text},'{cbWarehouse.SelectedValue}','{cbRequest.SelectedValue}')", connectionString);
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
                        command = new NpgsqlCommand($@"call Invoice_Delete({(int)row["Номер накладной"]})", connectionString);
                        command.ExecuteNonQuery();
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

            tbInShelf.Text = "0";
            dpInvoiceDate.Text = "";
            cbRequest.Text = "";
            cbWarehouse.Text = "";
            command = new NpgsqlCommand($"select ID_Invoice as \"Номер накладной\", invoice_date as \"Дата накладной\", Product_name as \"Товар\",size_name as \"Размер\", shelf_count as \"Количество\", Warehouse_address as \"Склад\", Shelf as \"Ячейка\", ID_request as \"Номер поставки\"" +
                $" FROM Invoice join Warehouse on Warehouse_ID = ID_Warehouse join Request on Request_ID = ID_Request join Product on Request.Product_ID=ID_Product join Size on Product.Size_ID = ID_Size", connectionString);
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
            tbInShelf.Text = row["Количество"].ToString();
            cbRequest.Text = row["Товар"].ToString() + " " + row["Размер"].ToString() + " " + row["Количество"].ToString();
            cbWarehouse.Text = row["Склад"].ToString() + " " + row["Ячейка"].ToString();
            dpInvoiceDate.Text = row["Дата накладной"].ToString();
        }
        /// <summary>
        /// Метод, который обрабатывает производит поиск данных
        /// </summary>
        /// <param name="sender">ссылка на элемент управления/объект, вызвавший событие</param>
        /// <param name="e">экземпляр класса для классов, содержащих данные событий, и предоставляет данные событий</param>

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            command = new NpgsqlCommand($"select ID_Invoice as \"Номер накладной\", invoice_date as \"Дата накладной\", Product_name as \"Товар\",size_name as \"Размер\", shelf_count as \"Количество\", Warehouse_address as \"Склад\", Shelf as \"Ячейка\", ID_request as \"Номер поставки\"" +
                $" FROM Invoice join Warehouse on Warehouse_ID = ID_Warehouse join Request on Request_ID = ID_Request join Product on Request.Product_ID=ID_Product join Size on Product.Size_ID = ID_Size where Product_name like '%{Search.Text}%' or Shelf like '%{Search.Text}%'", connectionString);
            dataTable = new SD.DataTable();
            dataTable.Load(command.ExecuteReader());
            dg.ItemsSource = dataTable.DefaultView;
        }

        private void cbRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbRequest.SelectedIndex!=-1)
            {   
                command = new NpgsqlCommand($@"select Quantity from Request where id_request = {Convert.ToInt32(cbRequest.SelectedValue)}", connectionString);
                tbInShelf.Text = command.ExecuteScalar().ToString();
            }
        }
        public NpgsqlConnection connectionString { get; }
    }
}
