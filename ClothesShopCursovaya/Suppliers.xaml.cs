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
    /// Логика взаимодействия для Suppliers.xaml
    /// </summary>
    public partial class Suppliers : Page
    {
        Authorisation auth = Application.Current.Windows.OfType<Authorisation>().FirstOrDefault();
        NpgsqlCommand command = new NpgsqlCommand();
        Helper helper = new Helper();
        int sort = 0;
        int ID_Supplier = 0;
        int ID_Employe;
        public Suppliers( int id)
        {
            InitializeComponent();
            connectionString = new NpgsqlConnection(helper.connect);
            connectionString.Open();
            ID_Employe = id; 
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
            if (e.Source == btnRequest)
            {
                auth.MainFrame.NavigationService.Navigate(new Request(ID_Employe));
            }
            if (e.Source == btnContracts)
            {
                auth.MainFrame.NavigationService.Navigate(new Contracts(ID_Employe));
            }
            if (e.Source == btnProducts)
            {
                auth.MainFrame.NavigationService.Navigate(new Products(ID_Employe));
            }
            if (e.Source == btnAdd)
            {
                try
                {
                    if (helper.IsNull(tbName.Text) && helper.IsNull(Address.Text))
                    {
                       new NpgsqlCommand($@"call Supplier_Insert('{tbName.Text}','{Address.Text}')", connectionString).ExecuteNonQuery();
                    }
                    else
                        MessageBox.Show("Заполните данные", "Добавление поставщика", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (NpgsqlException ex){MessageBox.Show(ex.Message);}
                finally{Refresh();}
            }
            if (e.Source == btnChange)
            {
                try
                {
                    if (lbDB.SelectedItem != null)
                    {
                        if (helper.IsNull(tbName.Text)&& helper.IsNull(Address.Text))
                        {
                            command = new NpgsqlCommand($@"call Supplier_Update ({ID_Supplier},'{tbName.Text}','{Address.Text}')", connectionString);
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                        MessageBox.Show("Выберите поставщика", "Изменение поставщика", MessageBoxButton.OK, MessageBoxImage.Warning);    
                }
                catch (NpgsqlException ex) { MessageBox.Show(ex.Message); }
                finally { Refresh(); }
            }
            if (e.Source == btnDelete)
            {
                try
                {
                    if (lbDB.SelectedItem != null)
                    {
                        command = new NpgsqlCommand($@"call Supplier_Delete ({ID_Supplier})", connectionString);
                        command.ExecuteNonQuery();
                    }
                    else
                        MessageBox.Show("Выберите поставщика", "Удаление поставщика", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (NpgsqlException ex) { MessageBox.Show(ex.Message); }
                finally { Refresh(); }
            }
            if (e.Source == Sort)
            {
                if (sort == 0)
                {
                    lbDB.Items.Clear();
                    command = new NpgsqlCommand($@"select Company_name from Supplier order by Company_name desc ", connectionString);
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
                    command = new NpgsqlCommand($@"select Company_name from Supplier order by Company_name asc ", connectionString);
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
            if (e.Source == lbDB)
            {
                command = new NpgsqlCommand($@"select ID_Supplier, Company_name, Legal_address from Supplier where Company_name='{lbDB.SelectedItem}' ", connectionString);
                NpgsqlDataReader dataReader = null;
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    ID_Supplier = (int)dataReader[$@"ID_Supplier"];
                    tbName.Text = dataReader[@"Company_name"].ToString();
                    Address.Text = dataReader[@"Legal_address"].ToString();
                }
                dataReader.Close();
            }
        }
        /// <summary>
        /// Метод обновления данных
        /// </summary>

        public void Refresh()
        {
            lbDB.Items.Clear();
            tbName.Text = "";
            Address.Text = "";
            command = new NpgsqlCommand($@"select Company_name from Supplier", connectionString);
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
            tbName.Text = "";
            Address.Text = "";
            command = new NpgsqlCommand($@"select Company_name from Supplier where Company_name like '%{Search.Text}%' or Legal_address like '%{Search.Text}%'", connectionString);
            NpgsqlDataReader dataReader = null;
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                lbDB.Items.Add(dataReader.GetValue(0).ToString());
            }
            dataReader.Close();
        }
        public NpgsqlConnection connectionString { get; }
    }
}
