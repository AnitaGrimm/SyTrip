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
using дипломная_работа.Resources;

namespace дипломная_работа.Controls
{
    /// <summary>
    /// Логика взаимодействия для HotelInfo.xaml
    /// </summary>
    public partial class HotelInfo : UserControl
    {
        AmadeusAPI.Room Room;
        public HotelInfo(AmadeusAPI.Room Room)
        {
            InitializeComponent();
            this.Room = Room;
            try
            {
                HotelName.Text = "Название: " + Room.hotel.property_name;
                HotelRoomAddress.Text = "Адрес: "+Room.hotel.address.line1;
                HotelRoomDescription.Text = "Описание: " + Room.description.Replace('\"','\0').Replace('{','\0').Replace('}', '\0');
                HotelRoomInfo.Text = "Тип комнаты: " + Room.room_type_info.room_type + ", " + Room.room_type_info.bed_type + ", " + Room.room_type_info.number_of_beds + " beds";
                CurrencyConverter cc = CommonData.CurrencyConverter;
                HotelRoomPrice.Text = "Стоимость: " + String.Format("{0:f2} rub", cc.getRub(Room.total_amount).amount);
                try
                {
                    HotelRoomStars.Text = "Рейтинг: " + String.Format("{0:f2}", Room.hotel.awards.Where(x => { double d; return double.TryParse(x.rating, out d); }).Average(x => double.Parse(x.rating)));
                }
                catch
                {
                    HotelRoomStars.Text = "";
                }
            }
            catch
            {

            }
        }
    }
}
