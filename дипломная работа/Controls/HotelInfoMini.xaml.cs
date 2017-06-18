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
using static дипломная_работа.Helpers.AmadeusAPI;

namespace дипломная_работа.Controls
{
    /// <summary>
    /// Логика взаимодействия для HotelInfoMini.xaml
    /// </summary>
    public partial class HotelInfoMini : UserControl
    {
        Room Room;
        public HotelInfoMini(Room Room, string price)
        {
            InitializeComponent();
            this.Room = Room;
            Name.Text = Room?.hotel?.property_name ?? "";
            Price.Text = price;
        }
    }
}
