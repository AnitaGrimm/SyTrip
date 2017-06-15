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
using дипломная_работа.Resources;

namespace дипломная_работа.Controls
{
    /// <summary>
    /// Логика взаимодействия для FlightInfo.xaml
    /// </summary>
    public partial class FlightInfo : UserControl
    {
        public FlightInfo()
        {
            InitializeComponent();
        }
        public void SetData(ResultItemInfo info)
        {
            DepDate.Text = "Дата отправления: " + info.DeaprtureDate.Text; 
            ArDate.Text = "Дата прибытия: " + info.ArrivalDate.Text;
            DepPlace.Text = "Место отправления: "+ info.DeaprturePlace.Text; 
            ArPlace.Text = "Место прибытия: " + info.ArrivalPlace.Text;
        }
    }
}
