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
using дипломная_работа.Helpers;
using дипломная_работа.Model;
using дипломная_работа.Resources;

namespace дипломная_работа.Controls
{
    /// <summary>
    /// Логика взаимодействия для ResultItemInfo.xaml
    /// </summary>
    public partial class ResultItemInfo : UserControl
    {
        public double HotelCost { get; private set; } = 0;
        public List<AmadeusAPI.Room> Rooms { get; set; }
        public bool IsNativeTown { get; private set; } = false;
        public ResultItem resItem { get; private set; }
        public ResultItemInfo(ResultItem resitem)
        {
            InitializeComponent();
            resItem = resitem;
            try
            {
                var defaultDate = new DateTime();
                TownName.Text = resItem.Town.Name_rus == "" ? resItem.Town.Name : resItem.Town.Name_rus;
                if (resitem.ArrivalPlace != null)
                    ArrivalPlace.Text = resitem.ArrivalPlace.Name_rus == "" ? resitem.ArrivalPlace.Name : resitem.ArrivalPlace.Name_rus;
                else
                {
                    ArrivalPlaceI.Visibility = Visibility.Collapsed;
                    IsNativeTown = true;
                }
                if (resitem.ArrivalDate != defaultDate)
                    ArrivalDate.Text = resitem.ArrivalDate.ToShortDateString() + " " + resitem.ArrivalDate.ToShortTimeString();
                else {
                    ArrivalDateI.Visibility = Visibility.Collapsed;
                    IsNativeTown = true;
                }
                if (resitem.DeparturePlace != null)
                    DeaprturePlace.Text = resitem.DeparturePlace.Name_rus == "" ? resitem.DeparturePlace.Name : resitem.DeparturePlace.Name_rus;
                else {
                    DeaprturePlaceI.Visibility = Visibility.Collapsed;
                    IsNativeTown = true;
                }
                if (resitem.DepatureDate != defaultDate)
                    DeaprtureDate.Text = resitem.DepatureDate.ToShortDateString() + " " + resitem.DepatureDate.ToShortTimeString();
                else {
                    DeaprtureDateI.Visibility = Visibility.Collapsed;
                    IsNativeTown = true;
                }
                if (!IsNativeTown)
                {
                    Rooms = resitem.rooms;
                }
                else HotelsCostI.Visibility = Visibility.Collapsed;    
            }
            catch { }
        }
    }
}
