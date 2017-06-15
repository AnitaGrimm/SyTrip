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

namespace дипломная_работа
{
    /// <summary>
    /// Логика взаимодействия для AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        Frame MainFrame;
        WelcomePage WelcomePage;
        public AboutPage(WelcomePage WelcomePage, Frame MainFrame)
        {
            InitializeComponent();
            this.MainFrame = MainFrame;
            this.WelcomePage = WelcomePage;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(
                WelcomePage,
                null
                );
        }
    }
}
