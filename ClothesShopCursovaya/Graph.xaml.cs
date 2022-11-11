using ClothesShopCursovaya.Extensions;
using LiveCharts;
using LiveCharts.Wpf;
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
using System.Windows.Shapes;

namespace ClothesShopCursovaya
{
    /// <summary>
    /// Логика взаимодействия для Graph.xaml
    /// </summary>
    public partial class Graph : Window
    {
        Helper helper = new Helper();
        NpgsqlCommand command = new NpgsqlCommand();
        DataTable dataTable = new DataTable();
        List<string> list = new List<string>();
        List<double> list1 = new List<double>();
        List<int> list2 = new List<int>();
        public Graph()
        {
            InitializeComponent();
            connectionString = new NpgsqlConnection(helper.connect);
            connectionString.Open();
            command = new NpgsqlCommand($"select Product_name, Price, quantity FROM Request join Product on Product_ID = ID_Product", connectionString);
            dataTable = new DataTable();
            dataTable.Load(command.ExecuteReader());
            NpgsqlDataReader dataReader = null;
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                list.Add(dataReader[$@"Product_name"].ToString());
                list1.Add(Convert.ToDouble(dataReader[$@"Price"]));
                list2.Add((int)dataReader[$@"quantity"]);
            }
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title="Заказано",
                    Values = new ChartValues<int>(list2),
                }
            };
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Цена",
                Values = new ChartValues<double>(list1)
            });


            BarLabels = new string[list.Count];
            for (int i = 0; i < BarLabels.Length; i++)
                BarLabels[i] = list[i];
            Formatter = values => values.ToString("N");
            DataContext = this;
        }
        public SeriesCollection SeriesCollection { get; set; }
        public string[] BarLabels { get; set; }

        public Func<double, string> Formatter { get; set; }

        public NpgsqlConnection connectionString { get; }
    }
}
