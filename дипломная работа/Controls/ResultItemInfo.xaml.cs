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
        int _selectedRoomsnum= -1;
        public int SelectedRoomsnum
        {
            get { return _selectedRoomsnum; }
            set
            {
                if (value < 0 && value >= resItem.rooms.Count && value!=-1)
                    return;
                _selectedRoomsnum = value;
                if (!IsNativeTown)
                {
                    if (value == -1)
                    {
                        Background = new SolidColorBrush(Colors.Transparent);
                    }
                    else
                    {
                        Background = new SolidColorBrush(Colors.LightGreen);
                        try
                        {
                            CurrencyConverter cc = CommonData.CurrencyConverter;
                            HotelCost = SelectedRooms.Sum(room => cc.getRub(room.total_amount).amount);
                            HotelsCost.Text = string.Format("{0:f2} руб", HotelCost);
                        }
                        catch { }
                    }
                }
            }
        }
        public double HotelCost { get; private set; } = 0;
        public List<AmadeusAPI.Room> SelectedRooms
        {
            get
            {
                if (SelectedRoomsnum == -1)
                    return null;
                try
                {
                    return resItem.rooms[SelectedRoomsnum];
                }
                catch
                {
                    return null;
                }
            }
        }
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
                    CurrencyConverter cc = CommonData.CurrencyConverter;
                    if (SelectedRoomsnum == -1)
                        HotelsCost.Text = string.Format("{0:f2} - {1:f2} руб", resitem.rooms.Select(roomset => roomset.Sum(room => cc.getRub(room.total_amount).amount)).Min(), resitem.rooms.Select(roomset => roomset.Sum(room => cc.getRub(room.total_amount).amount)).Max());
                    else
                        HotelsCost.Text = string.Format("{0:f2} руб", SelectedRooms.Sum(room => cc.getRub(room.total_amount).amount));
                }
                else HotelsCostI.Visibility = Visibility.Collapsed;    
            }
            catch { }
        }
    }
}
