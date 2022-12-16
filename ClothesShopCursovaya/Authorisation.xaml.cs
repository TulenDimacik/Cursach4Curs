
using System.Windows;


namespace ClothesShopCursovaya
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class Authorisation : Window
    {
        public Authorisation()
        {
            InitializeComponent();
            MainFrame.NavigationService.Navigate(new SignIn());
            
        }
    }
}
