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
    /// Логика взаимодействия для Contracts.xaml
    /// </summary>
    public partial class Contracts : Page
    {
        Authorisation auth = Application.Current.Windows.OfType<Authorisation>().FirstOrDefault();
        NpgsqlCommand command = new NpgsqlCommand();
        DataTable dataTable = new DataTable();
        Helper helper = new Helper();
        int ID_Employe;
        public Contracts(int ID_Employee)
        {
            InitializeComponent();
            connectionString = new NpgsqlConnection(helper.connect);
            connectionString.Open();
            ID_Employe = ID_Employee;
            DataTable dataTable1 = new DataTable();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("select * from Supplier", connectionString);
            adapter.Fill(dataTable1);
            cbSupplier.DisplayMemberPath = "company_name";
            cbSupplier.SelectedValuePath = "id_supplier";
            cbSupplier.ItemsSource = dataTable1.DefaultView;
            dpConclusion.DisplayDateEnd = DateTime.Now;
            dpTermination.DisplayDateStart = DateTime.Now.AddDays(1);
            Refresh();
            
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {

            DataRowView row = (DataRowView)dg.SelectedItem;
            if (e.Source == btnBack)
            {
                auth.MainFrame.NavigationService.Navigate(new SignIn());
            }
            if (e.Source == btnRequest)
            {
                auth.MainFrame.NavigationService.Navigate(new Request(ID_Employe));
            }
            if (e.Source == btnSupplier)
            {
                auth.MainFrame.NavigationService.Navigate(new Suppliers(ID_Employe));
            }
            if (e.Source == btnProducts)
            {
                auth.MainFrame.NavigationService.Navigate(new Products(ID_Employe));
            }
            if (e.Source == cbUrgent)
            {
                if(cbUrgent.IsChecked==true)
                {
                    dpTermination.IsEnabled = true;
                }
                else
                {
                    dpTermination.IsEnabled = false;
                    dpTermination.Text = "";
                }
            }

            if (e.Source == btnAdd)
            {
                try
                {
                    if (cbUrgent.IsChecked == true)
                    {
                        if (helper.IsNull(cbSupplier.Text) && helper.IsNull(dpConclusion.Text) && helper.IsNull(dpTermination.Text))
                        {
                            command = new NpgsqlCommand($@"call Contract_Insert('{dpConclusion.Text}',true,'{dpTermination.Text}','{cbSupplier.SelectedValue}',{ID_Employe})", connectionString);
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        if (helper.IsNull(cbSupplier.Text) && helper.IsNull(dpConclusion.Text))
                        {
                            command = new NpgsqlCommand($@"call Contract_Insert('{dpConclusion.Text}',false,NULL,'{cbSupplier.SelectedValue}',{ID_Employe})", connectionString);
                            command.ExecuteNonQuery();
                        }
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
                        if (cbUrgent.IsChecked == true)
                        {
                            if (helper.IsNull(cbSupplier.Text) && helper.IsNull(dpConclusion.Text) && helper.IsNull(dpTermination.Text))
                            {
                                command = new NpgsqlCommand($@"call Contract_Update({(int)row["Номер контракта"]}, '{dpConclusion.Text}',true,'{dpTermination.Text}','{cbSupplier.SelectedValue}',{ID_Employe})", connectionString);
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            if (helper.IsNull(cbSupplier.Text) && helper.IsNull(dpConclusion.Text))
                            {
                                command = new NpgsqlCommand($@"call Contract_Update({(int)row["Номер контракта"]},'{dpConclusion.Text}',false,NULL,'{cbSupplier.SelectedValue}',{ID_Employe})", connectionString);
                                command.ExecuteNonQuery();
                            }
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
                        command = new NpgsqlCommand($@"call Contract_Delete({(int)row["Номер контракта"]})", connectionString);
                        command.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException ex) { MessageBox.Show(ex.Message); }
                finally { Refresh(); }
            }
            
        }
        public void Refresh()
        {

            cbSupplier.Text = "";
            dpConclusion.Text = "";
            dpTermination.Text = "";
            command = new NpgsqlCommand($"select ID_Contract as \"Номер контракта\",  concat(Employee.Surname,' ',Employee.Employee_Name) as \"Заключил\", company_name as \"Поставщик\", conclusion_date as \"Дата заключения\", Urgent as \"Срочный\", termination_date as \"Дата расторжения\" FROM Contract join Employee on Employee_ID=ID_Employee join Supplier on Supplier_ID = ID_Supplier", connectionString);
            dataTable = new DataTable();
            dataTable.Load(command.ExecuteReader());
            dg.ItemsSource = dataTable.DefaultView;
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            command = new NpgsqlCommand($"select ID_Contract as \"Номер контракта\",  concat(Employee.Surname,' ',Employee.Employee_Name) as \"Заключил\", company_name as \"Поставщик\", conclusion_date as \"Дата заключения\", Urgent as \"Срочный\", termination_date as \"Дата расторжения\" FROM Contract join Employee on Employee_ID=ID_Employee join Supplier on Supplier_ID = ID_Supplier where Company_name like '%{Search.Text}%' or Employee.Surname  like '%{Search.Text}%' or Employee.Employee_Name like '%{Search.Text}%'", connectionString);
            dataTable = new DataTable();
            dataTable.Load(command.ExecuteReader());
            dg.ItemsSource = dataTable.DefaultView;
        }

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg.SelectedItem == null) return;
            DataRowView row = (DataRowView)dg.SelectedItem;
            cbUrgent.IsChecked = (bool)row["Срочный"];
            if (cbUrgent.IsChecked==true)
                dpTermination.IsEnabled = true;
            else
                dpTermination.IsEnabled = false;
            cbSupplier.Text = row["Поставщик"].ToString();
            dpConclusion.Text = row["Дата заключения"].ToString();
            dpTermination.Text = row["Дата расторжения"].ToString();
        }
        public NpgsqlConnection connectionString { get; }
    }
}
