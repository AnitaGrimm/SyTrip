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
using дипломная_работа.Model;

namespace дипломная_работа
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool IsInitialed = false;
        public MainWindow()
        {
            InitializeComponent();
            if (!IsInitialed)
            {
                MainFrame.Navigate(
                    new InitialPage(MainFrame, this),
                    null);
                IsInitialed = true;
            }
            else
                MainFrame.Navigate(
                    new WelcomePage(MainFrame),
                    null);
        }
        
    }
}
