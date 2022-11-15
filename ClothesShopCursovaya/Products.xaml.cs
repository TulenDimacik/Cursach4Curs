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
using ClothesShopCursovaya.Extensions;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace ClothesShopCursovaya
{
    /// <summary>
    /// Логика взаимодействия для Products.xaml
    /// </summary>
    public partial class Products : System.Windows.Controls.Page
    {
        Authorisation auth = System.Windows.Application.Current.Windows.OfType<Authorisation>().FirstOrDefault();
        NpgsqlCommand command = new NpgsqlCommand();
        SD.DataTable dataTable = new SD.DataTable();
        Helper helper = new Helper();
        int ID_Employe;

        public Products(int ID_Employee)
        {
            InitializeComponent();
            connectionString = new NpgsqlConnection(helper.connect);
            connectionString.Open();
            ID_Employe = ID_Employee;
            dataTable = new SD.DataTable();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("select * from Size", connectionString);
            adapter.Fill(dataTable);
            cbSize.DisplayMemberPath = "size_name";
            cbSize.SelectedValuePath = "id_size";
            cbSize.ItemsSource = dataTable.DefaultView;
            dataTable = new SD.DataTable();
            adapter.Fill(dataTable);
            cbFilter.DisplayMemberPath = "size_name";
            cbFilter.SelectedValuePath = "id_size";
            dataTable.Rows.Add(8);
            cbFilter.ItemsSource = dataTable.DefaultView;
            dataTable = new SD.DataTable();
            adapter = new NpgsqlDataAdapter("select * from Product_type", connectionString);
            adapter.Fill(dataTable);
            cbProductType.DisplayMemberPath = "type_name";
            cbProductType.SelectedValuePath = "id_product_type";
            cbProductType.ItemsSource = dataTable.DefaultView;
            Refresh();
        }
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
            if (e.Source == btnRequest)
            {
                auth.MainFrame.NavigationService.Navigate(new Request(ID_Employe));
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
                    if (helper.IsNull(tbName.Text) && helper.IsNull(tbColor.Text) && helper.IsNull(tbMaterial.Text) && helper.IsNull(tbPrice.Text) && helper.IsNull(cbSize.Text) && helper.IsNull(cbProductType.Text))
                    {
                        command = new NpgsqlCommand($@"call Product_Insert('{tbName.Text}','{tbColor.Text}','{tbMaterial.Text}','{tbPrice.Text}','{cbProductType.SelectedValue}','{cbSize.SelectedValue}')", connectionString);
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
                        if (helper.IsNull(tbName.Text) && helper.IsNull(tbColor.Text) && helper.IsNull(tbMaterial.Text) && helper.IsNull(tbPrice.Text) && helper.IsNull(cbSize.Text) && helper.IsNull(cbProductType.Text))
                        {
                            command = new NpgsqlCommand($@"call Product_Update({(int)row["Артикул"]},'{tbName.Text}','{tbColor.Text}','{tbMaterial.Text}','{tbPrice.Text}','{cbProductType.SelectedValue}','{cbSize.SelectedValue}')", connectionString);
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
                        command = new NpgsqlCommand($@"call Product_Delete({(int)row["Артикул"]})", connectionString);
                        command.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException ex) { MessageBox.Show(ex.Message); }
                finally { Refresh(); }
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(Search.Text=="")
            {
                Refresh();
                cbFilter.SelectedIndex = 7;
                return;
            }
            if (cbFilter.SelectedIndex == 7)
                command = new NpgsqlCommand($"select ID_Product as \"Артикул\",  Product_name as \"Название\", Color as \"Цвет\", Material as \"Материал\", Price as \"Цена\", Type_name as \"Вид\", size_name as \"Размер\" " +
                $" FROM Product join Size on Size_ID=ID_Size join Product_type on product_type_ID = ID_product_type where Product_name like '%{Search.Text}%'" +
                $" or Color like '%{Search.Text}%' or Material like '%{Search.Text}%' or Type_name like '%{Search.Text}%'", connectionString);
            else
                command = new NpgsqlCommand($"select ID_Product as \"Артикул\",  Product_name as \"Название\", Color as \"Цвет\", Material as \"Материал\", Price as \"Цена\", Type_name as \"Вид\", size_name as \"Размер\" " +
                $" FROM Product join Size on Size_ID=ID_Size join Product_type on product_type_ID = ID_product_type where size_name = '{cbFilter.Text}'", connectionString);
            dataTable = new SD.DataTable();
            dataTable.Load(command.ExecuteReader());
            dg.ItemsSource = dataTable.DefaultView;
        }

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg.SelectedItem == null) return;
            SD.DataRowView row = (SD.DataRowView)dg.SelectedItem;
            cbProductType.Text = row["Вид"].ToString();
            cbSize.Text = row["Размер"].ToString();
            tbColor.Text = row["Цвет"].ToString();
            tbMaterial.Text = row["Материал"].ToString();
            tbName.Text = row["Название"].ToString();
            tbPrice.Text = row["Цена"].ToString();
        }
        public void Refresh()
        {

            cbProductType.Text = "";
            cbSize.Text = "";
            tbColor.Text = "";
            tbMaterial.Text = "";
            tbName.Text = "";
            tbPrice.Text = "";
            command = new NpgsqlCommand($"select ID_Product as \"Артикул\",  Product_name as \"Название\", Color as \"Цвет\", Material as \"Материал\", Price as \"Цена\", Type_name as \"Вид\", size_name as \"Размер\" " +
                $" FROM Product join Size on Size_ID=ID_Size join Product_type on product_type_ID = ID_product_type", connectionString);
            dataTable = new SD.DataTable();
            dataTable.Load(command.ExecuteReader());
            dg.ItemsSource = dataTable.DefaultView;
        }

        private void tbPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(helper.Isnum);
        }
        public NpgsqlConnection connectionString { get; }

        private void cbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cbFilter.SelectedIndex==7)
                Refresh();
            else
            {
                command = new NpgsqlCommand($"select ID_Product as \"Артикул\",  Product_name as \"Название\", Color as \"Цвет\", Material as \"Материал\", Price as \"Цена\", Type_name as \"Вид\", size_name as \"Размер\" " +
                                $" FROM Product join Size on Size_ID=ID_Size join Product_type on product_type_ID = ID_product_type where size_id = '{cbFilter.SelectedValue}'", connectionString);
                dataTable = new SD.DataTable();
                dataTable.Load(command.ExecuteReader());
                dg.ItemsSource = dataTable.DefaultView;
            }
            
        }
    }
}
