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
    /// Логика взаимодействия для ProductChoose.xaml
    /// </summary>
    public partial class ProductChoose : Page
    {
        Authorisation auth = Application.Current.Windows.OfType<Authorisation>().FirstOrDefault();
        Helper helper = new Helper();
        NpgsqlCommand command = new NpgsqlCommand();
        int id_Client;
        int id_Order;
        int id_Product;
        int sort = 1;
        public ProductChoose(int ID_Client)
        {   
            InitializeComponent();
            id_Client = ID_Client; 
            connectionString = new NpgsqlConnection(helper.connect);
            connectionString.Open();
            command = new NpgsqlCommand($"select count(*) from Orderr where Client_ID = {id_Client} and Payment = false", connectionString);
            if (command.ExecuteScalar().ToString() == "0")
            {
                command = new NpgsqlCommand($@"call Order_insert(Null,Null,false,false,null,{id_Client})", connectionString);
                command.ExecuteNonQuery();
            }
            command = new NpgsqlCommand($"select ID_Order from Orderr where Client_ID = {id_Client} and Payment = false", connectionString);
            id_Order = (int)command.ExecuteScalar();

            Refresh();
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
                auth.MainFrame.NavigationService.Navigate(new SignIn());
            }
            if (e.Source == btnOrderedProducts)
            {
                auth.MainFrame.NavigationService.Navigate(new OrderedProducts(id_Order,id_Client));
            }
            if (e.Source == btnProfile)
            {
                auth.MainFrame.NavigationService.Navigate(new Registration(id_Client));
            }
            if (e.Source == btnAddProduct)
            {
                try
                {
                    if (lbDB.SelectedItem != null)
                    {
                        if (Convert.ToInt32(tbCount.Text) > 0)
                        {
                            command = new NpgsqlCommand($@"call ordered_products_insert({tbPrice.Text},{tbCount.Text},{id_Order},{id_Product})", connectionString);
                            command.ExecuteNonQuery();
                            SetVisible();
                        }
                        else
                            MessageBox.Show("Неверное количество товара", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                        MessageBox.Show("Выберите Товар", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (NpgsqlException ex) { MessageBox.Show(ex.Message); }
               
            }
            if (e.Source == lbDB)
            {
                if(tbName.Visibility == Visibility.Hidden)
                    SetVisible();
                    command = new NpgsqlCommand($@"select * from Product join Size on size_ID = ID_size join Product_type on product_type_ID = ID_product_type
                    where concat(product_name,' ',size_name,' ',Price,'₽') ='{lbDB.SelectedItem}' ", connectionString);
                    NpgsqlDataReader dataReader = null;
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        id_Product = (int)dataReader[$@"ID_Product"];
                        tbName.Text = dataReader[@"Product_name"].ToString();
                        tbColor.Text = dataReader[@"Color"].ToString();
                        tbMaterial.Text = dataReader[@"Material"].ToString();
                        tbSize.Text = dataReader[@"size_name"].ToString();
                        tbType.Text = dataReader[@"Type_name"].ToString();
                        tbPrice.Text = dataReader[@"Price"].ToString();
                        tbCount.Text = "1";
                    }
                    dataReader.Close();
            }
            if (e.Source == Sort)
            {
                if (sort == 0)
                {
                    lbDB.Items.Clear();
                    command = new NpgsqlCommand($@"select concat (product_name,' ',size_name,' ',Price,'₽') from Product join Size on Size_ID = ID_size order by product_name desc ", connectionString);
                    NpgsqlDataReader dataReader = null;
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        lbDB.Items.Add(dataReader.GetValue(0).ToString());
                    }
                    dataReader.Close();
                    Sort.Content = FindResource("asc");
                    sort = 1;
                }
                else
                {
                    lbDB.Items.Clear();
                    command = new NpgsqlCommand($@"select concat (product_name,' ',size_name,' ',Price,'₽') from Product join Size on Size_ID = ID_size order by product_name asc ", connectionString);
                    NpgsqlDataReader dataReader = null;
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        lbDB.Items.Add(dataReader.GetValue(0).ToString());
                    }
                    dataReader.Close();
                    Sort.Content = FindResource("desc");
                    sort = 0;
                }
            }
        }

        /// <summary>
        /// Метод обновления данных
        /// </summary>
        public void Refresh()
        {
            lbDB.Items.Clear();
            command = new NpgsqlCommand($@"select concat (product_name,' ',size_name,' ',Price,'₽') from Product join Size on Size_ID = ID_size", connectionString);
            NpgsqlDataReader dataReader = null;
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                lbDB.Items.Add(dataReader.GetValue(0).ToString());
            }
            dataReader.Close();
        }
        /// <summary>
        /// Метод, который обрабатывает производит поиск данных
        /// </summary>
        /// <param name="sender">ссылка на элемент управления/объект, вызвавший событие</param>
        /// <param name="e">экземпляр класса для классов, содержащих данные событий, и предоставляет данные событий</param>
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbDB.Items.Clear();
            tbCount.Visibility = tbName.Visibility = tbColor.Visibility = tbMaterial.Visibility = tbSize.Visibility = tbType.Visibility =
                  tbPrice.Visibility = lbName.Visibility = lbColor.Visibility = lbMaterial.Visibility = lbSize.Visibility = lbType.Visibility =
                     lbPrice.Visibility = lbCount.Visibility = Visibility.Hidden;
            command = new NpgsqlCommand($@"select concat (product_name,' ',size_name,' ',Price,'₽') from Product  join Size on Size_ID = ID_size where product_name like '%{Search.Text}%'  or size_name like '%{Search.Text}%'", connectionString);
            NpgsqlDataReader dataReader = null;
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                lbDB.Items.Add(dataReader.GetValue(0).ToString());
            }
            dataReader.Close();
        }
        public void SetVisible()
        {
            
            tbName.Text = "";
            tbColor.Text = "";
            tbMaterial.Text = "";
            tbSize.Text = "";
            tbType.Text = "";
            tbPrice.Text = "";
            tbCount.Text = "1";
            if (tbName.Visibility == Visibility.Hidden)
            {

                tbCount.Visibility = tbName.Visibility = tbColor.Visibility= tbMaterial.Visibility= tbSize.Visibility= tbType.Visibility=
                  tbPrice.Visibility= lbName.Visibility= lbColor.Visibility= lbMaterial.Visibility= lbSize.Visibility = lbType.Visibility =
                 lbPrice.Visibility= lbCount.Visibility= Visibility.Visible;
            }
            else
            {
                tbCount.Visibility = tbName.Visibility = tbColor.Visibility = tbMaterial.Visibility = tbSize.Visibility = tbType.Visibility =
                   tbPrice.Visibility = lbName.Visibility = lbColor.Visibility = lbMaterial.Visibility = lbSize.Visibility = lbType.Visibility =
                  lbPrice.Visibility = lbCount.Visibility = Visibility.Hidden;
            }
        }
        public NpgsqlConnection connectionString { get; }

        private void tbCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(helper.Isnum);
        }
    }
}
