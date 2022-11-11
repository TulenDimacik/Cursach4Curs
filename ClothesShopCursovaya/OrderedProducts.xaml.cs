﻿using ClothesShopCursovaya.Extensions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
    /// Логика взаимодействия для OrderedProducts.xaml
    /// </summary>
    public partial class OrderedProducts : Page
    {
        Authorisation auth = Application.Current.Windows.OfType<Authorisation>().FirstOrDefault();
        Helper helper = new Helper();
        NpgsqlCommand command = new NpgsqlCommand();
        int id_Order;
        int id_Client;
        int id_Ordered_Products;
        int count;
        int Total_Price;
        
        public OrderedProducts(int id_order, int id_client)
        {
            InitializeComponent();
            connectionString = new NpgsqlConnection(helper.connect);
            connectionString.Open();
            id_Order = id_order;
            id_Client = id_client;
            Refresh();
            if(lbDB.Items.Count<=0)
            {
                btnOrder.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
        }
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source == btnBack)
            {
                Application.Current.Windows.OfType<Authorisation>().FirstOrDefault().MainFrame.NavigationService.GoBack();
            }
            if (e.Source == btnOrder)
            {
                try
                {
                    if (helper.IsNull(Address.Text))
                    {
                        command = new NpgsqlCommand($@"select client_email from Client where ID_Client = {id_Client}", connectionString);
                        MailAddress to = new MailAddress(command.ExecuteScalar().ToString());
                        command = new NpgsqlCommand($@"call Order_Update({id_Order},'{Address.Text}','{DateTime.Now.AddDays(3).ToString("dd-MM-yyyy")}',false,true,null,{id_Client})", connectionString);
                        command.ExecuteNonQuery();
                        MailAddress from = new MailAddress("isip_d.a.gordyushin@mpt.ru", "Магазин одежды");
                        MailMessage m = new MailMessage(from, to);
                        m.Subject = $"Заказ №{id_Order}";
                        m.IsBodyHtml = true;
                        command = new NpgsqlCommand($@"select client_name from Client where ID_Client = {id_Client}", connectionString);

                        m.Body += $"Спасибо за ваш заказ,<b>{command.ExecuteScalar()}</b>!<br>\n";
                        for (int i = 0; i<lbDB.Items.Count;i++)
                        {
                            m.Body += lbDB.Items[i] + "<br>";
                        }
                        m.Body += $"Количество товаров: {TotalCount.Text} <br>";
                        m.Body += $"Сумма заказа: {TotalPrice.Text} <br>";
                        m.Body += $"Адрес доставки: {Address.Text} <br>";
                        m.Body += $"Ожидайте доставку <b>{DateTime.Now.AddDays(3).ToString("dd-MM-yyyy")}</b>";
                        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                        smtp.Credentials = new NetworkCredential("isip_d.a.gordyushin@mpt.ru", "Go13rd09");
                        smtp.EnableSsl = true;
                        smtp.Send(m);
                        MessageBox.Show("Заказ успешно оформлен!\nПроверьте почту.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
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
                        command = new NpgsqlCommand($@"call Ordered_Products_Delete({id_Ordered_Products})", connectionString);
                        command.ExecuteNonQuery();
                    }
                    else
                        MessageBox.Show("Выберите товар", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (NpgsqlException ex) { MessageBox.Show(ex.Message); }
                finally { Refresh(); }
            }
            if (e.Source == lbDB)
            {
                command = new NpgsqlCommand($@"select ID_Ordered_Products from Ordered_Products join Product on Product_ID = ID_Product join Size on Product.Size_ID = Size.ID_size join Orderr on Order_ID = ID_Order where Order_ID={id_Order} and concat (products_count,' ',product_name,' ',size_name,' ',Price,'₽') = '{lbDB.SelectedItem}'   ", connectionString);
                NpgsqlDataReader dataReader = null;
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    id_Ordered_Products = (int)dataReader[$@"ID_Ordered_Products"];
                }
                dataReader.Close();
            }
        }

        public void Refresh()
        {
            lbDB.Items.Clear();
            count = 0;
            Total_Price = 0;
            command = new NpgsqlCommand($@"select concat (products_count,' ',product_name,' ',size_name,' ',Price,'₽'),products_count,Price from Ordered_products join Product on Product_ID = ID_Product join Size on Product.Size_ID = Size.ID_size join Orderr on Order_ID = ID_Order where Order_ID = {id_Order} and Payment = false", connectionString);
            NpgsqlDataReader dataReader = null;
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                lbDB.Items.Add(dataReader.GetValue(0).ToString());
                count += (int)dataReader[$@"products_count"];
                Total_Price += (int)dataReader[$@"products_count"] * (int)dataReader[$@"Price"];
            }
            dataReader.Close();
            TotalCount.Text = count.ToString();
            TotalPrice.Text = Total_Price.ToString();
        }
      
        public NpgsqlConnection connectionString { get; }
    }
}
